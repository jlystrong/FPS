Shader "FPS/FX/DistortionBump"
{
    Properties
    {
        _NoiseTex ("Noise Texture (RG)", 2D) = "white" {}
        _Mask ("Mask Texture (R)", 2D) = "white" {}
		_HeatTime  ("Heat Time", range (0,1)) = 0.1
		_HeatForce  ("Heat Force", range (0,0.1)) = 0.008
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

            TEXTURE2D(_NoiseTex);SAMPLER(sampler_NoiseTex);
            TEXTURE2D(_Mask);SAMPLER(sampler_Mask);
            SAMPLER(_CameraOpaqueTexture);
            CBUFFER_START(UnityPerMaterial)
            float4 _NoiseTex_ST;
            float4 _Mask_ST;
            float _HeatTime;
            float _HeatForce;
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
                float2 texcoord2 : TEXCOORD1;
                float4 uvgrab : TEXCOORD2;
            };

            v2f vert (appdata v){
                v2f o=(v2f)0;
				o.vertex = TransformWorldToHClip(v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_NoiseTex);
                o.texcoord2 = TRANSFORM_TEX(v.texcoord,_Mask);
				return o;
            }

            half4 frag (v2f i) : SV_Target{
                half4 offsetColor1 = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, i.texcoord+_Time.xz*_HeatTime);
				half4 offsetColor2 = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, i.texcoord+_Time.yx*_HeatTime);
				half distortX = ((offsetColor1.r + offsetColor2.r) - 1) * _HeatForce;
				half distorty = ((offsetColor1.g + offsetColor2.g) - 1) * _HeatForce;
                half mask = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord2).a;
				half2 screenUV = (i.vertex.xy / _ScreenParams.xy)+ float2(distortX, distorty)*mask;
				half4 col = tex2D(_CameraOpaqueTexture, screenUV)*i.color;
                col.a=mask*i.color.a;
                return col;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
