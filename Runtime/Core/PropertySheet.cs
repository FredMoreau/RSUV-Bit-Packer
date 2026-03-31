using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.RSUVBitPacker
{
    [CreateAssetMenu(fileName = "PropertySheet", menuName = "Rendering/RSUV Bit Packer/Property Sheet")]
    public class PropertySheet : ScriptableObject, IRendererProperties
    {
        [SerializeReference]
        internal List<RendererPropertyBase> rendererProperties = new();
        List<RendererPropertyBase> IRendererProperties.RendererProperties => rendererProperties;

#if UNITY_EDITOR
        [SerializeField]
        ShaderInclude shaderInclude;
        [SerializeField]
        bool splitFunctions;
#endif

        void IRendererProperties.Add(RendererPropertyBase property)
        {
            rendererProperties.Add(property);
        }
    }
}
