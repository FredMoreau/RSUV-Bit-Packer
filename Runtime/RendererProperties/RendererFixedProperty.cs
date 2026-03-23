using System;

namespace UnityEngine.RSUVBitPacker
{
    [Serializable, RendererValueTypeName("Float8"), RendererValueTypeLength(8)]
    public class RendererFixedProperty : RendererProperty<float>
    {
        [SerializeField, Range(0f, 1f)]
        private float _value = default;

        public override float Value { get => _value; set => _value = value; }

        public override uint Length => 8;

        public override uint Data => (uint)Mathf.Round(Math.Clamp(_value, 0f, 1f) * 255f);

        public override string hlslType => "half";

        public override string hlslDecoder(string paramName, uint bitIndex) => $"{paramName} = ((rsuv >> {bitIndex}) & 0xFF) / 255.0;";
    }
}
