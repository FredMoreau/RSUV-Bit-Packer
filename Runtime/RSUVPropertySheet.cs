using System.Collections.Generic;
using UnityEditor;

namespace UnityEngine.RSUVBitPacker
{
    [CreateAssetMenu(fileName = "RSUVPropertySheet", menuName = "Rendering/RSUVPropertySheet")]
    public class RSUVPropertySheet : ScriptableObject, IRendererProperties
    {
        [SerializeReference]
        internal List<RendererPropertyBase> rendererProperties = new();
        public List<RendererPropertyBase> RendererProperties => rendererProperties;

        [SerializeField]
        ShaderInclude shaderInclude;

        public void Add(RendererPropertyBase property)
        {
            rendererProperties.Add(property);
        }

        //public void UpdateProperties()
        //{
            
        //}
    }
}
