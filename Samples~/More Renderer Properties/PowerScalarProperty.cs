using System;
using UnityEngine;
using UnityEngine.RSUVBitPacker;

namespace RSUVBitPacker.Samples
{
    [Serializable]
    [RendererValueTypeName("Power Scalar")]
    [RendererValueTypeTooltip("The Power Scalar Property allows setting a precision (1 to 32 bits), a min and max value, and a power so as to bias precision to lower or higher values.")]
    public class PowerScalarProperty : RendererProperty<float, PowerScalarProperty.PropertySettings>
    {
        [Serializable]
        public struct PropertySettings : IEquatable<PropertySettings>
        {
            [Tooltip("Number of bits used to quantize the value.")]
            public uint precision;

            [Tooltip("Minimum value of the remap range.")]
            public float minValue;

            [Tooltip("Maximum value of the remap range.")]
            public float maxValue;

            [Tooltip("Power applied to the scalar value.\nHigher than 1 improves precision on low range.\nLower than 1 improves precision on high range.")]
            [Range(0.01f, 10f)]
            public float power;

            [Tooltip("Shader precision to use when decoding (Half or Float).")]
            public HlslPrecision hlslPrecision;

            public PropertySettings(uint length, float minValue, float maxValue, float power)
            {
                this.precision = length;
                this.minValue = minValue;
                this.maxValue = maxValue;
                this.power = power;
                this.hlslPrecision = HlslPrecision.Half;
            }

            public uint Length => precision;

            public bool Equals(PropertySettings other)
            {
                return other.precision == this.precision &&
                    other.minValue == this.minValue &&
                    other.maxValue == this.maxValue &&
                    other.power == this.power &&
                    other.hlslPrecision == this.hlslPrecision;
            }
        }

        public PowerScalarProperty()
        {
            Settings = new PropertySettings(8, 0, 1, 3);
        }

        public override uint Length => Settings.Length;

        public override uint Data
        {
            get
            {
                uint rsuv = 0;
                if (Settings.precision != 0)
                {
                    Value = Mathf.Clamp(Value, Settings.minValue, Settings.maxValue);
                    var remap = (Value + (-Settings.minValue)) / (Settings.maxValue - Settings.minValue);
                    remap = Mathf.Pow(remap, 1f / Settings.power);
                    uint f = (uint)Mathf.Round(remap * (Mathf.Pow(2, Settings.precision) - 1f));
                    rsuv |= f << 0;
                }

                return rsuv;
            }
        }

        public override string HlslType => Settings.hlslPrecision.ToHlslString();

        public override string HlslDecoder(string paramName, uint bitIndex) =>
            $"{paramName} = pow((({rsuvDefineSymbol} >> {bitIndex}) & ((1 << {Settings.precision}) - 1)) / {Mathf.Pow(2, Settings.precision) - 1}.0, {Settings.power.ToString()}) * {(Settings.maxValue - Settings.minValue)} + {Settings.minValue};";
    }
}
