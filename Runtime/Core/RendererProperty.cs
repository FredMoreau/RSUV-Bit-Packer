using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.RSUVBitPacker
{
    /// <summary>
    /// Generic base class for renderer properties that store a value of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The value type stored by the property. Must be a value type (<c>struct</c>).</typeparam>
    /// <remarks>
    /// Concrete renderer property implementations should override <see cref="Length"/> and <see cref="Data"/>
    /// to describe how the value is encoded into the packed renderer user value. The class implements
    /// <see cref="IRendererProperty"/> explicitly so editor tooling interacts with properties via the
    /// interface while concrete implementations keep type-safe access to their underlying value.
    /// </remarks>
    [Serializable]
    public abstract class RendererProperty<T> : IRendererProperty where T : struct
    {
        /// <summary>
        /// Symbol used in generated HLSL to reference the packed renderer user value.
        /// </summary>
        public const string rsuvDefineSymbol = "RSUV";

        /// <summary>
        /// Field name for the serialized <see cref="name"/> member.
        /// </summary>
        public const string nameFieldName = nameof(name);

        /// <summary>
        /// Field name for the serialized value field (<see cref="_value"/>).
        /// </summary>
        public const string valueFieldName = nameof(_value);

        [SerializeField] protected string name;

        /// <summary>
        /// Display name of the property.
        /// </summary>
        public string Name => name;

        [SerializeField]
        private T _value;

        /// <summary>
        /// Strongly-typed property value. Protected so derived types can access it.
        /// </summary>
        internal protected virtual T Value { get => _value; set => _value = value; }

        /// <summary>
        /// The <see cref="System.Type"/> instance for the stored value type (<typeparamref name="T"/>).
        /// </summary>
        internal protected Type ValueType => typeof(T);

        Type IRendererProperty.ValueType => this.ValueType;

        void IRendererProperty.SetValue(object value)
        {
            Value = (T)value;
        }

        object IRendererProperty.GetValue()
        {
            return Value;
        }

        /// <summary>
        /// Number of bits used to encode this property into the packed renderer user value.
        /// </summary>
        public abstract uint Length { get; }

        uint IRendererProperty.Length => this.Length;

        /// <summary>
        /// Unsigned integer data representing the encoded value for packing.
        /// </summary>
        public abstract uint Data { get; }

        uint IRendererProperty.Data => this.Data;

        /// <summary>
        /// The HLSL type string used when generating shader-side decoding code. Override if necessary.
        /// </summary>
        public virtual string HlslType { get => ""; }

        string IRendererProperty.HlslType => this.HlslType;

        /// <summary>
        /// Returns HLSL code that decodes this property's value from the packed user value.
        /// </summary>
        /// <param name="paramName">The name of the target HLSL variable to assign.</param>
        /// <param name="bitIndex">Starting bit index in the packed value where this property's bits begin.</param>
        /// <returns>HLSL statement(s) as a string that decode the value into <paramref name="paramName"/>.</returns>
        public virtual string HlslDecoder(string paramName, uint bitIndex) => "";

        string IRendererProperty.HlslDecoder(string paramName, uint bitIndex) => this.HlslDecoder(paramName, bitIndex);

        /// <summary>
        /// Create a shallow clone of this property. Derived classes should override to clone extra state.
        /// </summary>
        /// <remarks>
        /// The default implementation uses <see cref="Activator.CreateInstance(Type)"/> to construct
        /// a new instance of the concrete type and copies the <see cref="name"/> and underlying value.
        /// Implementations that hold reference-type settings should override this method to perform
        /// deep copies of those fields to avoid shared mutable state between clones.
        /// </remarks>
        internal protected virtual RendererProperty<T> Clone()
        {
            var clone = (RendererProperty<T>)Activator.CreateInstance(this.GetType());
            clone.name = name;
            clone._value = _value;
            return clone;
        }

        IRendererProperty IRendererProperty.Clone()
        {
            return this.Clone();
        }

        /// <summary>
        /// Value-based comparison helper for derived classes to implement equality semantics.
        /// </summary>
        internal protected virtual bool Equals(RendererProperty<T> other)
        {
            return this.Name == other.Name && this.ValueType == other.ValueType;
        }

        bool IRendererProperty.Equals(IRendererProperty other)
        {
            return this.Equals(other as RendererProperty<T>);
        }
    }

    /// <summary>
    /// Extension of <see cref="RendererProperty{T}"/> that includes a settings struct of type <typeparamref name="U"/>.
    /// </summary>
    /// <typeparam name="T">The stored value type.</typeparam>
    /// <typeparam name="U">A settings struct type used to configure encoding/decoding. Must implement <see cref="IEquatable{U}"/>.</typeparam>
    [Serializable]
    public abstract class RendererProperty<T,U> : RendererProperty<T> where T : struct where U : struct, IEquatable<U>
    {
        /// <summary>
        /// Field name constant for the serialized settings field.
        /// </summary>
        public const string settingsFieldName = nameof(_settings);

        [SerializeField]
        private U _settings;

        /// <summary>
        /// Typed access to the settings instance. Override visibility to change behavior in derived types.
        /// </summary>
        protected virtual U Settings { get => _settings; set => _settings = value; }

        /// <summary>
        /// Clone the property including its settings. The default implementation copies the settings value.
        /// Override when settings contain reference types that require deep copies.
        /// </summary>
        internal protected override RendererProperty<T> Clone()
        {
            var clone = base.Clone() as RendererProperty<T,U>;
            clone._settings = _settings;
            return clone;
        }

        internal protected override bool Equals(RendererProperty<T> other)
        {
            var o = other as RendererProperty<T,U>;
            if (o == null)
                return false;
            return base.Equals(other) && this.Settings.Equals(o.Settings);
        }
    }
}
