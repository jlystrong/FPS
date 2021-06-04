Shader "FPS/FX/Blend"
{
    Properties
    {
        [HDR] _MainColor("Color", Color) = (1,1,1,1)
        [MainTexture] _MainTex("Albedo", 2D) = "white" {}
    }
    SubShader
    {
        Tags{"Queue"="Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True"}
        LOD 100

        Pass
        {
            Tags{"LightMode" = "UniversalForward"}
            Blend SrcAlpha OneMinusSrcAlpha
	        Cull Off Lighting Off ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);SAMPLER(sampler_MainTex);
            CBUFFER_START(UnityPerMaterial)
            half4 _MainColor;
            float4 _MainTex_ST;
            CBUFFER_END

            struct appdata{
                float3 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
            };

            struct v2f{
                float4 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
            };

            v2f vert (appdata v){
                v2f o;
				o.vertex = TransformWorldToHClip(v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
            }

            half4 frag (v2f i) : SV_Target{
                half4 c = i.color * _MainColor * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
                c.rgb*=2;
                return c;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
