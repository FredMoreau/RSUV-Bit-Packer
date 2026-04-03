using System;

namespace UnityEngine.RSUVBitPacker.RendererProperties
{
    /// <summary>
    /// A renderer property representing an enumerated selection backed by a list of string labels.
    /// </summary>
    /// <remarks>
    /// The <c>EnumProperty</c> stores an integer index selected from <see cref="PropertySettings.labels"/> and
    /// packs that index into the renderer user value using <see cref="Length"/> bits. The labels array is
    /// stored by reference inside the settings struct; callers should be aware that copying the struct will
    /// copy the reference only. Use the provided helper when cloning to perform a deep copy where needed.
    /// </remarks>
    [Serializable]
    [RendererValueTypeName("Enum")]
    [RendererValueTypeTooltip("The Enum Property allows setting a list of labels that will generate an integer.")]
    public class EnumProperty : RendererProperty<int, EnumProperty.PropertySettings>
    {
        [Serializable]
        /// <summary>
        /// Settings describing the enum options exposed by this property.
        /// </summary>
        public struct PropertySettings : IEquatable<PropertySettings>
        {
            /// <summary>
            /// Array of option labels. The index into this array corresponds to the integer value stored.
            /// </summary>
            public string[] labels;

            /// <summary>
            /// Create new settings with the provided labels. The array reference is stored as-is.
            /// </summary>
            /// <param name="labels">An array of option labels.</param>
            public PropertySettings(string[] labels)
            {
                // Defensive copy to avoid external shared reference
                this.labels = labels != null ? (string[])labels.Clone() : null;
            }

            /// <summary>
            /// Compare two settings instances for equality. This compares array contents, not just references.
            /// </summary>
            /// <param name="other">Other settings instance.</param>
            /// <returns>True when label sequences are equal; otherwise false.</returns>
            public bool Equals(PropertySettings other)
            {
                if (ReferenceEquals(this.labels, other.labels))
                    return true;
                if (this.labels == null || other.labels == null)
                    return false;
                if (this.labels.Length != other.labels.Length)
                    return false;
                for (int i = 0; i < this.labels.Length; i++)
                {
                    if (this.labels[i] != other.labels[i])
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Default constructor initializing the enum with example options.
        /// </summary>
        public EnumProperty()
        {
            Settings = new PropertySettings(new string[] { "Option A", "Option B", "Option C" });
        }

        /// <summary>
        /// Clone the property instance performing a deep copy of the settings' labels array so cloned
        /// instances do not share the same reference.
        /// </summary>
        internal protected override RendererProperty<int> Clone()
        {
            var clone = base.Clone() as EnumProperty;
            var labelsCopy = Settings.labels != null ? (string[])Settings.labels.Clone() : null;
            clone.Settings = new PropertySettings(labelsCopy);
            return clone;
        }

        /// <summary>
        /// Number of bits required to encode the selection index (rounded up to next power of two).
        /// </summary>
        public override uint Length => (uint)Mathf.RoundToInt(Mathf.Log((uint)Mathf.NextPowerOfTwo(Settings.labels.Length), 2));

        /// <summary>
        /// Packed unsigned integer representation of the selected index (clamped to representable range).
        /// </summary>
        public override uint Data => (uint)Mathf.Clamp(Value, 0, Mathf.Pow(2, Length));

        /// <summary>
        /// Corresponding HLSL type for this property (integer).
        /// </summary>
        public override string HlslType => HlslPrecision.Int.ToHlslString();

        /// <summary>
        /// Returns HLSL code that decodes the enum integer from the packed user value.
        /// </summary>
        /// <param name="paramName">Target HLSL variable.</param>
        /// <param name="bitIndex">Starting bit index within the packed user value.</param>
        public override string HlslDecoder(string paramName, uint bitIndex) => $"{paramName} = (({rsuvDefineSymbol} >> {bitIndex}) & ((1 << {Length}) - 1));";
    }
}
