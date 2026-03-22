using System.Collections.Generic;
using UnityEditor;

namespace UnityEngine.RSUVBitPacker
{
    [CreateAssetMenu(fileName = "RSUVPropertySheet", menuName = "Rendering/RSUVPropertySheet")]
    public class RSUVPropertySheet : ScriptableObject, IRendererProperties
    {
        [SerializeReference]
        internal List<RendererPropertyBase> rendererProperties = new();
        List<RendererPropertyBase> IRendererProperties.RendererProperties => rendererProperties;

        [SerializeField]
        ShaderInclude shaderInclude;

        void IRendererProperties.Add(RendererPropertyBase property)
        {
            rendererProperties.Add(property);
        }
    }
}
