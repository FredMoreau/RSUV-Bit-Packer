#include "ShaderApiReflectionSupport.hlsl"

/// <funchints>
///     <sg:ProviderKey>RSUVBitPacker.Bool</sg:ProviderKey>
/// </funchints>
UNITY_EXPORT_REFLECTION
bool GetBoolFromRSUV(uint bitIndex)
{
    return (unity_RendererUserValue & (1 << bitIndex)) != 0;
}

/// <funchints>
///     <sg:ProviderKey>RSUVBitPacker.Fixed</sg:ProviderKey>
/// </funchints>
UNITY_EXPORT_REFLECTION
half GetFixedFromRSUV(uint bitIndex)
{
    return ((unity_RendererUserValue >> bitIndex) & 0xFF) / 255.0;
}

/// <funchints>
///     <sg:ProviderKey>RSUVBitPacker.Fixed3</sg:ProviderKey>
/// </funchints>
UNITY_EXPORT_REFLECTION
half3 GetFixed3FromRSUV(uint bitIndex)
{
    return half3((unity_RendererUserValue >> bitIndex) & 0xFF,
    (unity_RendererUserValue >> bitIndex + 8) & 0xFF,
    (unity_RendererUserValue >> bitIndex + 16) & 0xFF) / 255.0;
}
