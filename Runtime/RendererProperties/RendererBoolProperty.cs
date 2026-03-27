using System;

namespace UnityEngine.RSUVBitPacker
{
    [Serializable, RendererValueTypeName("Boolean"), RendererValueTypeLength(1), RendererValueTypeTooltip("A single bit storing a boolean value.")]
    public class RendererBoolProperty : RendererProperty<bool>
    {
        public override uint Length => 1;
        public override uint Data => Value ? 1u : 0u;

        public override string HlslType => "bool";

        public override string HlslDecoder(string paramName, uint bitIndex) => $"{paramName} = (rsuv & (1 << {bitIndex})) != 0;";
    }
}
