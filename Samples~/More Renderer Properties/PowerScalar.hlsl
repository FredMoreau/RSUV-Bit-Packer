#include "ShaderApiReflectionSupport.hlsl"

#ifndef RSUV
#define RSUV unity_RendererUserValue
#endif

namespace RSUVBitPacker_Samples
{
/// <funchints>
///     <sg:ProviderKey>RSUVBitPacker.Samples.PowerScalar</sg:ProviderKey>
///     <sg:DisplayName>PowerScalar</sg:DisplayName>
///     <sg:SearchCategory>Input/Property Sheets</sg:SearchCategory>
///     <sg:SearchName>PowerScalar</sg:SearchName>
///     <sg:SearchTerms>RSUV, PowerScalar</sg:SearchTerms>
/// </funchints>
UNITY_EXPORT_REFLECTION
void PowerScalar(out half Intensity)
{
    Intensity = pow(((RSUV >> 0) & ((1 << 4) - 1)) / 15.0, 3) * 1 + 0;
}

}
