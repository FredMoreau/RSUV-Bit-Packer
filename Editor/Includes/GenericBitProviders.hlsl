#include "ShaderApiReflectionSupport.hlsl"

/// <funchints>
///     <sg:ProviderKey>RSUVBitPacker.Bool</sg:ProviderKey>
///     <sg:DisplayName>RSUV Bool</sg:DisplayName>
///     <sg:SearchCategory>RSUV Bit Packer</sg:SearchCategory>
///     <sg:SearchName>RSUV Bool</sg:SearchName>
///     <sg:SearchTerms>RSUV, Bool</sg:SearchTerms>
/// </funchints>
UNITY_EXPORT_REFLECTION
bool GetBoolFromRSUV(uint bitIndex)
{
    return (unity_RendererUserValue & (1 << bitIndex)) != 0;
}

/// <funchints>
///     <sg:ProviderKey>RSUVBitPacker.Fixed</sg:ProviderKey>
///     <sg:DisplayName>RSUV Float8</sg:DisplayName>
///     <sg:SearchCategory>RSUV Bit Packer</sg:SearchCategory>
///     <sg:SearchName>RSUV Float8</sg:SearchName>
///     <sg:SearchTerms>RSUV, Float8</sg:SearchTerms>
/// </funchints>
UNITY_EXPORT_REFLECTION
half GetFixedFromRSUV(uint bitIndex)
{
    return ((unity_RendererUserValue >> bitIndex) & 0xFF) / 255.0;
}

/// <funchints>
///     <sg:ProviderKey>RSUVBitPacker.Fixed3</sg:ProviderKey>
///     <sg:DisplayName>RSUV Color24</sg:DisplayName>
///     <sg:SearchCategory>RSUV Bit Packer</sg:SearchCategory>
///     <sg:SearchName>RSUV Color24</sg:SearchName>
///     <sg:SearchTerms>RSUV, Color24</sg:SearchTerms>
/// </funchints>
UNITY_EXPORT_REFLECTION
half3 GetFixed3FromRSUV(uint bitIndex)
{
    return half3((unity_RendererUserValue >> bitIndex) & 0xFF,
    (unity_RendererUserValue >> bitIndex + 8) & 0xFF,
    (unity_RendererUserValue >> bitIndex + 16) & 0xFF) / 255.0;
}
