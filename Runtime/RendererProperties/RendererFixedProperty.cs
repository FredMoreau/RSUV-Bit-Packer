using System;

namespace UnityEngine.RSUVBitPacker
{
    [Serializable]
    [RendererValueTypeName("Float8")]
    [RendererValueTypeLength(8)]
    [RendererValueTypeTooltip("The Float8 Property stores a normalized float (0-1) over 8 bits.")]
    public class RendererFixedProperty : RendererProperty<float>
    {
        public override uint Length => 8;

        public override uint Data => (uint)Mathf.Round(Mathf.Clamp(Value, 0f, 1f) * 255f);

        public override string HlslType => "half";

        public override string HlslDecoder(string paramName, uint bitIndex) => $"{paramName} = (({rsuvDefineSymbol} >> {bitIndex}) & 0xFF) / 255.0;";
    }
}
