using System;

namespace UnityEngine.RSUVBitPacker
{
    [Serializable]
    [RendererValueTypeName("Color24 (RGB)")]
    [RendererValueTypeLength(24)]
    [RendererValueTypeTooltip("The Color24 Property stores a Color over 24 bits. (8bits per channel).")]
    public class RendererFixedRGBProperty : RendererProperty<Color>
    {
        public override uint Length => 24;

        public override uint Data
        {
            get
            {
                Color32 color = Value;
                return ((uint)color.r << 0) | ((uint)color.g << 8) | ((uint)color.b << 16);
            }
        }

        public override string HlslType => "half3";
        public override string HlslDecoder(string paramName, uint bitIndex) => @$"{paramName} = half3(
        ({rsuvDefineSymbol} >> {bitIndex}) & 0xFF,
        ({rsuvDefineSymbol} >> {bitIndex}+8) & 0xFF,
        ({rsuvDefineSymbol} >> {bitIndex}+16) & 0xFF
    ) / 255.0;";
    }
}
