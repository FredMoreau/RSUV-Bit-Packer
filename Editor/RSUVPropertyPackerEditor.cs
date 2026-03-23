using UnityEngine;
using UnityEngine.RSUVBitPacker;

namespace UnityEditor.RSUVBitPacker
{
    [CustomEditor(typeof(RSUVPropertyPacker))]
    public class RSUVPropertyPackerEditor : Editor
    {
        SerializedProperty rendererProp;
        SerializedProperty propertySheetProp;
        SerializedProperty rendererPropertiesProp;
        RendererPropertyListView rendererPropertyListView;

        RSUVPropertySheet ps => propertySheetProp.objectReferenceValue as RSUVPropertySheet;
        RSUVPropertyPacker pp => target as RSUVPropertyPacker;
        bool propertySheetMismatch => !pp.Match(ps);

        private void OnEnable()
        {
            rendererProp = serializedObject.FindProperty("_renderer");
            propertySheetProp = serializedObject.FindProperty("_propertySheet");
            rendererPropertiesProp = serializedObject.FindProperty("rendererProperties");
            rendererPropertyListView = new RendererPropertyListView(serializedObject, target);
        }

        bool showProp = true;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(propertySheetProp);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                if (propertySheetProp.objectReferenceValue != null)
                    (target as RSUVPropertyPacker).UpdadePropertyList();
            }

            if (propertySheetProp.objectReferenceValue == null)
            {
                rendererPropertyListView.DoLayoutList();
            }
            else
            {
                if (propertySheetMismatch)
                {
                    EditorGUILayout.HelpBox("The list of Renderer Properties is out of sync.", MessageType.Warning, true);
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Update", EditorStyles.miniButton, GUILayout.Width(60)))
                    {
                        Undo.RecordObject(target, "Update Renderer Properties.");
                        pp.UpdadePropertyList();
                        serializedObject.ApplyModifiedProperties();
                    }
                    GUILayout.EndHorizontal();
                }

                showProp = EditorGUILayout.Foldout(showProp, new GUIContent("Renderer Properties", "Populated from Renderer Property Sheet"));

                if (rendererPropertiesProp.arraySize > 0 && showProp)
                {
                    EditorGUI.indentLevel = 1;
                    for (int i = 0; i < rendererPropertiesProp.arraySize; i++)
                    {
                        var p = rendererPropertiesProp.GetArrayElementAtIndex(i);
                        EditorGUILayout.PropertyField(p);
                    }
                    EditorGUI.indentLevel = 0;
                }
            }

            EditorGUILayout.PropertyField(rendererProp);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
