using System;

namespace UnityEngine.RSUVBitPacker
{
    internal interface IRendererProperty
    {
        string Name { get; }
        Type ValueType { get; }
        void SetValue(object value);
        object GetValue();

        uint Length { get; }
        uint Data { get; }
        string HlslType { get; }
        string HlslDecoder(string paramName, uint bitIndex);

        IRendererProperty Clone();

        bool Equals(IRendererProperty other);
    }
}
