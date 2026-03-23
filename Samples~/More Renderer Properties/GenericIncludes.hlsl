#include "ShaderApiReflectionSupport.hlsl"

/// <funchints>
///     <sg:ProviderKey>RSUVBitPacker.Int4</sg:ProviderKey>
///     <sg:DisplayName>RSUV Int4</sg:DisplayName>
///     <sg:SearchCategory>RSUV Bit Packer</sg:SearchCategory>
///     <sg:SearchName>RSUV Int4</sg:SearchName>
///     <sg:SearchTerms>RSUV, Int4</sg:SearchTerms>
/// </funchints>
UNITY_EXPORT_REFLECTION
int GetInt4FromRSUV(uint bitIndex)
{
    return ((unity_RendererUserValue >> bitIndex) & 0xF);
}

/// <funchints>
///     <sg:ProviderKey>RSUVBitPacker.FixedHS</sg:ProviderKey>
///     <sg:DisplayName>RSUV Color16 (HS)</sg:DisplayName>
///     <sg:SearchCategory>RSUV Bit Packer</sg:SearchCategory>
///     <sg:SearchName>RSUV Color16 (HS)</sg:SearchName>
///     <sg:SearchTerms>RSUV, Color16</sg:SearchTerms>
/// </funchints>
UNITY_EXPORT_REFLECTION
half3 GetFixed3HSFromRSUV(uint bitIndex)
{
    half3 hsv = half3(
        (unity_RendererUserValue >> bitIndex) & 0xFF,
        (unity_RendererUserValue >> bitIndex + 8) & 0xFF, 255) / 255.0;
    
    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 P = abs(frac(hsv.xxx + K.xyz) * 6.0 - K.www);
    
    return hsv.z * lerp(K.xxx, saturate(P - K.xxx), hsv.y);
}
