using System;

namespace UnityEngine.RSUVBitPacker.RendererProperties
{
    [Serializable]
    [RendererValueTypeName("Integer")]
    [RendererValueTypeTooltip("The Integer Property allows setting a precision to store an int going from 0 to 2 ^ precision - 1.\ne.g.: 4 bits = 0 - 15")]
    public class IntegerProperty : RendererProperty<int, IntegerProperty.PropertySettings>
    {
        [Serializable]
        public struct PropertySettings : IEquatable<PropertySettings>
        {
            public uint length;

            public PropertySettings(uint length)
            {
                this.length = length;
            }

            public bool Equals(PropertySettings other)
            {
                return other.length == this.length;
            }
        }

        public IntegerProperty()
        {
            Settings = new PropertySettings(4);
        }

        public override uint Length => Settings.length;

        public override uint Data => (uint)Mathf.Clamp(Value, 0, Mathf.Pow(2, Length));// - 1;

        public override string HlslType => "int";

        public override string HlslDecoder(string paramName, uint bitIndex) => $"{paramName} = (({rsuvDefineSymbol} >> {bitIndex}) & ((1 << {Length}) - 1));";
    }
}
