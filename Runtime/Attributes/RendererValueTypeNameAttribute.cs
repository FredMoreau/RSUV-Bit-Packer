using System;

namespace UnityEngine.RSUVBitPacker
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RendererValueTypeNameAttribute : Attribute
    {
        public string Name;
        public RendererValueTypeNameAttribute(string name) => Name = name;
    }
}
