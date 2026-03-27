using UnityEngine;
using UnityEngine.RSUVBitPacker;

namespace UnityEditor.RSUVBitPacker
{
    [CustomPropertyDrawer(typeof(RendererFixedProperty))]
    public class RendererFixedPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var nameProp = property.FindPropertyRelative(RendererPropertyBase.nameFieldName);
            var valueProp = property.FindPropertyRelative(RendererFixedProperty.valueFieldName);

            var labelStr = string.IsNullOrWhiteSpace(nameProp.stringValue) ? "<no name>" : nameProp.stringValue;

            valueProp.floatValue = EditorGUI.Slider(position, new GUIContent(labelStr), valueProp.floatValue, 0f, 1f);
        }
    }
}
