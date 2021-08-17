#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

half3 ApplyFlatVertexSHLighting(half3 positionWS,half3 normalWS){
    half3 color=SampleSHVertex(normalWS);
    uint lightsCount = GetAdditionalLightsCount();
    for (uint lightIndex = 0u; lightIndex < lightsCount; ++lightIndex){
        Light light = GetAdditionalLight(lightIndex, positionWS);
        color += light.color * light.distanceAttenuation;
    }
    return color;
}

half3 ApplyVertexSHLighting(half3 positionWS,half3 normalWS){
    half3 color=SampleSHVertex(normalWS);
    uint lightsCount = GetAdditionalLightsCount();
    for (uint lightIndex = 0u; lightIndex < lightsCount; ++lightIndex){
        Light light = GetAdditionalLight(lightIndex, positionWS);
        half3 lightColor = light.color * light.distanceAttenuation;
        color += LightingLambert(lightColor,light.direction,normalWS);
    }
    return color;
}


struct BRDFInfo_Full{
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
inline void InitializeBRDFInfo_Full(half3 albedo, half metallic, half smoothness, out BRDFInfo_Full outBRDFInfo){
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

// half3 ApplyGlossyEnvironmentReflection(half3 reflectVector, half perceptualRoughness, half occlusion){
//     #if !defined(_ENVIRONMENTREFLECTIONS_OFF)
//     half mip = PerceptualRoughnessToMipmapLevel(perceptualRoughness);
//     half4 encodedIrradiance = SAMPLE_TEXTURECUBE_LOD(unity_SpecCube0, samplerunity_SpecCube0, reflectVector, mip);
//     #if defined(UNITY_USE_NATIVE_HDR) || defined(UNITY_DOTS_INSTANCING_ENABLED)
//         half3 irradiance = encodedIrradiance.rgb;
//     #else
//         half3 irradiance = DecodeHDREnvironment(encodedIrradiance, unity_SpecCube0_HDR);
//     #endif
//     #if UNITY_SPECCUBE_BLENDING
//     float interpolator = unity_SpecCube0_BoxMin.w;
//     if(interpolator < 0.99999){
//         encodedIrradiance = SAMPLE_TEXTURECUBE_LOD(UNITY_PASS_TEXCUBE_SAMPLER(unity_SpecCube1, unity_SpecCube0), samplerunity_SpecCube1, reflectVector, mip);
//         #if defined(UNITY_USE_NATIVE_HDR) || defined(UNITY_DOTS_INSTANCING_ENABLED)
//             half3 irradiance2 = encodedIrradiance.rgb;
//         #else
//             half3 irradiance2 = DecodeHDREnvironment(encodedIrradiance, unity_SpecCube1_HDR);
//         #endif
//         irradiance=lerp(irradiance2,irradiance,interpolator);
//     }
//     #endif
//     return irradiance * occlusion;
//     #endif
//     return _GlossyEnvironmentColor.rgb * occlusion;
// }
half3 ApplyBRDFEnvironmentLighting(BRDFInfo_Full brdfInfo,half3 bakedGI, half occlusion, half3 normalWS, half3 viewDirectionWS){
    half3 reflectVector = reflect(-viewDirectionWS, normalWS);
    half NoV = saturate(dot(normalWS, viewDirectionWS));
    half fresnelTerm = Pow4(1.0 - NoV);
    half3 indirectDiffuse = bakedGI * occlusion;
    half3 indirectSpecular = GlossyEnvironmentReflection(reflectVector, brdfInfo.perceptualRoughness, occlusion);
    half3 color = EnvironmentBRDF(brdfInfo, indirectDiffuse, indirectSpecular, fresnelTerm);
    return color;
}
half3 ApplyBRDFAdditionalLighting(BRDFInfo_Full brdfInfo,Light light,half3 normalWS,half3 viewDirectionWS){
    half NdotL = saturate(dot(normalWS, light.direction));
    half3 radiance = light.color * (light.distanceAttenuation * NdotL);
    half3 brdf = brdfInfo.diffuse;

    float3 halfDir = SafeNormalize(float3(light.direction) + float3(viewDirectionWS));
    float NoH = saturate(dot(normalWS, halfDir));
    half LoH = saturate(dot(light.direction, halfDir));
    float d = NoH * NoH * brdfInfo.roughness2MinusOne + 1.00001f;
    half LoH2 = LoH * LoH;
    half specularTerm = brdfInfo.roughness2 / ((d * d) * max(0.1h, LoH2) * brdfInfo.normalizationTerm);
    specularTerm = specularTerm - HALF_MIN;
    specularTerm = clamp(specularTerm, 0.0, 100.0);
    brdf += brdfInfo.specular*specularTerm;
    
    return brdf*radiance;
}


#endif