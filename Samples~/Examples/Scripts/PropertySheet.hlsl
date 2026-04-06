#include "ShaderApiReflectionSupport.hlsl"

#ifndef RSUV
#define RSUV unity_RendererUserValue
#endif

namespace RSUVBitPacker_Samples
{
/// <funchints>
///     <sg:ProviderKey>RSUVBitPacker.Samples.PropertySheet.ColorIndex</sg:ProviderKey>
///     <sg:DisplayName>ColorIndex</sg:DisplayName>
///     <sg:SearchCategory>Input/Property Sheets/PropertySheet</sg:SearchCategory>
///     <sg:SearchName>ColorIndex</sg:SearchName>
///     <sg:SearchTerms>RSUV, PropertySheet, ColorIndex</sg:SearchTerms>
/// </funchints>
UNITY_EXPORT_REFLECTION
void PropertySheet_ColorIndex(out int ColorIndex)
{
    ColorIndex = ((RSUV >> 0) & ((1 << 2) - 1));
}

/// <funchints>
///     <sg:ProviderKey>RSUVBitPacker.Samples.PropertySheet.TextureIndex</sg:ProviderKey>
///     <sg:DisplayName>TextureIndex</sg:DisplayName>
///     <sg:SearchCategory>Input/Property Sheets/PropertySheet</sg:SearchCategory>
///     <sg:SearchName>TextureIndex</sg:SearchName>
///     <sg:SearchTerms>RSUV, PropertySheet, TextureIndex</sg:SearchTerms>
/// </funchints>
UNITY_EXPORT_REFLECTION
void PropertySheet_TextureIndex(out int TextureIndex)
{
    TextureIndex = ((RSUV >> 2) & ((1 << 3) - 1));
}

/// <funchints>
///     <sg:ProviderKey>RSUVBitPacker.Samples.PropertySheet.IsMetallic</sg:ProviderKey>
///     <sg:DisplayName>IsMetallic</sg:DisplayName>
///     <sg:SearchCategory>Input/Property Sheets/PropertySheet</sg:SearchCategory>
///     <sg:SearchName>IsMetallic</sg:SearchName>
///     <sg:SearchTerms>RSUV, PropertySheet, IsMetallic</sg:SearchTerms>
/// </funchints>
UNITY_EXPORT_REFLECTION
void PropertySheet_IsMetallic(out bool IsMetallic)
{
    IsMetallic = (RSUV & (1 << 5)) != 0;
}

/// <funchints>
///     <sg:ProviderKey>RSUVBitPacker.Samples.PropertySheet.Smoothness</sg:ProviderKey>
///     <sg:DisplayName>Smoothness</sg:DisplayName>
///     <sg:SearchCategory>Input/Property Sheets/PropertySheet</sg:SearchCategory>
///     <sg:SearchName>Smoothness</sg:SearchName>
///     <sg:SearchTerms>RSUV, PropertySheet, Smoothness</sg:SearchTerms>
/// </funchints>
UNITY_EXPORT_REFLECTION
void PropertySheet_Smoothness(out half Smoothness)
{
    Smoothness = ((RSUV >> 6) & ((1 << 8) - 1)) / 255.0 * 1 + 0;
}

/// <funchints>
///     <sg:ProviderKey>RSUVBitPacker.Samples.PropertySheet.Emission</sg:ProviderKey>
///     <sg:DisplayName>Emission</sg:DisplayName>
///     <sg:SearchCategory>Input/Property Sheets/PropertySheet</sg:SearchCategory>
///     <sg:SearchName>Emission</sg:SearchName>
///     <sg:SearchTerms>RSUV, PropertySheet, Emission</sg:SearchTerms>
/// </funchints>
UNITY_EXPORT_REFLECTION
void PropertySheet_Emission(out half Emission)
{
    Emission = ((RSUV >> 14) & ((1 << 8) - 1)) / 255.0 * 1 + 0;
}

}
