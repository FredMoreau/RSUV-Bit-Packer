using UnityEngine;
using UnityEngine.RSUVBitPacker.RendererProperties;

namespace UnityEditor.RSUVBitPacker.RendererProperties
{
    [CustomPropertyDrawer(typeof(IntegerProperty))]
    public class IntegerPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueProp = property.FindPropertyRelative(IntegerProperty.valueFieldName);
            var lengthProp = property.FindPropertyRelative($"{IntegerProperty.settingsFieldName}.length");

            var maxValue = (int)Mathf.Pow(2, Mathf.Clamp((int)lengthProp.uintValue, 1, 32)) - 1;
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = valueProp.hasMultipleDifferentValues;
            var intValue = EditorGUI.IntSlider(position, label, valueProp.intValue, 0, maxValue);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                valueProp.intValue = intValue;
            }
            EditorGUI.EndProperty();
        }
    }
}
