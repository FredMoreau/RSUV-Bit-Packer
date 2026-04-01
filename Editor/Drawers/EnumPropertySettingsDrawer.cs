using UnityEngine;
using UnityEditorInternal;
using UnityEngine.RSUVBitPacker.RendererProperties;

namespace UnityEditor.RSUVBitPacker.RendererProperties
{
    [CustomPropertyDrawer(typeof(EnumProperty.PropertySettings))]
    public class EnumPropertySettingsDrawer : PropertyDrawer
    {
        private bool initialized = false;
        ReorderableList list;

        void Init(SerializedProperty property)
        {
            list = new ReorderableList(property.serializedObject, property, true, true, true, true);
            list.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Labels");
            };
            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element, new GUIContent($"Label {index}"));
            };
            list.onCanAddCallback = (ReorderableList l) =>
            {
                return l.count < 32;
            };
            list.onChangedCallback = (ReorderableList l) =>
            {
                property.serializedObject.ApplyModifiedProperties();
            };
            initialized = true;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!initialized)
                Init(property.FindPropertyRelative("labels"));
            list.DoLayoutList();
        }
    }
}
