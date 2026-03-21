using System;

namespace UnityEngine.RSUVBitPacker
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RendererValueTypeLengthAttribute : Attribute
    {
        public uint Length { get; }
        public RendererValueTypeLengthAttribute(uint length) => Length = length;
    }
}
