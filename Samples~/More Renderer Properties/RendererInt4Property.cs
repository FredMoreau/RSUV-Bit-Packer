using System;

namespace UnityEngine.RSUVBitPacker
{
    [Serializable, RendererValueTypeName("Int4"), RendererValueTypeLength(4)]
    public class RendererInt4Property : RendererProperty<int>
    {
        [SerializeField, Range(0, 15)]
        private int _value = default;

        public override int Value { get => _value; set => _value = value; }

        public override uint Length => 4;

        public override uint Data => (uint)Mathf.Clamp(_value, 0, 15);

        public override string hlslType => "int";

        public override string hlslDecoder(string paramName, uint bitIndex) => $"{paramName} = ((rsuv >> {bitIndex}) & 0xF);";
    }
}
