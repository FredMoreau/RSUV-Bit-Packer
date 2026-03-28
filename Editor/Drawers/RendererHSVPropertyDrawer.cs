using UnityEngine;
using UnityEngine.RSUVBitPacker;

namespace UnityEditor.RSUVBitPacker
{
    [CustomPropertyDrawer(typeof(RendererHSVProperty.PropertySettings))]
    public class RendererHSVPropertySettingsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var lengthProp = property.FindPropertyRelative("precision3");

            EditorGUI.PropertyField(position, lengthProp, label);
        }
    }
}
