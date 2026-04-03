using System.Collections.Generic;

namespace UnityEngine.RSUVBitPacker
{
    internal interface IRendererProperties
    {
        List<IRendererProperty> RendererProperties { get; }
        void Add(IRendererProperty property);

        // Could use a Remove and Indexer?
        //void Remove(RendererPropertyBase property);
        //RendererPropertyBase this[int i] { get; set; }
    }
}
