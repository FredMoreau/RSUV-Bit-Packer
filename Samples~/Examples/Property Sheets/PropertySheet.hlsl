#include "ShaderApiReflectionSupport.hlsl"

#ifndef RSUV
#define RSUV unity_RendererUserValue
#endif

/// <funchints>
///     <sg:ProviderKey>PropertySheet_ColorIndex</sg:ProviderKey>
/// </funchints>
UNITY_EXPORT_REFLECTION
void PropertySheet_ColorIndex(out int ColorIndex)
{
    ColorIndex = ((RSUV >> 0) & ((1 << 2) - 1));
}

/// <funchints>
///     <sg:ProviderKey>PropertySheet_TextureIndex</sg:ProviderKey>
/// </funchints>
UNITY_EXPORT_REFLECTION
void PropertySheet_TextureIndex(out int TextureIndex)
{
    TextureIndex = ((RSUV >> 2) & ((1 << 3) - 1));
}

/// <funchints>
///     <sg:ProviderKey>PropertySheet_IsMetallic</sg:ProviderKey>
/// </funchints>
UNITY_EXPORT_REFLECTION
void PropertySheet_IsMetallic(out bool IsMetallic)
{
    IsMetallic = (RSUV & (1 << 5)) != 0;
}

/// <funchints>
///     <sg:ProviderKey>PropertySheet_Smoothness</sg:ProviderKey>
/// </funchints>
UNITY_EXPORT_REFLECTION
void PropertySheet_Smoothness(out half Smoothness)
{
    Smoothness = ((RSUV >> 6) & ((1 << 8) - 1)) / 255.0 * 1 + 0;
}

/// <funchints>
///     <sg:ProviderKey>PropertySheet_Emission</sg:ProviderKey>
/// </funchints>
UNITY_EXPORT_REFLECTION
void PropertySheet_Emission(out half Emission)
{
    Emission = ((RSUV >> 14) & ((1 << 8) - 1)) / 255.0 * 1 + 0;
}

