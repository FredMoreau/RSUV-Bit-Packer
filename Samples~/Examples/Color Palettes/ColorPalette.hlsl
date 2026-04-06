void ColorPalette_half(
    uint Index,
    out float4 Color)
{
    [branch] switch(Index)
    {
        case 0:
            Color = float4(0.28975, 0.4718495, 0.61, 0);
            break;
        case 1:
            Color = float4(0.5499648, 0.442416, 0.624, 0);
            break;
        case 2:
            Color = float4(0.6981132, 0.4696827, 0.2930758, 0);
            break;
        case 3:
            Color = float4(0.7075472, 0.2369616, 0.2458137, 0);
            break;
        default:
            Color = float4(0,0,0,1);
            break;
    }
}

