#ifndef RSUV
#define RSUV unity_RendererUserValue
#endif

void GetBoolFromRSUV_half(uint bitIndex, out bool Out)
{
    Out = (RSUV & (1 << bitIndex)) != 0;
}

void GetIntFromRSUV_half(uint bitIndex, uint length, out int Out)
{
    Out = ((RSUV >> bitIndex) & ((1 << length) - 1));
}

void GetScalarFromRSUV_half(uint bitIndex, uint length, out half Out)
{
    Out = ((RSUV >> bitIndex) & ((1 << length) - 1)) / (pow(2, length) - 1.0);
}

void GetColorRGBFromRSUV_half(uint bitIndex, out half3 Out)
{
    Out = half3((RSUV >> bitIndex) & 0xFF,
    (RSUV >> bitIndex + 8) & 0xFF,
    (RSUV >> bitIndex + 16) & 0xFF) / 255.0;
}

void GetColorHSVFromRSUV_half(uint bitIndex, uint huePrecision, uint saturationPrecision, uint valuePrecision, out half3 Out)
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
    
    Out = hsv.z * lerp(K.xxx, saturate(P - K.xxx), hsv.y);
}
