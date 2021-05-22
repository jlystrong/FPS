Shader "FPS/StandardWeapon"
{
    Properties
    {
        [MainColor] _MainColor("Color", Color) = (1,1,1,1)
        [MainTexture] _MainTex("Albedo", 2D) = "white" {}

        _BumpMap("Normal Map", 2D) = "bump" {}
        _BumpScale("Bump Scale", Range(0.0, 5.0)) = 1.0
        _MaskMap("Mask Map", 2D) = "white" {}
        _Metallic("Metallic", Range(0.0, 3.0)) = 1.0
        _Smoothness("Smoothness", Range(0.0, 3.0)) = 1.0
    }
    SubShader
    {
        Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit" "IgnoreProjector" = "True"}
        LOD 200

        Pass{
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}
            
            Blend One Zero
            ZWrite On
            Cull Back

            HLSLPROGRAM
            #pragma shader_feature_local _NORMALMAP

            #pragma multi_compile _ LIGHTMAP_ON

            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment


            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"

            TEXTURE2D(_MainTex);SAMPLER(sampler_MainTex);
            TEXTURE2D(_BumpMap);SAMPLER(sampler_BumpMap);
            TEXTURE2D(_MaskMap);SAMPLER(sampler_MaskMap);
            CBUFFER_START(UnityPerMaterial)
            half4 _MainColor;
            float4 _MainTex_ST;
            half _BumpScale;
            half _Metallic;
            half _Smoothness;
            CBUFFER_END

            struct Attributes{
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                float2 texcoord     : TEXCOORD0;
                float2 lightmapUV   : TEXCOORD1;
            };
            struct Varyings{
                float2 uv:TEXCOORD0;
                float4 positionCS:SV_POSITION;
                float3 normalWS:TEXCOORD1;
                #if defined(_NORMALMAP)
                float4 tangentWS:TEXCOORD2;
                #endif
                float3 viewDirWS:TEXCOORD3;
                float3 positionWS:TEXCOORD4;
                DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 5);
            };

            Varyings LitPassVertex(Attributes input){
                Varyings output = (Varyings)0;

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
                output.positionCS = vertexInput.positionCS;
                output.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
                output.normalWS = normalInput.normalWS;
                output.positionWS = vertexInput.positionWS;
                #if defined(_NORMALMAP)
                real sign = input.tangentOS.w * GetOddNegativeScale();
                output.tangentWS = half4(normalInput.tangentWS.xyz, sign);
                #endif
                OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
                OUTPUT_SH(output.normalWS.xyz, output.vertexSH);

                return output;
            }

            struct SurfaceInfo{
                half3 albedo;
                half  metallic;
                half  smoothness;
                half3 normalTS;
            };
            struct InputInfo{
                float3  positionWS;
                half3   normalWS;
                half3   viewDirectionWS;
                half3   bakedGI;
            };
            struct BRDFInfo{
                half3 diffuse;
                half3 specular;
                half reflectivity;
                half perceptualRoughness;
                half roughness;
                half roughness2;
                half grazingTerm;
                half normalizationTerm;     // roughness * 4.0 + 2.0
                half roughness2MinusOne;    // roughness^2 - 1.0
            };

            inline void InitializeBRDFInfo(half3 albedo, half metallic, half smoothness, out BRDFInfo outBRDFInfo){
                half oneMinusReflectivity = OneMinusReflectivityMetallic(metallic);

                outBRDFInfo.diffuse=albedo*oneMinusReflectivity;
                outBRDFInfo.specular=lerp(kDieletricSpec.rgb, albedo, metallic);
                outBRDFInfo.reflectivity=1.0 - oneMinusReflectivity;
                outBRDFInfo.perceptualRoughness=PerceptualSmoothnessToPerceptualRoughness(smoothness);
                outBRDFInfo.roughness=max(PerceptualRoughnessToRoughness(outBRDFInfo.perceptualRoughness), HALF_MIN_SQRT);
                outBRDFInfo.roughness2=max(outBRDFInfo.roughness * outBRDFInfo.roughness, HALF_MIN);
                outBRDFInfo.grazingTerm=saturate(smoothness + outBRDFInfo.reflectivity);
                outBRDFInfo.normalizationTerm=outBRDFInfo.roughness * 4.0h + 2.0h;
                outBRDFInfo.roughness2MinusOne=outBRDFInfo.roughness2 - 1.0h;
            }

            half3 GlobalLight(BRDFInfo brdfInfo,half3 bakedGI, half3 normalWS, half3 viewDirectionWS){
                half3 reflectVector = reflect(-viewDirectionWS, normalWS);
                half NoV = saturate(dot(normalWS, viewDirectionWS));
                half fresnelTerm = Pow4(1.0 - NoV);
                half3 indirectDiffuse = bakedGI;
                half3 indirectSpecular = GlossyEnvironmentReflection(reflectVector, brdfInfo.perceptualRoughness, 1);
                half3 color = EnvironmentBRDF(brdfInfo, indirectDiffuse, indirectSpecular, fresnelTerm);
                return color;
            }
            half LightSpecular(BRDFInfo brdfInfo, half3 normalWS, half3 lightDirectionWS, half3 viewDirectionWS){
                float3 halfDir = SafeNormalize(float3(lightDirectionWS) + float3(viewDirectionWS));
                float NoH = saturate(dot(normalWS, halfDir));
                half LoH = saturate(dot(lightDirectionWS, halfDir));
                float d = NoH * NoH * brdfInfo.roughness2MinusOne + 1.00001f;
                half LoH2 = LoH * LoH;
                half specularTerm = brdfInfo.roughness2 / ((d * d) * max(0.1h, LoH2) * brdfInfo.normalizationTerm);
                specularTerm = specularTerm - HALF_MIN;
                specularTerm = clamp(specularTerm, 0.0, 100.0);
                return specularTerm;
            }
            half3 AddLight(BRDFInfo brdfInfo,Light light,half3 normalWS,half3 viewDirectionWS){
                half NdotL = saturate(dot(normalWS, light.direction));
                half3 radiance = light.color * (light.distanceAttenuation * NdotL);
                half3 brdf = brdfInfo.diffuse;
                brdf += brdfInfo.specular * LightSpecular(brdfInfo, normalWS, light.direction, viewDirectionWS);
                return brdf*radiance;
            }

            half4 LitPassFragment(Varyings input) : SV_Target{
                half4 albedoAlpha=SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                half4 maskAlpha=SAMPLE_TEXTURE2D(_MaskMap, sampler_MaskMap, input.uv);

                SurfaceInfo surfaceInfo;
                surfaceInfo.albedo=albedoAlpha.rgb * _MainColor.rgb;
                surfaceInfo.metallic=maskAlpha.r*_Metallic;
                surfaceInfo.smoothness=maskAlpha.a*_Smoothness;
                #if defined(_NORMALMAP)
                    half4 n = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, input.uv);
                    #if BUMP_SCALE_NOT_SUPPORTED
                        surfaceInfo.normalTS=UnpackNormal(n);
                    #else
                        surfaceInfo.normalTS=UnpackNormalScale(n, _BumpScale);
                    #endif
                #else
                    surfaceInfo.normalTS=half3(0.0h, 0.0h, 1.0h);
                #endif
                
                InputInfo inputInfo=(InputInfo)0;
                inputInfo.positionWS = input.positionWS;
                inputInfo.viewDirectionWS=SafeNormalize(input.viewDirWS);
                #if defined(_NORMALMAP)
                    float sgn = input.tangentWS.w;
                    float3 bitangent = sgn * cross(input.normalWS.xyz, input.tangentWS.xyz);
                    inputInfo.normalWS = TransformTangentToWorld(surfaceInfo.normalTS, half3x3(input.tangentWS.xyz, bitangent.xyz, input.normalWS.xyz));
                #else
                    inputInfo.normalWS = input.normalWS;
                #endif
                inputInfo.normalWS = NormalizeNormalPerPixel(inputInfo.normalWS);
                inputInfo.bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, inputInfo.normalWS);

                BRDFInfo brdfInfo;
                InitializeBRDFInfo(surfaceInfo.albedo,surfaceInfo.metallic,surfaceInfo.smoothness,brdfInfo);

                half3 color=GlobalLight(brdfInfo,inputInfo.bakedGI,inputInfo.normalWS,inputInfo.viewDirectionWS);

                Light mainLight = GetMainLight();
                color+=AddLight(brdfInfo,mainLight,inputInfo.normalWS,inputInfo.viewDirectionWS);
                uint pixelLightCount = GetAdditionalLightsCount();
                for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex){
                    Light light = GetAdditionalLight(lightIndex, inputInfo.positionWS);
                    color+=AddLight(brdfInfo,light,inputInfo.normalWS,inputInfo.viewDirectionWS);
                }

                return half4(color,1);
            }
            ENDHLSL
        }
    }



    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "UnityEditor.StandardWeaponShader"
}
