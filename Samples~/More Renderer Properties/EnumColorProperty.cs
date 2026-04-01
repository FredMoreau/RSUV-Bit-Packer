using System;
using System.Text;
using UnityEngine;
using UnityEngine.RSUVBitPacker;

namespace RSUVBitPacker.Samples
{
    [Serializable]
    [RendererValueTypeName("Enum Color")]
    [RendererValueTypeTooltip("The Enum Property allows setting a list of colors that will generate an integer.")]
    public class EnumColorProperty : RendererProperty<int, EnumColorProperty.PropertySettings>
    {
        [Serializable]
        public struct PropertySettings : IEquatable<PropertySettings>
        {
            public Color[] colors;
            public HlslPrecision hlslPrecision;

            public PropertySettings(Color[] colors)
            {
                this.colors = colors != null ? (Color[])colors.Clone() : null;
                this.hlslPrecision = HlslPrecision.Float;
            }

            public bool Equals(PropertySettings other)
            {
                if (ReferenceEquals(this.colors, other.colors))
                    return true;
                if (this.colors == null || other.colors == null)
                    return false;
                if (this.colors.Length != other.colors.Length)
                    return false;
                for (int i = 0; i < this.colors.Length; i++)
                {
                    if (this.colors[i] != other.colors[i])
                        return false;
                }
                return true;
            }
        }

        public EnumColorProperty()
        {
            Settings = new PropertySettings(new Color[] { Color.red, Color.green, Color.blue });
        }

        protected override RendererPropertyBase Clone()
        {
            var clone = base.Clone() as EnumColorProperty;
            var colorsCopy = Settings.colors != null ? (Color[])Settings.colors.Clone() : null;
            clone.Settings = new PropertySettings(colorsCopy);
            return clone;
        }

        public override uint Length => (uint)Mathf.RoundToInt(Mathf.Log((uint)Mathf.NextPowerOfTwo(Settings.colors.Length), 2));

        public override uint Data => (uint)Mathf.Clamp(Value, 0, Mathf.Pow(2, Length));

        public override string HlslType => $"{Settings.hlslPrecision.ToHlslString()}4";

        public override string HlslDecoder(string paramName, uint bitIndex)
        {
            StringBuilder hlslBody = new StringBuilder();
            hlslBody.AppendLine($"int Index = (({rsuvDefineSymbol} >> {bitIndex}) & ((1 << {Length}) - 1));");
            hlslBody.AppendLine("    [branch] switch(Index)");
            hlslBody.AppendLine("    {");
            for (int i = 0; i < Settings.colors.Length; i++)
            {
                var c = Settings.colors[i];
                hlslBody.Append(@$"        case {i}:
            {paramName} = {Settings.hlslPrecision.ToHlslString()}4({c.r}, {c.g}, {c.b}, {c.a});
            break;
");
            }
            hlslBody.Append("    }");
            return hlslBody.ToString();
        }
    }
}
