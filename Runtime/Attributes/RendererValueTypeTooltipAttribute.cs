using System;

namespace UnityEngine.RSUVBitPacker
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RendererValueTypeTooltipAttribute : Attribute
    {
        public string Tooltip;
        public RendererValueTypeTooltipAttribute(string tooltip) => Tooltip = tooltip;
    }
}
