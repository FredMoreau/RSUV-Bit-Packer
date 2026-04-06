#ifndef RSUV
#define RSUV unity_RendererUserValue
#endif

void PowerScalar_half(out half Intensity)
{
    Intensity = pow(((RSUV >> 0) & ((1 << 8) - 1)) / 255.0, 3) * 1 + 0;
}

