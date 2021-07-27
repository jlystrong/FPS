Shader "FPS/FX/Blend Light"
{
    Properties
    {
        [HDR] _MainColor("Color", Color) = (1,1,1,1)
        [MainTexture] _MainTex("Albedo", 2D) = "white" {}
        _Add("Add Light", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags{"Queue"="Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit"}
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}
            Blend SrcAlpha OneMinusSrcAlpha
	        Cull Off
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "../CustomLighting.hlsl"

            TEXTURE2D(_MainTex);SAMPLER(sampler_MainTex);
            CBUFFER_START(UnityPerMaterial)
            half4 _MainColor;
            float4 _MainTex_ST;
            half4 _Add;
            CBUFFER_END

            struct appdata{
                float3 vertex : POSITION;
                float3 normal : NORMAL;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
            };

            struct v2f{
                float4 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
                half3 light : TEXCOORD1;
            };

            v2f vert (appdata v){
                v2f o;
                float3 positionWS = TransformObjectToWorld(v.vertex);
				o.vertex = TransformWorldToHClip(positionWS);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                float3 normalWS = TransformObjectToWorldNormal(v.normal);
                o.light = ApplyFlatVertexSHLighting(positionWS,normalWS);
				return o;
            }

            half4 frag (v2f i) : SV_Target{
                half4 c = i.color * _MainColor * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord) * (half4(i.light,0)+_Add);
                c.rgb*=2;
                return c;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
