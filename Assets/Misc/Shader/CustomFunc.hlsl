#ifndef CUSTOM_FUNC_INCLUDED
#define CUSTOM_FUNC_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

struct VertexPosition_WC
{
    float3 positionWS; // World space position
    float4 positionCS; // Homogeneous clip space position
};
VertexPosition_WC GetVertexPosition_WC(float3 positionOS)
{
    VertexPosition_WC input;
    input.positionWS = TransformObjectToWorld(positionOS);
    input.positionCS = TransformWorldToHClip(input.positionWS);
    return input;
}
struct VertexPosition_WVC
{
    float3 positionWS; // World space position
    float3 positionVS; // View space position
    float4 positionCS; // Homogeneous clip space position
};
VertexPosition_WVC GetVertexPosition_WVC(float3 positionOS)
{
    VertexPosition_WVC input;
    input.positionWS = TransformObjectToWorld(positionOS);
    input.positionVS = TransformWorldToView(input.positionWS);
    input.positionCS = TransformWorldToHClip(input.positionWS);
    return input;
}

struct VertexNormal_TN
{
    real3 tangentWS;
    float3 normalWS;
};
VertexNormal_TN GetVertexNormal_TN(float3 normalOS, float4 tangentOS)
{
    VertexNormal_TN input;
    // mikkts space compliant. only normalize when extracting normal at frag.
    // real sign = tangentOS.w * GetOddNegativeScale();
    input.normalWS = TransformObjectToWorldNormal(normalOS);
    input.tangentWS = TransformObjectToWorldDir(tangentOS.xyz);
    // input.bitangentWS = cross(input.normalWS, input.tangentWS) * sign;
    return input;
}


#endif