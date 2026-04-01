#include "ShaderApiReflectionSupport.hlsl"

#ifndef RSUV
#define RSUV unity_RendererUserValue
#endif

/// <funchints>
///     <sg:ProviderKey>EnumColor</sg:ProviderKey>
/// </funchints>
UNITY_EXPORT_REFLECTION
void EnumColor(out float4 Color)
{
    int Index = ((RSUV >> 0) & ((1 << 2) - 1));
    [branch] switch(Index)
    {
        case 0:
            Color = float4(1, 0, 0, 1);
            break;
        case 1:
            Color = float4(0, 1, 0, 1);
            break;
        case 2:
            Color = float4(0, 0, 1, 1);
            break;
    }
}

