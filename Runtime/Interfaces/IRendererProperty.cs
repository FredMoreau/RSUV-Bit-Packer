using System;

using System;

namespace UnityEngine.RSUVBitPacker
{
    /// <summary>
    /// Internal interface describing the minimal surface required for renderer property implementations.
    /// </summary>
    /// <remarks>
    /// The interface is internal because renderer properties are an implementation detail and should not be
    /// implemented by external assemblies. Consumers (editor tooling, packing logic) interact with properties
    /// through this interface to perform cloning, encoding and to generate shader decoding helpers.
    /// </remarks>
    internal interface IRendererProperty
    {
        /// <summary>
        /// Human readable name for the property.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The <see cref="System.Type"/> of the value stored by the property.
        /// </summary>
        Type ValueType { get; }

        /// <summary>
        /// Set the property value using a boxed <see cref="object"/>.
        /// Implementations should validate and cast to the expected type.
        /// </summary>
        /// <param name="value">Boxed value to assign.</param>
        void SetValue(object value);

        /// <summary>
        /// Get the property value boxed as an <see cref="object"/>.
        /// </summary>
        /// <returns>The boxed value.</returns>
        object GetValue();

        /// <summary>
        /// Number of bits used to encode this property into the packed renderer user value.
        /// </summary>
        uint Length { get; }

        /// <summary>
        /// Unsigned integer holding the encoded property data ready for packing.
        /// </summary>
        uint Data { get; }

        /// <summary>
        /// HLSL type keyword (for example "int", "half", "float") used when generating shader decoding code.
        /// </summary>
        string HlslType { get; }

        /// <summary>
        /// Returns HLSL code that decodes the property value from the packed user value and assigns it to the
        /// provided HLSL parameter name.
        /// </summary>
        /// <param name="paramName">Target parameter name in the generated HLSL code.</param>
        /// <param name="bitIndex">Starting bit index within the packed value.</param>
        /// <returns>HLSL code snippet as a string.</returns>
        string HlslDecoder(string paramName, uint bitIndex);

        /// <summary>
        /// Create a clone of this renderer property. Implementations should ensure cloned instances do not
        /// unintentionally share mutable reference-type state (perform deep copies where appropriate).
        /// </summary>
        /// <returns>A new <see cref="IRendererProperty"/> instance with the same value/settings.</returns>
        IRendererProperty Clone();

        /// <summary>
        /// Compare this property with another for equality of name, type and encoded settings/value where applicable.
        /// </summary>
        /// <param name="other">Other property to compare to.</param>
        /// <returns>True if the two properties are considered equal.</returns>
        bool Equals(IRendererProperty other);
    }
}
