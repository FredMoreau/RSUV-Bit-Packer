using System;

namespace UnityEngine.RSUVBitPacker.Samples
{
    [Serializable, RendererValueTypeName("Color16 (HS)"), RendererValueTypeLength(16)]
    public class RendererFixedHSProperty : RendererProperty<Color>
    {
        public override uint Length => 16;

        public override uint Data
        {
            get
            {
                Color.RGBToHSV(Value, out float h, out float s, out float v);
                h = Math.Clamp(h, 0f, 1f);
                s = Math.Clamp(s, 0f, 1f);

                uint hi = (uint)Math.Round(h * 255f);
                uint si = (uint)Math.Round(s * 255f);

                return (hi << 0) | (si << 8);
            }
        }

        public override string HlslType => "half3";
        public override string HlslDecoder(string paramName, uint bitIndex) => @$"half3 hsv = half3(
        ({rsuvDefineSymbol} >> {bitIndex}) & 0xFF,
        ({rsuvDefineSymbol} >> {bitIndex}+8) & 0xFF,
        255) / 255.0;
    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 P = abs(frac(hsv.xxx + K.xyz) * 6.0 - K.www);
    {paramName} = hsv.z * lerp(K.xxx, saturate(P - K.xxx), hsv.y);";
    }
}
