using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.RSUVBitPacker
{
    [CreateAssetMenu(fileName = "ColorPalette", menuName = "Rendering/RSUV Bit Packer/ColorPalette")]
    [HelpURL("https://github.com/FredMoreau/RSUV-Bit-Packer/#color-palette")]
    public class ColorPalette : ScriptableObject
    {
        [SerializeField]
        private Color[] colors;

#if UNITY_EDITOR
        [SerializeField]
        ShaderInclude shaderInclude;
#endif

        public int Count => colors.Length;

        public Color this[int index] => colors[index];

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public class Enumerator
        {
            int index;
            ColorPalette palette;
            public Enumerator(ColorPalette palette)
            {
                this.palette = palette;
                index = -1;
            }

            public bool MoveNext()
            {
                index++;
                return (index < palette.colors.Length);
            }

            public Color Current => palette.colors[index];
        }
    }
}
