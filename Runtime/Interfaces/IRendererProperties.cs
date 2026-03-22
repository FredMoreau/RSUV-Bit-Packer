using System.Collections.Generic;

namespace UnityEngine.RSUVBitPacker
{
    internal interface IRendererProperties
    {
        List<RendererPropertyBase> RendererProperties { get; }
        void Add(RendererPropertyBase property);

        // Could use a Remove and Indexer?
        //void Remove(RendererPropertyBase property);
        //RendererPropertyBase this[int i] { get; set; }
    }
}
