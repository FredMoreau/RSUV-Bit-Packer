using System;
using System.Text;
using UnityEngine;

namespace UnityEngine.RSUVBitPacker
{
    [Serializable]
    [RendererValueTypeName("HSV")]
    [RendererValueTypeTooltip("The HSV Property allows setting a precision per HSV channel to store a color.")]
    public class RendererHSVProperty : RendererProperty<Color, RendererHSVProperty.PropertySettings>
    {
        [Serializable]
        public struct PropertySettings : IEquatable<PropertySettings>
        {
            public Vector3Int precision3;

            public PropertySettings(Vector3Int length)
            {
                this.precision3 = length;
            }

            public uint Length => (uint)(precision3.x + precision3.y + precision3.z);

            public bool Equals(PropertySettings other)
            {
                return other.precision3 == this.precision3;
            }
        }

        public RendererHSVProperty()
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
                if (Settings.precision3.x != 0)
                {
                    uint hi = (uint)Mathf.Round(h * (Mathf.Pow(2, Settings.precision3.x) - 1f));
                    rsuv |= hi << 0;
                }
                if (Settings.precision3.y != 0)
                {
                    uint si = (uint)Mathf.Round(s * (Mathf.Pow(2, Settings.precision3.y) - 1f));
                    rsuv |= si << Settings.precision3.x;
                }
                if (Settings.precision3.z != 0)
                {
                    uint vi = (uint)Mathf.Round(v * (Mathf.Pow(2, Settings.precision3.z) - 1f));
                    rsuv |= vi << (Settings.precision3.x + Settings.precision3.y);
                }

                return rsuv;
            }
        }

        public override string HlslType => "half3";
        public override string HlslDecoder(string paramName, uint bitIndex)
        {
            StringBuilder hlslBody = new StringBuilder();
            hlslBody.AppendLine("half3 hsv = half3(");

            if (Settings.precision3.x == 0)
                hlslBody.AppendLine("        0,");
            else
                hlslBody.AppendLine($"        (({rsuvDefineSymbol} >> {bitIndex}) & ((1 << {Settings.precision3.x}) - 1)) / {Mathf.Pow(2, Settings.precision3.x) - 1}.0,");

            if (Settings.precision3.y == 0)
                hlslBody.AppendLine("        1,");
            else
                hlslBody.AppendLine($"        (({rsuvDefineSymbol} >> {bitIndex + Settings.precision3.x}) & ((1 << {Settings.precision3.y}) - 1)) / {Mathf.Pow(2, Settings.precision3.y) - 1f}.0,");

            if (Settings.precision3.z == 0)
                hlslBody.AppendLine("        1);");
            else
                hlslBody.AppendLine($"        (({rsuvDefineSymbol} >> {bitIndex + Settings.precision3.x + Settings.precision3.y}) & ((1 << {Settings.precision3.z}) - 1)) / {Mathf.Pow(2, Settings.precision3.z) - 1f}.0);");

            hlslBody.Append($@"    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 P = abs(frac(hsv.xxx + K.xyz) * 6.0 - K.www);
    {paramName} = hsv.z * lerp(K.xxx, saturate(P - K.xxx), hsv.y);");

            return hlslBody.ToString();
        }
    }
}
