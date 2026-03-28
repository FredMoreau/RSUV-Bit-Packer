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

            var labelStr = string.IsNullOrWhiteSpace(nameProp.stringValue) ? "<no name>" : nameProp.stringValue;

            valueProp.floatValue = EditorGUI.Slider(position, new GUIContent(labelStr), valueProp.floatValue, minValueProp.floatValue, maxValueProp.floatValue);
        }
    }

    [CustomPropertyDrawer(typeof(RendererScalarProperty.PropertySettings))]
    public class RendererScalarPropertySettingsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var lengthProp = property.FindPropertyRelative("precision");
            var minValueProp = property.FindPropertyRelative("minValue");
            var maxValueProp = property.FindPropertyRelative("maxValue");

            var r1 = new Rect(position.x, position.y, position.width/3, position.height);
            var r2 = new Rect(r1.max.x, position.y, position.width/3, position.height);
            var r3 = new Rect(r2.max.x, position.y, position.width/3, position.height);

            EditorGUI.PropertyField(r1, lengthProp, label);
            EditorGUI.PropertyField(r2, minValueProp, new GUIContent(""));
            EditorGUI.PropertyField(r3, maxValueProp, new GUIContent(""));
        }
    }
}
