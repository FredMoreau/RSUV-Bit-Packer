using UnityEngine;
using UnityEngine.RSUVBitPacker;

namespace UnityEditor.RSUVBitPacker
{
    //[CustomPropertyDrawer(typeof(RendererHSVProperty))]
    //public class RendererHSVPropertyDrawer : PropertyDrawer
    //{
    //    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //    {
    //        var nameProp = property.FindPropertyRelative(RendererPropertyBase.nameFieldName);
    //        var valueProp = property.FindPropertyRelative(RendererHSVProperty.valueFieldName);

    //        var labelStr = string.IsNullOrWhiteSpace(nameProp.stringValue) ? "<no name>" : nameProp.stringValue;

    //        EditorGUI.PropertyField(position, valueProp, new GUIContent(labelStr));
    //    }
    //}

    [CustomPropertyDrawer(typeof(RendererHSVProperty.PropertySettings))]
    public class RendererHSVPropertySettingsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var lengthProp = property.FindPropertyRelative("precision3");

            EditorGUI.PropertyField(position, lengthProp, new GUIContent(""));
        }
    }
}
