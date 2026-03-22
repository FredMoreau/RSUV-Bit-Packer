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

        private void OnEnable()
        {
            rendererProp = serializedObject.FindProperty("_renderer");
            propertySheetProp = serializedObject.FindProperty("_propertySheet");
            rendererPropertiesProp = serializedObject.FindProperty("rendererProperties");
            rendererPropertyListView = new RendererPropertyListView(serializedObject, target);

            var ps = propertySheetProp.objectReferenceValue as RSUVPropertySheet;
            var pp = target as RSUVPropertyPacker;
            if (!pp.Match(ps) && EditorUtility.DisplayDialog("Renderer Property Sheet", "The list of Renderer Properties is out of sync.\nDo you want to update it?", "Yes", "No"))
                pp.UpdadePropertyList();
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
