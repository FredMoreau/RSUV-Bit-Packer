using UnityEngine;
using UnityEngine.RSUVBitPacker;
using UnityEngine.RSUVBitPacker.RendererProperties;

namespace UnityEditor.RSUVBitPacker.RendererProperties
{
    [CustomPropertyDrawer(typeof(ScalarProperty))]
    public class ScalarPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var nameProp = property.FindPropertyRelative(RendererPropertyBase.nameFieldName);
            var valueProp = property.FindPropertyRelative(IntegerProperty.valueFieldName);
            var lengthProp = property.FindPropertyRelative($"{IntegerProperty.settingsFieldName}.precision");
            var minValueProp = property.FindPropertyRelative($"{IntegerProperty.settingsFieldName}.minValue");
            var maxValueProp = property.FindPropertyRelative($"{IntegerProperty.settingsFieldName}.maxValue");

            valueProp.floatValue = EditorGUI.Slider(position, label, valueProp.floatValue, minValueProp.floatValue, maxValueProp.floatValue);
        }
    }
}
