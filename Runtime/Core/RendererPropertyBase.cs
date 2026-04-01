using System;

namespace UnityEngine.RSUVBitPacker
{
    /// <summary>
    /// Base abstraction for renderer properties that can be packed into a 32-bit renderer user value.
    /// </summary>
    /// <remarks>
    /// Not intended for direct inheritance. Derive from the provided generic base classes
    /// <see cref="RendererProperty{T}"/> or <see cref="RendererProperty{T,U}"/> which implement typed behavior.
    /// Implementations describe how a value maps to bits (<see cref="Length"/>, <see cref="Data"/>) and provide
    /// HLSL helper information (<see cref="HlslType"/>, <see cref="HlslDecoder(string,uint)"/>).
    ///
    /// This class exposes cloning and equality helpers used by editor tooling and the property-sheet workflow.
    /// </remarks>
    [Serializable]
    public abstract class RendererPropertyBase
    {
        public const string nameFieldName = nameof(name);
        public const string rsuvDefineSymbol = "RSUV";

        [SerializeField] protected string name;
        public string Name => name;

        internal abstract Type ValueType { get; }
        internal abstract void SetValue(object value);
        internal abstract object GetValue();

        public abstract uint Length { get; }
        public abstract uint Data { get; }
        public abstract string HlslType { get; }
        public abstract string HlslDecoder(string paramName, uint bitIndex);

        internal protected virtual RendererPropertyBase Clone()
        {
            var clone = (RendererPropertyBase)Activator.CreateInstance(this.GetType());
            clone.name = name;
            return clone;
        }

        internal virtual bool Equals(RendererPropertyBase other)
        {
            return this.name == other.name && this.ValueType == other.ValueType;
        }
    }

    /// <summary>
    /// Generic typed renderer property storing a value of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The value type for this property. Must be a value type (<c>struct</c>).</typeparam>
    /// <remarks>
    /// Provides default implementations for <see cref="ValueType"/>, <see cref="SetValue(object)"/>, <see cref="GetValue"/>,
    /// and cloning of the stored value. Concrete property types should override <see cref="HlslType"/> and
    /// <see cref="HlslDecoder(string,uint)"/> to provide shader-side typing and decoding logic.
    /// </remarks>
    [Serializable]
    public abstract class RendererProperty<T> : RendererPropertyBase where T : struct
    {
        public const string valueFieldName = nameof(_value);

        [SerializeField]
        private T _value;
        internal override Type ValueType => typeof(T);
        public virtual T Value { get => _value; set => _value = value; }

        internal override void SetValue(object value)
        {
            Value = (T)value;
        }

        internal override object GetValue()
        {
            return Value;
        }

        public override string HlslType { get => ""; }
        public override string HlslDecoder(string paramName, uint bitIndex) => "";

        internal protected override RendererPropertyBase Clone()
        {
            var clone = base.Clone() as RendererProperty<T>;
            clone._value = _value;
            return clone;
        }
    }

    /// <summary>
    /// Renderer property that includes an additional settings struct of type <typeparamref name="U"/>.
    /// </summary>
    /// <typeparam name="T">The primary value type for this property. Must be a value type (<c>struct</c>).</typeparam>
    /// <typeparam name="U">A settings type used to configure the property. Must be a value type and implement <see cref="IEquatable{U}"/>.</typeparam>
    /// <remarks>
    /// Useful for properties that require extra configuration (for example quantization, ranges, or encoding parameters).
    /// The settings are cloned and compared as part of asset matching and serialization workflows.
    /// </remarks>
    [Serializable]
    public abstract class RendererProperty<T,U> : RendererProperty<T> where T : struct where U : struct, IEquatable<U>
    {
        public const string settingsFieldName = nameof(_settings);

        [SerializeField]
        private U _settings;

        protected virtual U Settings { get => _settings; set => _settings = value; }

        internal protected override RendererPropertyBase Clone()
        {
            var clone = base.Clone() as RendererProperty<T,U>;
            clone._settings = _settings;
            return clone;
        }

        internal override bool Equals(RendererPropertyBase other)
        {
            var o = other as RendererProperty<T, U>;
            if (o == null)
                return false;
            return base.Equals(other) && this.Settings.Equals(o.Settings);
        }
    }
}
