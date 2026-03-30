using System;
using UnityEngine;

namespace UnityEngine.RSUVBitPacker.RendererProperties
{
    [Serializable]
    [RendererValueTypeName("Scalar")]
    [RendererValueTypeTooltip("The Scalar Property allows setting a precision (1 to 32 bits), and a min and max value.")]
    public class ScalarProperty : RendererProperty<float, ScalarProperty.PropertySettings>
    {
        [Serializable]
        public struct PropertySettings : IEquatable<PropertySettings>
        {
            public uint precision;
            public float minValue;
            public float maxValue;

            public PropertySettings(uint length, float minValue, float maxValue)
            {
                this.precision = length;
                this.minValue = minValue;
                this.maxValue = maxValue;
            }

            public uint Length => precision;

            public bool Equals(PropertySettings other)
            {
                return other.precision == this.precision && other.minValue == this.minValue && other.maxValue == this.maxValue;
            }
        }

        public ScalarProperty()
        {
            Settings = new PropertySettings(8, 0, 1);
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
                    uint f = (uint)Mathf.Round(remap * (Mathf.Pow(2, Settings.precision) - 1f));
                    rsuv |= f << 0;
                }

                return rsuv;
            }
        }

        public override string HlslType => "half";
        public override string HlslDecoder(string paramName, uint bitIndex) =>
            $"{paramName} = (({rsuvDefineSymbol} >> {bitIndex}) & ((1 << {Settings.precision}) - 1)) / {Mathf.Pow(2, Settings.precision) - 1}.0 * {(Settings.maxValue - Settings.minValue)} + {Settings.minValue};";
    }
}
