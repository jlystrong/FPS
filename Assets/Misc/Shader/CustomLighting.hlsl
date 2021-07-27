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


#endif