#ifndef RSUV
#define RSUV unity_RendererUserValue
#endif

void PropertySheet_ColorIndex_half(out int ColorIndex)
{
    ColorIndex = ((RSUV >> 0) & ((1 << 2) - 1));
}

void PropertySheet_TextureIndex_half(out int TextureIndex)
{
    TextureIndex = ((RSUV >> 2) & ((1 << 3) - 1));
}

void PropertySheet_IsMetallic_half(out bool IsMetallic)
{
    IsMetallic = (RSUV & (1 << 5)) != 0;
}

void PropertySheet_Smoothness_half(out half Smoothness)
{
    Smoothness = ((RSUV >> 6) & ((1 << 8) - 1)) / 255.0 * 1 + 0;
}

void PropertySheet_Emission_half(out half Emission)
{
    Emission = ((RSUV >> 14) & ((1 << 8) - 1)) / 255.0 * 1 + 0;
}

