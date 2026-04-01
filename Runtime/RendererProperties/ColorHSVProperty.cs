using System;
using System.Text;

namespace UnityEngine.RSUVBitPacker.RendererProperties
{
    /// <summary>
    /// Packs a color in HSV space using configurable bit precision per channel (Hue, Saturation, Value).
    /// </summary>
    /// <remarks>
    /// Each HSV channel is quantized according to the corresponding precision in <see cref="PropertySettings"/>.
    /// The packed data concatenates the quantized H, S and V bits into a single unsigned integer.
    /// The class also produces HLSL helper code to decode and convert HSV back to RGB in shaders.
    /// Note: Do not derive directly from <c>RendererPropertyBase</c>. Use <c>RendererProperty&lt;T&gt;</c> or
    /// <c>RendererProperty&lt;T,U&gt;</c> for concrete implementations.
    /// </remarks>
    [Serializable]
    [RendererValueTypeName("Color (HSV)")]
    [RendererValueTypeTooltip("The Color (HSV) Property allows setting a precision per HSV channel to store a color.")]
    public class ColorHSVProperty : RendererProperty<Color, ColorHSVProperty.PropertySettings>
    {
        [Serializable]
        /// <summary>
        /// Settings controlling the number of bits allocated to each HSV channel.
        /// </summary>
        public struct PropertySettings : IEquatable<PropertySettings>
        {
            /// <summary>
            /// Bits used to quantize the Hue channel.
            /// </summary>
            [Tooltip("Bits used to quantize the Hue channel.")]
            public uint huePrecision;
            /// <summary>
            /// Bits used to quantize the Saturation channel.
            /// </summary>
            [Tooltip("Bits used to quantize the Saturation channel.")]
            public uint saturationPrecision;
            /// <summary>
            /// Bits used to quantize the Value channel.
            /// </summary>
            [Tooltip("Bits used to quantize the Value channel.")]
            public uint valuePrecison;

            /// <summary>
            /// HLSL precision/type to use for the decoded color. Default is Half.
            /// </summary>
            [Tooltip("Shader precision to use when decoding the color (Half or Float).")]
            public HlslPrecision hlslPrecision;

            /// <summary>
            /// Initialize settings from a Vector3Int containing precisions for H, S and V respectively.
            /// </summary>
            /// <param name="precision">X = hue bits, Y = saturation bits, Z = value bits.</param>
            public PropertySettings(Vector3Int precision)
            {
                this.huePrecision = (uint)precision.x;
                this.saturationPrecision = (uint)precision.y;
                this.valuePrecison = (uint)precision.z;
                this.hlslPrecision = HlslPrecision.Half;
            }

            /// <summary>
            /// Total number of bits used to store the concatenated HSV channels.
            /// </summary>
            public uint Length => (uint)(huePrecision + saturationPrecision + valuePrecison);

            /// <summary>
            /// Compare two settings instances for equality.
            /// </summary>
            public bool Equals(PropertySettings other)
            {
                return other.huePrecision == this.huePrecision &&
                    other.saturationPrecision == this.saturationPrecision &&
                    other.valuePrecison == this.valuePrecison &&
                    other.hlslPrecision == this.hlslPrecision;
            }
        }

        /// <summary>
        /// Default constructor initializing HSV precision to (8,4,4) for H,S,V respectively.
        /// </summary>
        public ColorHSVProperty()
        {
            Settings = new PropertySettings(new Vector3Int(8, 4, 4));
        }

        /// <summary>
        /// Total bit length used by this color property (sum of H, S and V precisions).
        /// </summary>
        public override uint Length => Settings.Length;

        /// <summary>
        /// Packed HSV representation combining the quantized H, S and V fields.
        /// </summary>
        public override uint Data
        {
            get
            {
                Color.RGBToHSV(Value, out float h, out float s, out float v);
                h = Mathf.Clamp(h, 0f, 1f);
                s = Mathf.Clamp(s, 0f, 1f);
                v = Mathf.Clamp(v, 0f, 1f);

                uint rsuv = 0;
                if (Settings.huePrecision != 0)
                {
                    uint hi = (uint)Mathf.Round(h * (Mathf.Pow(2, Settings.huePrecision) - 1f));
                    rsuv |= hi << 0;
                }
                if (Settings.saturationPrecision != 0)
                {
                    uint si = (uint)Mathf.Round(s * (Mathf.Pow(2, Settings.saturationPrecision) - 1f));
                    rsuv |= si << (int)Settings.huePrecision;
                }
                if (Settings.valuePrecison != 0)
                {
                    uint vi = (uint)Mathf.Round(v * (Mathf.Pow(2, Settings.valuePrecison) - 1f));
                    rsuv |= vi << (int)(Settings.huePrecision + Settings.saturationPrecision);
                }

                return rsuv;
            }
        }

        /// <summary>
        /// Corresponding HLSL type for the decoded color; e.g. "half3" or "float3" based on settings.
        /// </summary>
        public override string HlslType => Settings.hlslPrecision.ToHlslString() + "3";

        /// <summary>
        /// Returns HLSL code that decodes the packed HSV value starting at <paramref name="bitIndex"/>,
        /// converts it to RGB and assigns it to <paramref name="paramName"/>.
        /// </summary>
        /// <param name="paramName">Target HLSL variable.</param>
        /// <param name="bitIndex">Starting bit index within the packed user value.</param>
        public override string HlslDecoder(string paramName, uint bitIndex)
        {
            StringBuilder hlslBody = new StringBuilder();
            var type = Settings.hlslPrecision.ToHlslString() + "3";
            hlslBody.AppendLine(type + " hsv = " + type + "(");

            if (Settings.huePrecision == 0)
                hlslBody.AppendLine("        0,");
            else
                hlslBody.AppendLine($"        (({rsuvDefineSymbol} >> {bitIndex}) & ((1 << {Settings.huePrecision}) - 1)) / {Mathf.Pow(2, Settings.huePrecision) - 1}.0,");

            if (Settings.saturationPrecision == 0)
                hlslBody.AppendLine("        1,");
            else
                hlslBody.AppendLine($"        (({rsuvDefineSymbol} >> {bitIndex + Settings.huePrecision}) & ((1 << {Settings.saturationPrecision}) - 1)) / {Mathf.Pow(2, Settings.saturationPrecision) - 1f}.0,");

            if (Settings.valuePrecison == 0)
                hlslBody.AppendLine("        1);");
            else
                hlslBody.AppendLine($"        (({rsuvDefineSymbol} >> {bitIndex + (int)Settings.huePrecision + Settings.saturationPrecision}) & ((1 << {Settings.valuePrecison}) - 1)) / {Mathf.Pow(2, Settings.valuePrecison) - 1f}.0);");

            hlslBody.Append($@"    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 P = abs(frac(hsv.xxx + K.xyz) * 6.0 - K.www);
    {paramName} = hsv.z * lerp(K.xxx, saturate(P - K.xxx), hsv.y);");

            return hlslBody.ToString();
        }
    }
}
