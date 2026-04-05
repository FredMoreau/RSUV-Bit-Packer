using UnityEngine;
using UnityEngine.RSUVBitPacker;

namespace UnityEditor.RSUVBitPacker
{
    [CustomEditor(typeof(PropertyPacker)), CanEditMultipleObjects]
    public class PropertyPackerEditor : Editor
    {
        SerializedProperty rendererProp;
        SerializedProperty propertySheetProp;
        SerializedProperty rendererPropertiesProp;
        RendererPropertyListView rendererPropertyListView;

        bool propertySheetMismatch
        {
            get
            {
                foreach (PropertyPacker pp in targets)
                {
                    if (pp._propertySheet == null)
                        continue;
                    if (!pp.Match(pp._propertySheet))
                        return true;
                }
                return false;
            }
        }

        bool propertyListMismatch
        {
            get
            {
                if (targets.Length <= 1)
                    return false;
                for (int i = 1; i < targets.Length; i++)
                {
                    if (!(targets[i] as PropertyPacker).Match(targets[0] as PropertyPacker))
                        return true;
                }
                return false;
            }
        }

        private void OnEnable()
        {
            rendererProp = serializedObject.FindProperty("_renderers");
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
                    foreach (PropertyPacker pp in targets)
                        pp.UpdadePropertyList();
            }

            if (propertySheetProp.objectReferenceValue == null) // No Property Sheet assigned
            {
                if (targets.Length == 1) // single object, allow editing the list of Renderer Properties and saving it as a Property Sheet
                {
                    rendererPropertyListView.DoLayoutList();
                    SaveAsPropertySheetButton();
                }
                else // multiple objects, only show the list of Renderer Properties if they match between the selected objects, otherwise show a warning
                {
                    DrawNonReorderablePropertyList();
                }

                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            }
            else // Property Sheet assigned, show the list of Renderer Properties only if it matches the Property Sheet, otherwise show a warning
            {
                if (propertySheetMismatch)
                    UpdatePropertyListButton();

                DrawNonReorderablePropertyList();
            }

            EditorGUILayout.PropertyField(rendererProp);
            serializedObject.ApplyModifiedProperties();

            void UpdatePropertyListButton()
            {
                if (targets.Length > 1)
                    EditorGUILayout.HelpBox("Some Objects Renderer Properties do not match their Property Sheet.", MessageType.Warning, true);
                else
                    EditorGUILayout.HelpBox("The current list of Renderer Properties does not match the Property Sheet.", MessageType.Warning, true);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Update", EditorStyles.miniButton, GUILayout.Width(60)))
                {
                    Undo.RecordObjects(targets, "Update Renderer Properties.");
                    foreach (PropertyPacker pp in targets)
                        pp.UpdadePropertyList();
                    serializedObject.ApplyModifiedProperties();
                }
                GUILayout.EndHorizontal();
            }

            void DrawNonReorderablePropertyList()
            {
                if (propertyListMismatch)
                {
                    EditorGUILayout.HelpBox("The list of Renderer Properties do not match between the selected objects.", MessageType.Warning, true);
                    return;
                }

                showProp = EditorGUILayout.Foldout(showProp, new GUIContent("Renderer Properties", "Populated from Renderer Property Sheet"));

                if (rendererPropertiesProp.arraySize > 0 && showProp)
                {
                    EditorGUI.indentLevel = 1;
                    for (int i = 0; i < rendererPropertiesProp.arraySize; i++)
                    {
                        var p = rendererPropertiesProp.GetArrayElementAtIndex(i);
                        SerializedProperty name = p.FindPropertyRelative(RendererProperty<bool>.nameFieldName);
                        EditorGUILayout.PropertyField(p, new GUIContent(string.IsNullOrWhiteSpace(name.stringValue) ? "<no name>" : name.stringValue));
                    }
                    EditorGUI.indentLevel = 0;
                }
            }

            void SaveAsPropertySheetButton()
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Save as Property Sheet", EditorStyles.miniButton, GUILayout.Width(180)))
                {
                    var path = EditorUtility.SaveFilePanelInProject("Save Renderer Property Sheet", (targets[0] as PropertyPacker).name, "asset", "Save Renderer Property Sheet");
                    if (path != null)
                    {
                        (targets[0] as PropertyPacker).UpdadePropertyList();
                        var propertySheet = CreateInstance<PropertySheet>();
                        propertySheet.rendererProperties.Clear();
                        foreach (IRendererProperty property in (targets[0] as PropertyPacker).rendererProperties)
                        {
                            var clone = property.Clone();
                            propertySheet.rendererProperties.Add(clone);
                        }
                        AssetDatabase.CreateAsset(propertySheet, path);
                        Undo.RecordObject(target, "Set Property Sheet.");
                        propertySheetProp.objectReferenceValue = propertySheet;
                        serializedObject.ApplyModifiedProperties();
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
    }
}
