using System;

namespace UnityEngine.RSUVBitPacker
{
    [Serializable, RendererValueTypeName("Int4"), RendererValueTypeLength(4)]
    public class RendererInt4Property : RendererProperty<int>
    {
        public override uint Length => 4;

        public override uint Data => (uint)Mathf.Clamp(Value, 0, 15);

        public override string HlslType => "int";

        public override string HlslDecoder(string paramName, uint bitIndex) => $"{paramName} = ((rsuv >> {bitIndex}) & 0xF);";
    }
}
