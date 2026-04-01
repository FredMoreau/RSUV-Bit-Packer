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
            valueProp.intValue = EditorGUI.IntSlider(position, label, valueProp.intValue, 0, maxValue);
        }
    }
}
