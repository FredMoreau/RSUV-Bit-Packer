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

        void SanitizeProperties()
        {
            int nullCount = 0;
            for (int i = RendererProperties.Count - 1; i >= 0; i--)
            {
                if (RendererProperties[i] == null)
                {
                    RendererProperties.RemoveAt(i);
                    nullCount++;
                }
            }
            if (nullCount > 0)
                Debug.LogWarning($"Removed {nullCount} null properties from {this}");
        }

        uint GetRendererUserValue()
        {
            uint result = 0;
            int offset = 0;
            foreach (IRendererProperty prop in RendererProperties)
            {
                result |= prop.Data << offset;
                offset += (int)prop.Length;
            }
            return result;
        }
    }
}
