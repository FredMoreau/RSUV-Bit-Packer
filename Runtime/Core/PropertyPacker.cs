using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Serialization;

namespace UnityEngine.RSUVBitPacker
{
    /// <summary>
    /// Packs a list of renderer properties into a single 32-bit renderer user value and applies it to one or more <see cref="Renderer"/> components.
    /// </summary>
    /// <remarks>
    /// PropertyPacker stores a collection of <see cref="IRendererProperty"/> instances that describe individual properties (boolean, integer, float, vectors, colors, or custom types).
    /// Each property's bit-sized representation is concatenated into a single uint which is written to the renderer's shader user value via <c>renderer.SetShaderUserValue</c>.
    ///
    /// Use this component to efficiently transmit compact property state to shaders that read the packed user value.
    /// - At runtime, call the provided <c>TrySetValue</c> overloads to update property values. Updated properties are marked dirty and written to renderers in <c>LateUpdate</c>.
    /// - In the Editor, the component validates renderer compatibility, synchronizes with a <c>PropertySheet</c>, and applies values immediately where appropriate.
    ///
    /// Notes:
    /// - The packing assumes the total bit length of all configured properties fits into 32 bits.
    /// </remarks>
    [AddComponentMenu("Rendering/RSUV Bit Packer/Property Packer")]
    [HelpURL("https://github.com/FredMoreau/RSUV-Bit-Packer/#property-packer")]
    [ExecuteAlways]
    public sealed class PropertyPacker : MonoBehaviour, IRendererProperties
    {
        [SerializeField]
        internal PropertySheet _propertySheet;

        [SerializeField, FormerlySerializedAs("_renderer")]
        Renderer[] _renderers;

        [SerializeReference]
        internal List<IRendererProperty> rendererProperties = new();

        List<IRendererProperty> IRendererProperties.RendererProperties => rendererProperties;

        List<IRendererProperty> dirtyProperties = new();
        uint RendererUserValue {  get; set; }

        void IRendererProperties.Add(IRendererProperty property)
        {
            rendererProperties.Add(property);
            Apply();
        }

        bool _isDirty;

        /// <summary>
        /// Returns the index of a renderer property by its name.
        /// </summary>
        /// <param name="propertyName">The name of the renderer property to look up.</param>
        /// <returns>
        /// The zero-based index of the property in <see cref="rendererProperties"/> if found; otherwise -1.
        /// </returns>
        public int GetPropertyIndex(string propertyName)
        {
            var prop = rendererProperties.Find(x => x.Name == propertyName);
            if (prop == null)
                return -1;
            else
                return rendererProperties.IndexOf(prop);
        }

        /// <summary>
        /// Attempts to set the value of a renderer property by name using a boxed <see cref="object"/>.
        /// This method resolves the property index and delegates to the indexed overload.
        /// </summary>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The new value boxed as <see cref="object"/>. Supported runtime types: bool, float, int, Color, Vector2, Vector3, Vector4, or a type assignable to the property's <see cref="IRendererProperty.ValueType"/>.</param>
        public void TrySetValue(string propertyName, object value)
        {
            int index = GetPropertyIndex(propertyName);
            if (index >= 0)
                TrySetValue(index, value);
        }

        /// <summary>
        /// Attempts to set the value of a renderer property by index using a boxed <see cref="object"/>.
        /// Supports the built-in primitive/vector/color types as well as any value compatible with the property's declared <see cref="IRendererProperty.ValueType"/>.
        /// If the provided <paramref name="value"/> does not match a supported type or is not assignable to the property's value type, no change is made.
        /// </summary>
        /// <param name="propertyIndex">The zero-based index of the property in <see cref="rendererProperties"/>.</param>
        /// <param name="value">The new value boxed as <see cref="object"/>.</param>
        public void TrySetValue(int propertyIndex, object value)
        {
            var prop = rendererProperties[propertyIndex];
            if (prop == null)
                return;

            switch (value)
            {
                case bool bValue:
                    TrySetValue(propertyIndex, bValue);
                    break;
                case float fValue:
                    TrySetValue(propertyIndex, fValue);
                    break;
                case int iValue:
                    TrySetValue(propertyIndex, iValue);
                    break;
                case Color color:
                    TrySetValue(propertyIndex, color);
                    break;
                case Vector2 vector2:
                    TrySetValue(propertyIndex, vector2);
                    break;
                case Vector3 vector3:
                    TrySetValue(propertyIndex, vector3);
                    break;
                case Vector4 vector4:
                    TrySetValue(propertyIndex, vector4);
                    break;
                default:
                    if (!prop.ValueType.IsAssignableFrom(value.GetType()))
                        return;
                    prop.SetValue(value);
                    dirtyProperties.Add(prop);
                    _isDirty = true;
                    break;
            }
        }

