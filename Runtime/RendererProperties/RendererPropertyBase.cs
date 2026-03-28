using System;

namespace UnityEngine.RSUVBitPacker
{
    [Serializable]
    public abstract class RendererPropertyBase
    {
        internal const string nameFieldName = nameof(name);
        public const string rsuvDefineSymbol = "RSUV";

        [SerializeField] protected string name;
        public string Name => name;

        internal abstract object GetSettings();
        internal abstract void SetSettings(object settings);

        internal abstract Type ValueType { get; }
        internal abstract void SetValue(object value);
        internal abstract object GetValue();

        public abstract uint Length { get; }
        public abstract uint Data { get; }
        public abstract string HlslType { get; }
        public abstract string HlslDecoder(string paramName, uint bitIndex);

        internal virtual RendererPropertyBase Clone()
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

    [Serializable]
    public abstract class RendererProperty<T> : RendererPropertyBase where T : struct
    {
        internal const string valueFieldName = nameof(_value);

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

        internal override void SetSettings(object settings) { }

        internal override object GetSettings() => null;

        public override string HlslType { get => ""; }
        public override string HlslDecoder(string paramName, uint bitIndex) => "";

        internal override RendererPropertyBase Clone()
        {
            var clone = base.Clone() as RendererProperty<T>;
            clone._value = _value;
            return clone;
        }
    }

    [Serializable]
    public abstract class RendererProperty<T,U> : RendererProperty<T> where T : struct where U : struct, IEquatable<U>
    {
        internal const string settingsFieldName = nameof(_settings);

        [SerializeField]
        private U _settings;

        protected virtual U Settings { get => _settings; set => _settings = value; }

        internal override void SetSettings(object settings)
        {
            Settings = (U)settings;
        }

        internal override object GetSettings()
        {
            return Settings;
        }

        internal override RendererPropertyBase Clone()
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
