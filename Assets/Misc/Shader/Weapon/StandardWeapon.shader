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

            #include "../CustomFunc.hlsl"
            #include "../CustomLighting.hlsl"

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
                float3 vertexSH:TEXCOORD5;
            };

            Varyings LitPassVertex(Attributes input){
                Varyings output = (Varyings)0;
                VertexPosition_WC vertexInput = GetVertexPosition_WC(input.positionOS.xyz);
                VertexNormal_TN normalInput = GetVertexNormal_TN(input.normalOS, input.tangentOS);

                output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
                output.positionCS = vertexInput.positionCS;
                output.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
                output.normalWS = normalInput.normalWS;
                output.positionWS = vertexInput.positionWS;
                #if defined(_NORMALMAP)
                real sign = input.tangentOS.w * GetOddNegativeScale();
                output.tangentWS = half4(normalInput.tangentWS.xyz, sign);
                #endif
                output.vertexSH = SampleSHVertex(output.normalWS);

                return output;
            }

            half4 LitPassFragment(Varyings input) : SV_Target{
                half4 albedoAlpha=SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                half4 maskAlpha=SAMPLE_TEXTURE2D(_MaskMap, sampler_MaskMap, input.uv);

                half3 albedo=albedoAlpha.rgb * _MainColor.rgb;
                half occlusion=1;
                half metallic=maskAlpha.r*_Metallic;
                half smoothness=maskAlpha.a*_Smoothness;

                #if defined(_NORMALMAP)
                    half4 n = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, input.uv);
                    #if BUMP_SCALE_NOT_SUPPORTED
                        half3 normalTS=UnpackNormal(n);
                    #else
                        half3 normalTS=UnpackNormalScale(n, _BumpScale);
                    #endif
                    float sgn = input.tangentWS.w;
                    float3 bitangent = sgn * cross(input.normalWS.xyz, input.tangentWS.xyz);
                    half3 normalWS = TransformTangentToWorld(normalTS, half3x3(input.tangentWS.xyz, bitangent.xyz, input.normalWS.xyz));
                #else
                    half3 normalWS = input.normalWS;
                #endif
                normalWS = NormalizeNormalPerPixel(normalWS);

                half3 positionWS = input.positionWS;
                half3 viewDirectionWS=SafeNormalize(input.viewDirWS);
                half3 bakedGI = SampleSHPixel(input.vertexSH, normalWS);

                BRDFInfo_Full brdfInfo;
                InitializeBRDFInfo_Full(albedo,metallic,smoothness,brdfInfo);

                half3 color=ApplyBRDFEnvironmentLighting(brdfInfo,bakedGI,occlusion,normalWS,viewDirectionWS);

                uint pixelLightCount = GetAdditionalLightsCount();
                for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex){
                    Light light = GetAdditionalLight(lightIndex, positionWS);
                    color+=ApplyBRDFAdditionalLighting(brdfInfo,light,normalWS,viewDirectionWS);
                }
                return half4(color,1);
            }
            ENDHLSL
        }
    }



    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "UnityEditor.StandardWeaponShader"
}
