using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Serialization;

namespace UnityEngine.RSUVBitPacker
{
    [AddComponentMenu("Rendering/RSUV Bit Packer/Property Packer")]
    [ExecuteAlways]
    public class PropertyPacker : MonoBehaviour, IRendererProperties
    {
        [SerializeField]
        internal PropertySheet _propertySheet;

        [SerializeField, FormerlySerializedAs("_renderer")]
        Renderer[] _renderers;

        [SerializeReference]
        internal List<RendererPropertyBase> rendererProperties = new();

        List<RendererPropertyBase> IRendererProperties.RendererProperties => rendererProperties;

        List<RendererPropertyBase> dirtyProperties = new();
        uint RendererUserValue {  get; set; }

        void IRendererProperties.Add(RendererPropertyBase property)
        {
            rendererProperties.Add(property);
            Apply();
        }

        bool _isDirty;

        // For Visual Scripting only!
        public void TrySetValue(int propertyIndex, object value)
        {
            var prop = rendererProperties[propertyIndex];
            if (prop == null || !prop.ValueType.IsAssignableFrom(value.GetType()))
                return;
            prop.SetValue(value);
            dirtyProperties.Add(prop);
            _isDirty = true;
        }

        public void TrySetValue<T>(string propertyName, T value) where T : struct
        {
            int index = GetPropertyIndex(propertyName);
            if (index >= 0)
                TrySetValue(index, value);
        }

        public int GetPropertyIndex(string propertyName)
        {
            var prop = rendererProperties.Find(x => x.Name == propertyName);
            if (prop == null)
                return -1;
            else
                return rendererProperties.IndexOf(prop);
        }

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

        internal bool Match(PropertySheet propertySheet)
        {
            bool match = true;
            if (propertySheet != null)
            {
                if (rendererProperties.Count == propertySheet.rendererProperties.Count)
                {
                    for (int i = 0; i < propertySheet.rendererProperties.Count; i++)
                    {
                        if (!this.rendererProperties[i].Equals(propertySheet.rendererProperties[i]))
                        {
                            match = false;
                            break;
                        }
                    }
                }
                else
                {
                    match = false;
                }
            }
            return match;
        }

        internal void UpdadePropertyList()
        {
            if (_propertySheet != null)
            {
                Undo.RecordObject(this, "Update Renderer Properties");
                rendererProperties.Clear();
                foreach (RendererPropertyBase property in _propertySheet.rendererProperties)
                {
                    var clone = property.Clone();
                    rendererProperties.Add(clone);
                }
                Apply();
            }
        }
#endif

        private void Start()
        {
            enabled = _renderers.Length != 0;
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
                foreach (RendererPropertyBase prop in rendererProperties)
                {
                    if (dirtyProperties.Contains(prop))
                    {
                        uint mask = ((1u << (int)prop.Length) - 1u) << offset;
                        rsuv &= ~mask;
                        rsuv |= prop.Data << offset;
                        dirtyProperties.Remove(prop);
                    }
                    offset += (int)prop.Length;
                }
                RendererUserValue = rsuv;
                foreach (var renderer in _renderers)
                    renderer.SetShaderUserValue(RendererUserValue);
                _isDirty = false;
            }
        }

        private void Apply()
        {
            if (_renderers == null || _renderers.Length == 0)
                return;

            RendererUserValue = GatherValues();

            for (int i = 0; i < _renderers.Length; i++)
                _renderers[i].SetShaderUserValue(RendererUserValue);
        }

        private uint GatherValues()
        {
            uint result = 0;
            int offset = 0;
            foreach (RendererPropertyBase prop in rendererProperties)
            {
                result |= prop.Data << offset;
                offset += (int)prop.Length;
            }
            return result;
        }
    }
}
