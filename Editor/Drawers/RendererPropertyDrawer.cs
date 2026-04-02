using UnityEngine;
using UnityEngine.RSUVBitPacker;

namespace UnityEditor.RSUVBitPacker
{
    [CustomPropertyDrawer(typeof(IRendererProperty))]
    public class RendererPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var nameProp = property.FindPropertyRelative(RendererProperty<bool>.nameFieldName);
            var valueProp = property.FindPropertyRelative(RendererProperty<bool>.valueFieldName);

            EditorGUI.PropertyField(position, valueProp, label);
        }
    }
}
