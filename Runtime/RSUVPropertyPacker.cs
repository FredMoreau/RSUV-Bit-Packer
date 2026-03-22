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

        public void Add(RendererPropertyBase property)
        {
            rendererProperties.Add(property);
        }

        public void TrySetValue<T>(string propertyName, T value) where T : struct
        {
            var prop = rendererProperties.Find(x => x.Name == propertyName);
            if (prop == null || prop.ValueType != typeof(T))
                return;
            prop.SetValue(value);
        }

        public int GetPropertyId(string propertyName)
        {
            var prop = rendererProperties.Find(x => x.Name == propertyName);
            if (prop == null)
                return -1;
            else
                return rendererProperties.IndexOf(prop);
        }

        public void TrySetValue<T>(int propertyId, T value) where T : struct
        {
            var prop = rendererProperties[propertyId];
            if (prop == null || prop.ValueType != typeof(T))
                return;
            prop.SetValue(value);
        }

        private void Awake()
        {
            foreach (var property in rendererProperties)
            {
                property.ValueChanged += (x) =>
                {
                    Apply();
                };
            }
        }

        private void OnDestroy()
        {
            
        }

        //public void UpdateProperties()
        //{

        //}

        // TODO : events and accessors

        private void OnValidate()
        {
            Apply();
        }

        private void OnDidApplyAnimationProperties()
        {
            Apply();
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
