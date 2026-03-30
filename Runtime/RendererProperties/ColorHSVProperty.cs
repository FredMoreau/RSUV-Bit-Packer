using System;
using System.Text;
using UnityEngine;

namespace UnityEngine.RSUVBitPacker.RendererProperties
{
    [Serializable]
    [RendererValueTypeName("HSV")]
    [RendererValueTypeTooltip("The HSV Property allows setting a precision per HSV channel to store a color.")]
    public class ColorHSVProperty : RendererProperty<Color, ColorHSVProperty.PropertySettings>
    {
        [Serializable]
        public struct PropertySettings : IEquatable<PropertySettings>
        {
            public uint huePrecision;
            public uint saturationPrecision;
            public uint valuePrecison;

            public PropertySettings(Vector3Int precision)
            {
                this.huePrecision = (uint)precision.x;
                this.saturationPrecision = (uint)precision.y;
                this.valuePrecison = (uint)precision.z;

            }

            public uint Length => (uint)(huePrecision + saturationPrecision + valuePrecison);

            public bool Equals(PropertySettings other)
            {
                return other.huePrecision == this.huePrecision &&
                    other.saturationPrecision == this.saturationPrecision &&
                    other.valuePrecison == this.valuePrecison;
            }
        }

        public ColorHSVProperty()
        {
            Settings = new PropertySettings(new Vector3Int(8, 4, 4));
        }

        public override uint Length => Settings.Length;

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

        public override string HlslType => "half3";
        public override string HlslDecoder(string paramName, uint bitIndex)
        {
            StringBuilder hlslBody = new StringBuilder();
            hlslBody.AppendLine("half3 hsv = half3(");

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
