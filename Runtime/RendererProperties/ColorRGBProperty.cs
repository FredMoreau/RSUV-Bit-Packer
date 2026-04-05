using System;

namespace UnityEngine.RSUVBitPacker.RendererProperties
{
    /// <summary>
    /// Packs an RGB color into 24 bits (8 bits per channel) and provides HLSL decoding helpers.
    /// </summary>
    /// <remarks>
    /// The color's R, G, and B channels are stored as 8-bit unsigned values in the packed user value.
    /// The property decodes to <c>float3</c> in HLSL and normalizes the components to [0,1].
    /// </remarks>
    [Serializable]
    [RendererValueTypeName("Color24 (RGB)")]
    [RendererValueTypeLength(24)]
    [RendererValueTypeTooltip("The Color24 (RGB) Property stores a Color over 24 bits. (8bits per channel).")]
    public class ColorRGBProperty : RendererProperty<Color>
    {
        /// <summary>
        /// Total number of bits used to represent the RGB color (24).
        /// </summary>
        public override uint Length => 24;

        /// <summary>
        /// The packed 24-bit representation of the current color value (R | G << 8 | B << 16).
        /// </summary>
        public override uint Data
        {
            get
            {
                Color32 color = Value;
                return ((uint)color.r << 0) | ((uint)color.g << 8) | ((uint)color.b << 16);
            }
        }

        /// <summary>
        /// Corresponding HLSL type for the decoded color (3-component float type using HlslPrecision.Float).
        /// </summary>
        public override string HlslType => HlslPrecision.Float.ToHlslString() + "3";

        /// <summary>
        /// Returns HLSL code that decodes the packed 24-bit RGB color into a normalized <c>float3</c>.
        /// </summary>
        /// <param name="paramName">Target HLSL variable.</param>
        /// <param name="bitIndex">Starting bit index within the packed user value.</param>
        public override string HlslDecoder(string paramName, uint bitIndex) => @$"{paramName} = {HlslPrecision.Float.ToHlslString()}3(
        ({rsuvDefineSymbol} >> {bitIndex}) & 0xFF,
        ({rsuvDefineSymbol} >> {bitIndex}+8) & 0xFF,
        ({rsuvDefineSymbol} >> {bitIndex}+16) & 0xFF
    ) / 255.0;";
    }
}
