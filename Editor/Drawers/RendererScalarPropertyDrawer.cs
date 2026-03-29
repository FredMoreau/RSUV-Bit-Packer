using UnityEngine;
using UnityEngine.RSUVBitPacker;

namespace UnityEditor.RSUVBitPacker
{
    [CustomPropertyDrawer(typeof(RendererScalarProperty))]
    public class RendererScalarPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var nameProp = property.FindPropertyRelative(RendererPropertyBase.nameFieldName);
            var valueProp = property.FindPropertyRelative(RendererGenericIntProperty.valueFieldName);
            var lengthProp = property.FindPropertyRelative($"{RendererGenericIntProperty.settingsFieldName}.precision");
            var minValueProp = property.FindPropertyRelative($"{RendererGenericIntProperty.settingsFieldName}.minValue");
            var maxValueProp = property.FindPropertyRelative($"{RendererGenericIntProperty.settingsFieldName}.maxValue");

            valueProp.floatValue = EditorGUI.Slider(position, label, valueProp.floatValue, minValueProp.floatValue, maxValueProp.floatValue);
        }
    }
}
