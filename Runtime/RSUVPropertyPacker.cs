using System.Collections.Generic;

namespace UnityEngine.RSUVBitPacker
{
    [AddComponentMenu("Rendering/RSUV Property Packer")]
    [ExecuteAlways]
    public class RSUVPropertyPacker : MonoBehaviour, IRendererProperties
    {
        [SerializeField]
        RSUVPropertySheet _propertySheet;

        [SerializeField]
        Renderer _renderer;

        [SerializeReference]
        internal List<RendererPropertyBase> rendererProperties = new();

        public List<RendererPropertyBase> RendererProperties => rendererProperties;

        List<RendererPropertyBase> dirtyProperties = new();

        public void Add(RendererPropertyBase property)
        {
            rendererProperties.Add(property);
        }

        bool _isDirty;

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

        //private void Awake()
        //{
        //}

        private void OnDestroy()
        {
            
        }

        //public void UpdateProperties()
        //{

        //}

        private void OnValidate()
        {
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

        internal void UpdadePropertyList()
        {
            rendererProperties.Clear();
            if (_propertySheet != null)
            {
                foreach (RendererPropertyBase property in _propertySheet.RendererProperties)
                {
                    var clone = property.Clone();
                    rendererProperties.Add(clone);
                }
            }
        }

        void ApplyPropertiesIfDirty()
        {
            if (_isDirty)
            {
                uint rsuv = _renderer.GetShaderUserValue();
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
                _renderer.SetShaderUserValue(rsuv);
                _isDirty = false;
            }
        }

        public void Apply()
        {
            if (_renderer == null)
                return;

            uint rsuv = GatherValues();

            _renderer.SetShaderUserValue(rsuv);
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
