using System;

namespace UnityEngine.RSUVBitPacker
{
    [Serializable, RendererValueTypeName("Int")]
    public class RendererGenericIntProperty : RendererProperty<int, RendererGenericIntProperty.PropertySettings>
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

        public RendererGenericIntProperty()
        {
            Settings = new PropertySettings(4);
        }

        public override uint Length => Settings.length;

        public override uint Data => (uint)Mathf.Clamp(Value, 0, Mathf.Pow(2, Length)) - 1;

        public override string HlslType => "half";

        public override string HlslDecoder(string paramName, uint bitIndex) => $"{paramName} = ((rsuv >> {bitIndex}) & ((1 << {Length}) - 1));";
    }
}
