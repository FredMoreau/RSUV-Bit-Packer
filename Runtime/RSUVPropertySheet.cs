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
        [SerializeField]
        bool splitFunctions;

        void IRendererProperties.Add(RendererPropertyBase property)
        {
            rendererProperties.Add(property);
        }

#if UNITY_EDITOR
        internal void OnValidate()
        {
            var allPropertyPackers = Resources.FindObjectsOfTypeAll<RSUVPropertyPacker>();
            foreach (var propertyPacker in allPropertyPackers)
            {
                if (propertyPacker._propertySheet == this && !propertyPacker.Match(this))
                    propertyPacker.UpdadePropertyList();
            }
        }
#endif
    }
}
