#ifndef RSUV
#define RSUV unity_RendererUserValue
#endif

void EnumColor_half(out float4 Color)
{
    int Index_0 = ((RSUV >> 0) & ((1 << 3) - 1));
    [branch] switch(Index_0)
    {
        case 0:
            Color = float4(0.4392157, 0.682353, 0.6627451, 1);
            break;
        case 1:
            Color = float4(0.7411765, 0.8666667, 0.8627452, 1);
            break;
        case 2:
            Color = float4(0.9960785, 0.9294118, 0.8196079, 1);
            break;
        case 3:
            Color = float4(0.9843138, 0.7529413, 0.6980392, 1);
            break;
        case 4:
            Color = float4(0.9921569, 0.5647059, 0.4745098, 1);
            break;
        case 5:
            Color = float4(0.8117648, 0.7254902, 0.4941177, 1);
            break;
        case 6:
            Color = float4(0.2, 0.3568628, 0.3333333, 1);
            break;
        case 7:
            Color = float4(0.01960784, 0.2, 0.1607843, 1);
            break;
    }
}

