#include "ShaderApiReflectionSupport.hlsl"

#ifndef RSUV
#define RSUV unity_RendererUserValue
#endif

namespace RSUVBitPacker_Samples
{
/// <funchints>
///     <sg:ProviderKey>RSUVBitPacker.Samples.EnumColor</sg:ProviderKey>
///     <sg:DisplayName>EnumColor</sg:DisplayName>
///     <sg:SearchCategory>Input/Property Sheets</sg:SearchCategory>
///     <sg:SearchName>EnumColor</sg:SearchName>
///     <sg:SearchTerms>RSUV, EnumColor</sg:SearchTerms>
/// </funchints>
UNITY_EXPORT_REFLECTION
void EnumColor(out float4 Color)
{
    int Index_0 = ((RSUV >> 0) & ((1 << 3) - 1));
    [branch] switch(Index_0)
    {
        case 0:
            Color = float4(0, 0.1803922, 0.2784314, 1);
            break;
        case 1:
            Color = float4(0.8117648, 0.1529412, 0.1529412, 1);
            break;
        case 2:
            Color = float4(0.937255, 0.4784314, 0, 1);
            break;
        case 3:
            Color = float4(0.9568628, 0.7254902, 0.2784314, 1);
            break;
        case 4:
            Color = float4(0.8901961, 0.8588236, 0.6941177, 1);
            break;
    }
}

}
