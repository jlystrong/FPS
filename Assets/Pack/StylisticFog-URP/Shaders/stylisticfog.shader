﻿Shader "Custom/RenderFeature/StylisticFog"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}

	CGINCLUDE
	#include "UnityCG.cginc"

		#define SKYBOX_THREASHOLD_VALUE 0.9999
		#define FOG_AMOUNT_CONTRIBUTION_THREASHOLD 0.0001

		bool _ApplyDistToSkybox;
		bool _ApplyHeightToSkybox;

		float4 _MainTex_TexelSize;
		UNITY_DECLARE_SCREENSPACE_TEXTURE( _MainTex);
		UNITY_DECLARE_SCREENSPACE_TEXTURE( _CameraDepthTexture);

		float4x4 _InverseViewMatrix;

		uniform float4 _DistanceColor;
		uniform float _FogStartDistance;
		uniform float _FogEndDistance;
		uniform float _FogDistanceType;

		uniform float4 _HeightColor;
		uniform float _Height;
		uniform float _BaseDensity;
		uniform float _DensityFalloff;
		float4 _MainTex_ST;

		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f_multitex
		{
			float4 pos : SV_POSITION;
			float2 uv0 : TEXCOORD0;
			float2 uv1 : TEXCOORD1;
		};

		v2f_multitex vert_img_fog(appdata_img v)
		{
			v2f_multitex o;

			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(v);

			// Handles vertically-flipped case.
			float vflip = sign(_MainTex_TexelSize.y);

			o.pos = UnityObjectToClipPos(v.vertex);
			float2 uv = TRANSFORM_TEX(v.texcoord, _MainTex);
			//float2 uv = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
			o.uv0 = uv.xy;
			o.uv1 = (uv.xy - 0.5) * float2(1, vflip) + 0.5;
			return o;
		}

		// from https://github.com/keijiro/DepthToWorldPos
		inline float4 DepthToWorld(float depth, float2 uv, float4x4 inverseViewMatrix)
		{
			float viewDepth = LinearEyeDepth(depth);
			float2 p11_22 = float2(unity_CameraProjection._11, unity_CameraProjection._22);
			float3 vpos = float3((uv * 2 - 1) / p11_22, -1) * viewDepth;
			float4 wpos = mul(inverseViewMatrix, float4(vpos, 1));
			return wpos;
		}

		// Compute how intense the distance fog is according to the distance
		// and the fog intensity curve.
		inline float ComputeDistanceFogAmount(float distance,float distanceType)
		{
			float f = saturate((distance-_FogStartDistance) / _FogEndDistance) ;
			if(distanceType<0.5)//Linear
				return f;
			else if(distanceType<1.5)//EXP
				return 2-saturate(exp2(-f))*2;
			else
				return 2-saturate(exp2(-f*f))*2;
		}

		// Computes the amount of fog treversed based on a desnity function d(h)
		// where d(h) = _BaseDensity * exp2(-DensityFalloff * h) <=> d(h) = a * exp2(b * h)
		inline float ComputeHeightFogAmount(float viewDirY, float effectiveDistance)
		{
			float relativeHeight = _WorldSpaceCameraPos.y - _Height;
			return _BaseDensity * (exp2(-relativeHeight * _DensityFalloff) - exp2((-effectiveDistance * viewDirY - relativeHeight) * _DensityFalloff)) / viewDirY;
		}

		// Not used yet, but might be useful for pass seperation.
		inline half4 BlendFogToScene(float2 uv, half4 fogColor, float fogAmount)
		{
			// clamp the scene color to at most 1. to avoid HDR rendering to change lumiance in final image.
			// half4 sceneColor = min(1., tex2D(_MainTex, uv));
			half4 sceneColor = tex2D(_MainTex, uv);
			half4 blended = lerp(sceneColor, half4(fogColor.xyz, 1.), fogColor.a * step(FOG_AMOUNT_CONTRIBUTION_THREASHOLD, fogAmount));
			blended.a = 1.;
			return blended;
		}

		half4 fragment_distance(v2f_multitex i) : SV_Target
		{
			float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv1);
			float4 wpos = DepthToWorld(depth, i.uv1, _InverseViewMatrix);
			float4 cameraToFragment = wpos - float4(_WorldSpaceCameraPos, 1.);
			float totalDistance = length(cameraToFragment);
			float linDepth = Linear01Depth(depth);
			float distanceFogAmount = 0.;

			if (_ApplyDistToSkybox)
				distanceFogAmount = ComputeDistanceFogAmount(totalDistance,_FogDistanceType);
			else if (linDepth < SKYBOX_THREASHOLD_VALUE)
				distanceFogAmount = ComputeDistanceFogAmount(totalDistance,_FogDistanceType);

			half4 fogColor = half4(_DistanceColor.rgb,distanceFogAmount*_DistanceColor.a);
			// half4 fogColor = GetColorFromTexture(_FogColorTexture0, distanceFogAmount);

			return BlendFogToScene(i.uv0, fogColor, fogColor.a);
		}

		half4 fragment_height(v2f_multitex i) : SV_Target
		{
			float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv1);
			float4 wpos = DepthToWorld(depth, i.uv1, _InverseViewMatrix);
			float4 cameraToFragment = wpos - float4(_WorldSpaceCameraPos, 1.);
			float viewDirY = normalize(cameraToFragment).y;
			float totalDistance = length(cameraToFragment);
			float linDepth = Linear01Depth(depth);
			float heightFogAmount = 0.;

			if (_ApplyHeightToSkybox)
				heightFogAmount = ComputeHeightFogAmount(viewDirY, totalDistance);
			else if (linDepth < SKYBOX_THREASHOLD_VALUE)
				heightFogAmount = ComputeHeightFogAmount(viewDirY, totalDistance);

			half4 fogColor=half4(_HeightColor.rgb,_HeightColor.a*heightFogAmount);
			return BlendFogToScene(i.uv0, fogColor, heightFogAmount);
		}

		half4 fragment_distance_height(v2f_multitex i) : SV_Target
		{
			float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv1);
			float4 wpos = DepthToWorld(depth, i.uv1, _InverseViewMatrix);
			float4 cameraToFragment = wpos - float4(_WorldSpaceCameraPos, 1.);
			float3 viewDir = normalize(cameraToFragment);
			float totalDistance = length(cameraToFragment);
			float linDepth = Linear01Depth(depth);
			float distanceFogAmount = 0.;
			float heightFogAmount = 0.;

			if (_ApplyDistToSkybox)
				distanceFogAmount = ComputeDistanceFogAmount(totalDistance,_FogDistanceType);
			else if (linDepth < SKYBOX_THREASHOLD_VALUE)
				distanceFogAmount = ComputeDistanceFogAmount(totalDistance,_FogDistanceType);

			if (_ApplyHeightToSkybox)
				heightFogAmount = ComputeHeightFogAmount(viewDir.y, totalDistance);
			else if (linDepth < SKYBOX_THREASHOLD_VALUE)
				heightFogAmount = ComputeHeightFogAmount(viewDir.y, totalDistance);

			float totalFogAmount = distanceFogAmount + heightFogAmount;
			// half4 fogColor=(_DistanceColor*distanceFogAmount+_HeightColor*heightFogAmount)/totalFogAmount;
			half4 fogColor=lerp(_HeightColor,_DistanceColor,saturate(distanceFogAmount/totalFogAmount));
			fogColor.a = fogColor.a*totalFogAmount;

			return BlendFogToScene(i.uv0, fogColor, totalFogAmount);
		}

		ENDCG
		SubShader
		{
			// 0: Distance fog only
			Pass
			{
				Cull Off ZWrite Off ZTest Always
				CGPROGRAM
				#pragma vertex vert_img_fog
				#pragma fragment fragment_distance
				ENDCG
			}

			// 1: Height fog only
			Pass
			{
				Cull Off ZWrite Off ZTest Always
				CGPROGRAM
				#pragma vertex vert_img_fog
				#pragma fragment fragment_height
				ENDCG
			}
			
			// 2: Distance and height fog using same color source
			Pass
			{
				Cull Off ZWrite Off ZTest Always
				CGPROGRAM
				#pragma vertex vert_img_fog
				#pragma fragment fragment_distance_height
				ENDCG
			}
		}
}
