using System;

namespace UnityEngine.RSUVBitPacker.RendererProperties
{
    /// <summary>
    /// An integer property that packs an unsigned integer into a configurable number of bits.
    /// </summary>
    /// <remarks>
    /// The number of bits used to store the integer is defined by <see cref="PropertySettings.length"/>.
    /// The stored value is clamped to the representable range [0, 2^length - 1].
    /// </remarks>
    [Serializable]
    [RendererValueTypeName("Integer")]
    [RendererValueTypeTooltip("The Integer Property allows setting a precision to store an int going from 0 to 2 ^ precision - 1.\ne.g.: 4 bits = 0 - 15")]
    public class IntegerProperty : RendererProperty<int, IntegerProperty.PropertySettings>
    {
        [Serializable]
        /// <summary>
        /// Settings controlling the integer property's bit length.
        /// </summary>
        public struct PropertySettings : IEquatable<PropertySettings>
        {
            /// <summary>
            /// Number of bits allocated to store the integer value.
            /// </summary>
            [Tooltip("Number of bits allocated to store the integer value.")]
            public uint length;

            /// <summary>
            /// Create a new <see cref="PropertySettings"/> with the given bit length.
            /// </summary>
            /// <param name="length">Number of bits used to store the integer.</param>
            public PropertySettings(uint length)
            {
                this.length = length;
            }

            /// <summary>
            /// Compare two settings instances for equality.
            /// </summary>
            /// <param name="other">Other settings instance.</param>
            /// <returns>True when lengths are equal; otherwise false.</returns>
            public bool Equals(PropertySettings other)
            {
                return other.length == this.length;
            }
        }

        /// <summary>
        /// Default constructor initializing settings to 4 bits of precision.
        /// </summary>
        public IntegerProperty()
        {
            Settings = new PropertySettings(4);
        }

        /// <summary>
        /// The number of bits used by this property (as defined in <see cref="PropertySettings.length"/>).
        /// </summary>
        public override uint Length => Settings.length;

        /// <summary>
        /// The packed unsigned integer representation clamped to the representable range.
        /// </summary>
        public override uint Data => (uint)Mathf.Clamp(Value, 0, Mathf.Pow(2, Length));// - 1;

        /// <summary>
        /// Corresponding HLSL type for this property.
        /// </summary>
        public override string HlslType => HlslPrecision.Int.ToHlslString();

        /// <summary>
        /// Returns HLSL code that decodes the integer from the packed user value.
        /// </summary>
        /// <param name="paramName">Target HLSL variable.</param>
        /// <param name="bitIndex">Starting bit index for this property's data.</param>
        public override string HlslDecoder(string paramName, uint bitIndex) => $"{paramName} = (({rsuvDefineSymbol} >> {bitIndex}) & ((1 << {Length}) - 1));";
    }
}
