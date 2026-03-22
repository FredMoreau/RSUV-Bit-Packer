void GetBoolFromRSUV_half(uint bitIndex, out bool Out)
{
    Out = (unity_RendererUserValue & (1 << bitIndex)) != 0;
}

void GetFixedFromRSUV_half(uint bitIndex, out half Out)
{
    Out = ((unity_RendererUserValue >> bitIndex) & 0xFF) / 255.0;
}

void GetFixed3FromRSUV_half(uint bitIndex, out half3 Out)
{
    Out = half3((unity_RendererUserValue >> bitIndex) & 0xFF,
    (unity_RendererUserValue >> bitIndex + 8) & 0xFF,
    (unity_RendererUserValue >> bitIndex + 16) & 0xFF) / 255.0;
}
