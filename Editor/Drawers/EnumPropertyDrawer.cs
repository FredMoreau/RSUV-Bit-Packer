using UnityEngine;
using UnityEngine.RSUVBitPacker.RendererProperties;

namespace UnityEditor.RSUVBitPacker.RendererProperties
{
    [CustomPropertyDrawer(typeof(EnumProperty))]
    public class EnumPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueProp = property.FindPropertyRelative(EnumProperty.valueFieldName);
            var labelsProp = property.FindPropertyRelative($"{EnumProperty.settingsFieldName}.labels");

            string[] labels = new string[labelsProp.arraySize];

            for (int i = 0; i < labelsProp.arraySize; i++)
            {
                var labelProp = labelsProp.GetArrayElementAtIndex(i);
                labels[i] = labelProp.stringValue;
            }

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = valueProp.hasMultipleDifferentValues;
            var intValue = EditorGUI.Popup(position, label.text, valueProp.intValue, labels);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                valueProp.intValue = intValue;
            }
            EditorGUI.EndProperty();
        }
    }
}
