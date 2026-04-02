using System;

namespace UnityEngine.RSUVBitPacker
{
    [Serializable]
    public abstract class RendererProperty<T> : IRendererProperty where T : struct
    {
        public const string rsuvDefineSymbol = "RSUV";
        public const string nameFieldName = nameof(name);
        public const string valueFieldName = nameof(_value);

        [SerializeField] protected string name;
        public string Name => name;

        [SerializeField]
        private T _value;
        internal protected virtual T Value { get => _value; set => _value = value; }

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

        public abstract uint Length { get; }
        uint IRendererProperty.Length => this.Length;

        public abstract uint Data { get; }
        uint IRendererProperty.Data => this.Data;

        public virtual string HlslType { get => ""; }
        string IRendererProperty.HlslType => this.HlslType;
        public virtual string HlslDecoder(string paramName, uint bitIndex) => "";
        string IRendererProperty.HlslDecoder(string paramName, uint bitIndex) => this.HlslDecoder(paramName, bitIndex);

        internal protected virtual RendererProperty<T> Clone()
        {
            var clone = (RendererProperty<T>)Activator.CreateInstance(typeof(RendererProperty<T>));
            clone.name = name;
            clone._value = _value;
            return clone;
        }

        IRendererProperty IRendererProperty.Clone()
        {
            return this.Clone();
        }

        internal protected virtual bool Equals(RendererProperty<T> other)
        {
            return this.Name == other.Name && this.ValueType == other.ValueType;
        }

        bool IRendererProperty.Equals(IRendererProperty other)
        {
            return this.Equals(other as RendererProperty<T>);
        }
    }

    [Serializable]
    public abstract class RendererProperty<T,U> : RendererProperty<T> where T : struct where U : struct, IEquatable<U>
    {
        public const string settingsFieldName = nameof(_settings);

        [SerializeField]
        private U _settings;

        protected virtual U Settings { get => _settings; set => _settings = value; }

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
