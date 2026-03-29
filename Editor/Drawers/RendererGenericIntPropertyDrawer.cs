using UnityEngine;
using UnityEngine.RSUVBitPacker;

namespace UnityEditor.RSUVBitPacker
{
    [CustomPropertyDrawer(typeof(RendererGenericIntProperty))]
    public class RendererGenericIntPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var nameProp = property.FindPropertyRelative(RendererPropertyBase.nameFieldName);
            var valueProp = property.FindPropertyRelative(RendererGenericIntProperty.valueFieldName);
            var lengthProp = property.FindPropertyRelative($"{RendererGenericIntProperty.settingsFieldName}.length");

            var maxValue = (int)Mathf.Pow(2, Mathf.Clamp((int)lengthProp.uintValue, 1, 32)) - 1;
            valueProp.intValue = EditorGUI.IntSlider(position, label, valueProp.intValue, 0, maxValue);
        }
    }
}
