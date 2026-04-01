using System;

namespace UnityEngine.RSUVBitPacker
{
    /// <summary>
    /// A single-bit boolean property packed into the renderer user value.
    /// </summary>
    /// <remarks>
    /// This property stores a boolean value using one bit. It derives from <c>RendererProperty&lt;bool&gt;</c>.
    /// Note: Do not derive directly from <c>RendererPropertyBase</c>. Use <c>RendererProperty&lt;T&gt;</c> or
    /// <c>RendererProperty&lt;T,U&gt;</c> for concrete implementations.
    /// </remarks>
    [Serializable, RendererValueTypeName("Boolean"), RendererValueTypeLength(1), RendererValueTypeTooltip("A single bit storing a boolean value.")]
    public class BooleanProperty : RendererProperty<bool>
    {
        /// <summary>
        /// The bit length used to store this property (always 1).
        /// </summary>
        public override uint Length => 1;
        /// <summary>
        /// The packed data representation of the current value (0 or 1).
        /// </summary>
        public override uint Data => Value ? 1u : 0u;

        /// <summary>
        /// The corresponding HLSL type for this property.
        /// </summary>
        public override string HlslType => HlslPrecision.Bool.ToHlslString();

        /// <summary>
        /// Returns HLSL code that decodes the boolean value from the packed uint.
        /// </summary>
        /// <param name="paramName">The target parameter name in HLSL to assign the decoded value to.</param>
        /// <param name="bitIndex">The bit index within the packed value where this property's data starts.</param>
        public override string HlslDecoder(string paramName, uint bitIndex) => $"{paramName} = ({rsuvDefineSymbol} & (1 << {bitIndex})) != 0;";
    }
}
