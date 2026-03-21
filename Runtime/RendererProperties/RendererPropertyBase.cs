using System;

namespace UnityEngine.RSUVBitPacker
{
    [Serializable]
    public abstract class RendererPropertyBase
    {
        [SerializeField] protected string name;
        public string Name => name;

        public abstract uint Length { get; }
        public abstract uint Data { get; }
        public abstract string hlslType { get; }
        public abstract string hlslDecoder(string paramName, uint bitIndex);

        public abstract RendererPropertyBase Clone();
    }

    [Serializable]
    public abstract class RendererProperty<T> : RendererPropertyBase where T : struct
    {
        public abstract T Value { get; set; }

        public override string hlslType { get => ""; }
        public override string hlslDecoder(string paramName, uint bitIndex) => "";

        public override RendererPropertyBase Clone()
        {
            var clone = (RendererProperty<T>)Activator.CreateInstance(this.GetType());
            clone.name = name;
            clone.Value = Value;
            return clone;
        }
    }
}
