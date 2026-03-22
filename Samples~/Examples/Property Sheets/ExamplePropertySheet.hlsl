#include "ShaderApiReflectionSupport.hlsl"

/// <funchints>
///     <sg:ProviderKey>ExamplePropertySheet</sg:ProviderKey>
/// </funchints>
UNITY_EXPORT_REFLECTION
void ExamplePropertySheet(
    out bool OverrideColor,
    out half3 Color,
    out bool Emission)
{
    uint rsuv = unity_RendererUserValue;
    OverrideColor = (rsuv & (1 << 0)) != 0;
    Color = half3(
        (rsuv >> 1) & 0xFF,
        (rsuv >> 1+8) & 0xFF,
        (rsuv >> 1+16) & 0xFF
    ) / 255.0;
    Emission = (rsuv & (1 << 2)) != 0;
}

