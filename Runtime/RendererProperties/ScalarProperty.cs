using System;
using UnityEngine;

namespace UnityEngine.RSUVBitPacker.RendererProperties
{
    /// <summary>
    /// A floating-point property packed into a configurable number of bits with a remapped range.
    /// </summary>
    /// <remarks>
    /// The property stores a float value by quantizing it into <see cref="PropertySettings.precision"/> bits
    /// and remapping the value from the configured [minValue, maxValue] range into the unsigned integer range.
    /// Note: Do not derive directly from <c>RendererPropertyBase</c>. Use <c>RendererProperty&lt;T&gt;</c> or
    /// <c>RendererProperty&lt;T,U&gt;</c> for concrete implementations.
    /// </remarks>
    [Serializable]
    [RendererValueTypeName("Scalar")]
    [RendererValueTypeTooltip("The Scalar Property allows setting a precision (1 to 32 bits), and a min and max value.")]
    public class ScalarProperty : RendererProperty<float, ScalarProperty.PropertySettings>
    {
        [Serializable]
        /// <summary>
        /// Settings specifying precision and remap range for the scalar property.
        /// </summary>
        public struct PropertySettings : IEquatable<PropertySettings>
        {
            /// <summary>
            /// Number of bits used to quantize the value.
            /// </summary>
            [Tooltip("Number of bits used to quantize the value.")]
            public uint precision;
            /// <summary>
            /// Minimum value of the remap range.
            /// </summary>
            [Tooltip("Minimum value of the remap range.")]
            public float minValue;
            /// <summary>
            /// Maximum value of the remap range.
            /// </summary>
            [Tooltip("Maximum value of the remap range.")]
            public float maxValue;
            /// <summary>
            /// The HLSL precision/type to use when decoding this value in shader code. Only Half and Float are intended for use here.
            /// </summary>
            [Tooltip("Shader precision to use when decoding (Half or Float).")]
            public HlslPrecision hlslPrecision;

            /// <summary>
            /// Create new settings with given precision and remap range.
            /// </summary>
            /// <param name="length">Number of bits used for precision.</param>
            /// <param name="minValue">Minimum remap value.</param>
            /// <param name="maxValue">Maximum remap value.</param>
            public PropertySettings(uint length, float minValue, float maxValue)
            {
                this.precision = length;
                this.minValue = minValue;
                this.maxValue = maxValue;
                this.hlslPrecision = HlslPrecision.Half;
            }

            /// <summary>
            /// Convenience property exposing the configured bit precision.
            /// </summary>
            public uint Length => precision;

            /// <summary>
            /// Compare two instances for equality.
            /// </summary>
            public bool Equals(PropertySettings other)
            {
                return other.precision == this.precision && other.minValue == this.minValue && other.maxValue == this.maxValue && other.hlslPrecision == this.hlslPrecision;
            }
        }

        /// <summary>
        /// Default constructor initializing precision to 8 bits and range [0,1].
        /// </summary>
        public ScalarProperty()
        {
            Settings = new PropertySettings(8, 0, 1);
        }

        /// <summary>
        /// Number of bits used to store the quantized scalar value.
        /// </summary>
        public override uint Length => Settings.Length;

        /// <summary>
        /// Packed unsigned integer representation of the quantized scalar value.
        /// </summary>
        public override uint Data
        {
            get
            {
                uint rsuv = 0;
                if (Settings.precision != 0)
                {
                    Value = Mathf.Clamp(Value, Settings.minValue, Settings.maxValue);
                    var remap = (Value + (-Settings.minValue)) / (Settings.maxValue - Settings.minValue);
                    uint f = (uint)Mathf.Round(remap * (Mathf.Pow(2, Settings.precision) - 1f));
                    rsuv |= f << 0;
                }

                return rsuv;
            }
        }

        /// <summary>
        /// Corresponding HLSL type for the decoded scalar value.
        /// </summary>
        public override string HlslType => Settings.hlslPrecision.ToHlslString();

        /// <summary>
        /// Returns HLSL code that decodes and remaps the quantized scalar value from the packed user value.
        /// </summary>
        /// <param name="paramName">Target HLSL variable.</param>
        /// <param name="bitIndex">Starting bit index within the packed value.</param>
        public override string HlslDecoder(string paramName, uint bitIndex) =>
            $"{paramName} = (({rsuvDefineSymbol} >> {bitIndex}) & ((1 << {Settings.precision}) - 1)) / {Mathf.Pow(2, Settings.precision) - 1}.0 * {(Settings.maxValue - Settings.minValue)} + {Settings.minValue};";
    }
}
