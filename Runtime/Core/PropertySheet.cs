using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.RSUVBitPacker
{
    /// <summary>
    /// A reusable collection of renderer properties that can be saved as an asset and shared between components.
    /// </summary>
    /// <remarks>
    /// `PropertySheet` is a `ScriptableObject` used to store a serializable list of `RendererPropertyBase` instances.
    /// Create instances via the Unity menu (Assets ? Create ? Rendering ? RSUV Bit Packer ? Property Sheet).
    ///
    /// Consumers such as `PropertyPacker` read the stored `rendererProperties` by implementing `IRendererProperties` (explicit interface).
    /// Editor-only fields (for example `shaderInclude` and `splitFunctions`) provide additional data used by tooling and generation workflows.
    ///
    /// Notes:
    /// - The `rendererProperties` list is serialized with `SerializeReference` to preserve concrete derived types of `RendererPropertyBase`.
    /// </remarks>
    [CreateAssetMenu(fileName = "PropertySheet", menuName = "Rendering/RSUV Bit Packer/Property Sheet")]
    [HelpURL("https://github.com/FredMoreau/RSUV-Bit-Packer/#property-sheet")]
    public sealed class PropertySheet : ScriptableObject, IRendererProperties
    {
        [SerializeReference]
        internal List<IRendererProperty> rendererProperties = new();
        List<IRendererProperty> IRendererProperties.RendererProperties => rendererProperties;

#if UNITY_EDITOR
        [SerializeField]
        ShaderInclude shaderInclude;
        [SerializeField]
        bool splitFunctions;
#endif

        void IRendererProperties.Add(IRendererProperty property)
        {
            rendererProperties.Add(property);
        }
    }
}
