using System;

namespace UnityEngine.RSUVBitPacker
{
    [Serializable, RendererValueTypeName("Fixed RGB"), RendererValueTypeLength(24)]
    public class RendererFixedRGBProperty : RendererProperty<Color>
    {
        [SerializeField] public Color _value;

        public override Color Value { get => _value; set => _value = value; }

        public override uint Length => 24;

        public override uint Data
        {
            get
            {
                var r = Math.Clamp(_value.r, 0f, 1f);
                var g = Math.Clamp(_value.g, 0f, 1f);
                var b = Math.Clamp(_value.b, 0f, 1f);

                uint ri = (uint)Math.Round(r * 255f);
                uint gi = (uint)Math.Round(g * 255f);
                uint bi = (uint)Math.Round(b * 255f);

                return (ri << 0) | (gi << 8) | (bi << 16);
            }
        }

        public override string hlslType => "half3";
        public override string hlslDecoder(string paramName, uint bitIndex) => @$"{paramName} = half3(
        (rsuv >> {bitIndex}) & 0xFF,
        (rsuv >> {bitIndex}+8) & 0xFF,
        (rsuv >> {bitIndex}+16) & 0xFF
    ) / 255.0;";
    }
}
