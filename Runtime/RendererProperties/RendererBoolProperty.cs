using System;

namespace UnityEngine.RSUVBitPacker
{
    [Serializable, RendererValueTypeName("Boolean"), RendererValueTypeLength(1)]
    public class RendererBoolProperty : RendererProperty<bool>
    {
        [SerializeField] public bool _value;

        public override bool Value { get => _value; set => _value = value; }

        public override uint Length => 1;
        public override uint Data => _value ? 1u : 0u;

        public override string hlslType => "bool";

        public override string hlslDecoder(string paramName, uint bitIndex) => $"{paramName} = (rsuv & (1 << {bitIndex})) != 0;";
    }
}