        /// <summary>
        /// Attempts to set the value of a renderer property by name using a strongly-typed value.
        /// This is a generic convenience overload primarily intended for Visual Scripting or external callers that know the value type.
        /// </summary>
        /// <typeparam name="T">The value type. Must be a value type (<c>struct</c>).</typeparam>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The new value.</param>
        public void TrySetValue<T>(string propertyName, T value) where T : struct
        {
            int index = GetPropertyIndex(propertyName);
            if (index >= 0)
                TrySetValue(index, value);
        }

        /// <summary>
        /// Attempts to set the value of a renderer property by index using a strongly-typed value.
        /// The value will only be applied if the property's declared type exactly matches <typeparamref name="T"/>.
        /// When applied, the property is marked dirty so that it will be packed into the renderer user value on the next update.
        /// </summary>
        /// <typeparam name="T">The value type. Must be a value type (<c>struct</c>).</typeparam>
        /// <param name="propertyIndex">The zero-based index of the property in <see cref="rendererProperties"/>.</param>
        /// <param name="value">The new value to assign to the property.</param>
        public void TrySetValue<T>(int propertyIndex, T value) where T : struct
        {
            var prop = rendererProperties[propertyIndex];
            if (prop == null || prop.ValueType != typeof(T))
                return;
            prop.SetValue(value);
            dirtyProperties.Add(prop);
            _isDirty = true;
        }

#if UNITY_EDITOR
        private void Reset()
        {
            _renderers = GetComponentsInChildren<Renderer>();
            CheckRenderersCompatibility();
        }

        internal void OnValidate()
        {
            CheckRenderersCompatibility();
            Apply();
        }

        void CheckRenderersCompatibility()
        {
            List<Renderer> unsupportedRenderers = new();
            foreach (var renderer in _renderers)
            {
                if (!renderer.SupportsShaderUserValue())
                {
                    Debug.LogWarning($"Renderer on {renderer.name} doesn't support RSUV.", renderer);
                    unsupportedRenderers.Add(renderer);
                }
            }
            if (unsupportedRenderers.Count > 0)
            {
                List<Renderer> supportedRenderers = new();
                foreach (var renderer in _renderers)
                {
                    if (!unsupportedRenderers.Contains(renderer))
                        supportedRenderers.Add(renderer);
                }
                _renderers = supportedRenderers.ToArray();
            }
        }

        internal bool Match(IRendererProperties other)
        {
            if (rendererProperties.Count != other.RendererProperties.Count)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < other.RendererProperties.Count; i++)
                    if (!this.rendererProperties[i].Equals(other.RendererProperties[i]))
                        return false;
            }
            return true;
        }

        internal void UpdadePropertyList()
        {
            if (_propertySheet != null)
            {
                Undo.RecordObject(this, "Update Renderer Properties");
                List<IRendererProperty> newProperties = new();
                foreach (IRendererProperty property in _propertySheet.rendererProperties)
                {
                    var clone = property.Clone();
                    var existingProp = rendererProperties.Find(p => p.Name == clone.Name && p.ValueType == clone.ValueType);
                    if (existingProp != null)
                    {
                        clone.SetValue(existingProp.GetValue());
                    }
                    newProperties.Add(clone);
                }
                rendererProperties.Clear();
                rendererProperties.AddRange(newProperties);
                Apply();
            }
        }
#endif

        private void Start()
        {
#if !UNITY_EDITOR
            enabled = _renderers.Length != 0;
#endif
            Apply();
        }

        private void OnDidApplyAnimationProperties()
        {
            Apply();
        }

        private void LateUpdate()
        {
            ApplyPropertiesIfDirty();
        }

        private void ApplyPropertiesIfDirty()
        {
            if (_isDirty)
            {
                uint rsuv = RendererUserValue;
                int offset = 0;
                foreach (IRendererProperty prop in rendererProperties)
                {
                    if (dirtyProperties.Contains(prop))
                    {
                        uint mask = ((1u << (int)prop.Length) - 1u) << offset;
                        rsuv &= ~mask;
                        rsuv |= prop.Data << offset;
                    }
                    offset += (int)prop.Length;
                }
                RendererUserValue = rsuv;

                foreach (var renderer in _renderers)
                    renderer.SetShaderUserValue(RendererUserValue);

                dirtyProperties.Clear();
                _isDirty = false;
            }
        }

        private void Apply()
        {
            if (_renderers == null || _renderers.Length == 0)
                return;

            RendererUserValue = ((IRendererProperties)this).GetRendererUserValue();

            for (int i = 0; i < _renderers.Length; i++)
                _renderers[i].SetShaderUserValue(RendererUserValue);
        }
    }
}
