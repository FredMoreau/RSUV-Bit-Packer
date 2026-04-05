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
            var valueProp = property.FindPropertyRelative(IntegerProperty.valueFieldName);
            var minValueProp = property.FindPropertyRelative($"{IntegerProperty.settingsFieldName}.minValue");
            var maxValueProp = property.FindPropertyRelative($"{IntegerProperty.settingsFieldName}.maxValue");

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = valueProp.hasMultipleDifferentValues;
            var floatValue = EditorGUI.Slider(position, label, valueProp.floatValue, minValueProp.floatValue, maxValueProp.floatValue);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                valueProp.floatValue = floatValue;
            }
            EditorGUI.EndProperty();
        }
    }
}
