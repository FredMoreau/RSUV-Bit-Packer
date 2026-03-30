#include "ShaderApiReflectionSupport.hlsl"

#ifndef RSUV
#define RSUV unity_RendererUserValue
#endif

/// <funchints>
///     <sg:ProviderKey>RSUVBitPacker.Bool</sg:ProviderKey>
///     <sg:DisplayName>RSUV Boolean</sg:DisplayName>
///     <sg:SearchCategory>RSUV Bit Packer</sg:SearchCategory>
///     <sg:SearchName>RSUV Bool</sg:SearchName>
///     <sg:SearchTerms>RSUV, Bool, Boolean</sg:SearchTerms>
/// </funchints>
UNITY_EXPORT_REFLECTION
bool GetBoolFromRSUV(uint bitIndex)
{
    return (RSUV & (1 << bitIndex)) != 0;
}

/// <funchints>
///     <sg:ProviderKey>RSUVBitPacker.Integer</sg:ProviderKey>
///     <sg:DisplayName>RSUV Integer</sg:DisplayName>
///     <sg:SearchCategory>RSUV Bit Packer</sg:SearchCategory>
///     <sg:SearchName>RSUV Int</sg:SearchName>
///     <sg:SearchTerms>RSUV, Int, Integer</sg:SearchTerms>
/// </funchints>
///<paramhints name = "length">
///    <sg:Default>8</sg:Default>
///</paramhints>
UNITY_EXPORT_REFLECTION
int GetIntFromRSUV(uint bitIndex, int length)
{
    return ((RSUV >> bitIndex) & ((1 << length) -1));
}

/// <funchints>
///     <sg:ProviderKey>RSUVBitPacker.Scalar</sg:ProviderKey>
///     <sg:DisplayName>RSUV Scalar</sg:DisplayName>
///     <sg:SearchCategory>RSUV Bit Packer</sg:SearchCategory>
///     <sg:SearchName>RSUV Scalar</sg:SearchName>
///     <sg:SearchTerms>RSUV, Scalar</sg:SearchTerms>
/// </funchints>
///<paramhints name = "length">
///    <sg:Default>8</sg:Default>
///</paramhints>
UNITY_EXPORT_REFLECTION
int GetScalarFromRSUV(uint bitIndex, int length)
{
    return ((RSUV >> bitIndex) & ((1 << length) - 1)) / (pow(2, length) - 1.0);
}

/// <funchints>
///     <sg:ProviderKey>RSUVBitPacker.RGB</sg:ProviderKey>
///     <sg:DisplayName>RSUV ColorRGB</sg:DisplayName>
///     <sg:SearchCategory>RSUV Bit Packer</sg:SearchCategory>
///     <sg:SearchName>RSUV Color(RGB)</sg:SearchName>
///     <sg:SearchTerms>RSUV, Color, RGB</sg:SearchTerms>
/// </funchints>
UNITY_EXPORT_REFLECTION
half3 GetColorRGBFromRSUV(uint bitIndex)
{
    return half3((RSUV >> bitIndex) & 0xFF,
    (RSUV >> bitIndex + 8) & 0xFF,
    (RSUV >> bitIndex + 16) & 0xFF) / 255.0;
}

/// <funchints>
///     <sg:ProviderKey>RSUVBitPacker.HSV</sg:ProviderKey>
///     <sg:DisplayName>RSUV ColorHSV</sg:DisplayName>
///     <sg:SearchCategory>RSUV Bit Packer</sg:SearchCategory>
///     <sg:SearchName>RSUV Color(HSV)</sg:SearchName>
///     <sg:SearchTerms>RSUV, Color, HSV</sg:SearchTerms>
/// </funchints>
///<paramhints name = "huePrecision">
///    <sg:Default>8</sg:Default>
///</paramhints>
///<paramhints name = "saturationPrecision">
///    <sg:Default>4</sg:Default>
///</paramhints>
///<paramhints name = "valuePrecision">
///    <sg:Default>4</sg:Default>
///</paramhints>
UNITY_EXPORT_REFLECTION
half3 GetColorHSVFromRSUV(uint bitIndex, uint huePrecision, uint saturationPrecision, uint valuePrecision)
{
    half3 hsv;
    
    if (huePrecision == 0)
        hsv.x = 0;
    else
        hsv.x = ((RSUV >> 0) & ((1 << huePrecision) - 1)) / (pow(2, huePrecision) - 1.0);
    
    if (saturationPrecision == 0)
        hsv.y = 1;
    else
        hsv.y = ((RSUV >> huePrecision) & ((1 << saturationPrecision) - 1)) / (pow(2, saturationPrecision) - 1.0);
    
    if (valuePrecision == 0)
        hsv.z = 1;
    else
        hsv.z = ((RSUV >> (huePrecision + saturationPrecision)) & ((1 << valuePrecision) - 1)) / (pow(2, valuePrecision) - 1.0);
    
    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 P = abs(frac(hsv.xxx + K.xyz) * 6.0 - K.www);
    
    return hsv.z * lerp(K.xxx, saturate(P - K.xxx), hsv.y);
}
