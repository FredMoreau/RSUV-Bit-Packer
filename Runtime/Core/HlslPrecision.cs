using UnityEngine;

namespace UnityEngine.RSUVBitPacker
{
    /// <summary>
    /// Represents the HLSL type keywords used for packed property decoding and shader generation.
    /// </summary>
    public enum HlslPrecision
    {
        /// <summary>HLSL <c>bool</c> type.</summary>
        Bool,
        /// <summary>HLSL <c>uint</c> type.</summary>
        UInt,
        /// <summary>HLSL <c>int</c> type.</summary>
        Int,
        /// <summary>HLSL <c>half</c> type (16-bit float).</summary>
        Half,
        /// <summary>HLSL <c>float</c> type (32-bit float).</summary>
        Float
    }

    /// <summary>
    /// Extension helpers for <see cref="HlslPrecision"/>.
    /// </summary>
    public static class HlslPrecisionExtensions
    {
        /// <summary>
        /// Returns the HLSL keyword string for the given precision.
        /// Example: <c>HlslPrecision.Half.ToHlslString()</c> -> "half".
        /// </summary>
        public static string ToHlslString(this HlslPrecision precision)
        {
            switch (precision)
            {
                case HlslPrecision.Bool:
                    return "bool";
                case HlslPrecision.UInt:
                    return "uint";
                case HlslPrecision.Int:
                    return "int";
                case HlslPrecision.Half:
                    return "half";
                case HlslPrecision.Float:
                    return "float";
                default:
                    return precision.ToString().ToLowerInvariant();
            }
        }
    }
}
