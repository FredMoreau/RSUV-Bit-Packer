using UnityEngine;
using UnityEngine.RSUVBitPacker;

namespace UnityEditor.RSUVBitPacker
{
    [CustomPropertyDrawer(typeof(RendererPropertyBase))]
    public class RendererPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var nameProp = property.FindPropertyRelative("name");
            var valueProp = property.FindPropertyRelative("_value");

            var labelStr = string.IsNullOrWhiteSpace(nameProp.stringValue) ? "<no name>" : nameProp.stringValue;

            EditorGUI.PropertyField(position, valueProp, new GUIContent(labelStr));
        }
    }
}
