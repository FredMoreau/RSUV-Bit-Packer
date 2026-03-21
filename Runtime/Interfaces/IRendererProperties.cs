using System.Collections.Generic;

namespace UnityEngine.RSUVBitPacker
{
    internal interface IRendererProperties
    {
        List<RendererPropertyBase> RendererProperties { get; }
        void Add(RendererPropertyBase property);
        //void UpdateProperties();
    }
}
