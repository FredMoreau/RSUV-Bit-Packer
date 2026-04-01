using UnityEngine;
using UnityEngine.RSUVBitPacker;

namespace UnityEditor.RSUVBitPacker
{
    /// <summary>
    /// Custom drawer for <see cref="HlslPrecision"/> that exposes only Half and Float in the inspector.
    /// Other enum values remain valid programmatically but are not shown in the popup.
    /// </summary>
    [CustomPropertyDrawer(typeof(HlslPrecision))]
    public class HlslPrecisionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (property.propertyType != SerializedPropertyType.Enum)
            {
                EditorGUI.LabelField(position, label, "Use HlslPrecision with enum");
                EditorGUI.EndProperty();
                return;
            }

            // Only show Half and Float options in the popup
            GUIContent[] options = new GUIContent[] { new GUIContent(HlslPrecision.Half.ToString()), new GUIContent(HlslPrecision.Float.ToString()) };

            // Determine current selection index (0 => Half, 1 => Float)
            int currentEnumIndex = property.enumValueIndex;
            int currentIndex = (currentEnumIndex == (int)HlslPrecision.Float) ? 1 : 0;

            int newIndex = EditorGUI.Popup(position, label, currentIndex, options);

            HlslPrecision newValue = (newIndex == 1) ? HlslPrecision.Float : HlslPrecision.Half;
            property.enumValueIndex = (int)newValue;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
