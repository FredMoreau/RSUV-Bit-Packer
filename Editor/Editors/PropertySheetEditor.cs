using System.IO;
using UnityEngine;
using UnityEngine.RSUVBitPacker;

namespace UnityEditor.RSUVBitPacker
{
    [CustomEditor(typeof(PropertySheet))]
    public class PropertySheetEditor : Editor
    {
        RendererPropertyListView rendererPropertyListView;

        SerializedProperty shaderIncludeProp;
        SerializedProperty splitFunctionsProp;

        private void OnEnable()
        {
            rendererPropertyListView = new RendererPropertyListView(serializedObject, target);
            rendererPropertyListView.OnChangeCallback += () =>
            {
                UpdateShaderInclude();
            };
            shaderIncludeProp = serializedObject.FindProperty("shaderInclude");
            splitFunctionsProp = serializedObject.FindProperty("splitFunctions");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // TODO: live update
            //EditorGUI.BeginChangeCheck();
            rendererPropertyListView.DoLayoutList();
            //if (EditorGUI.EndChangeCheck())
            //{
            //    serializedObject.ApplyModifiedProperties();
            //    UpdateShaderInclude();
            //}
            
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            EditorGUILayout.PropertyField(shaderIncludeProp);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(splitFunctionsProp, new GUIContent("Split Functions", "Shall Properties be split in individual functions."));
            if (shaderIncludeProp.objectReferenceValue == null)
            {
                if (GUILayout.Button(new GUIContent("Create")))
                    CreateShaderInclude();
            }
            else
            {
                if (GUILayout.Button(new GUIContent("Update")))
                    UpdateShaderInclude();
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        void CreateShaderInclude()
        {
            var assetPath = AssetDatabase.GetAssetPath(target).Replace(".asset", ".hlsl");
            var rendererProperties = (target as PropertySheet).rendererProperties;

            HLSLStreamBuilder.ShaderInclude(File.CreateText(assetPath), target.name, rendererProperties, splitFunctionsProp.boolValue);

            AssetDatabase.Refresh(ImportAssetOptions.Default);
            ShaderInclude incl = AssetDatabase.LoadAssetAtPath<ShaderInclude>(assetPath);
            shaderIncludeProp.objectReferenceValue = incl;
        }

        void UpdateShaderInclude()
        {
            ShaderInclude shaderInclude = (ShaderInclude)shaderIncludeProp.objectReferenceValue;
            if (shaderInclude != null)
            {
                var path = AssetDatabase.GetAssetPath(shaderInclude);
                var name = shaderInclude.name;

                var rendererProperties = (target as PropertySheet).rendererProperties;

                HLSLStreamBuilder.ShaderInclude(new StreamWriter(path), name, rendererProperties, splitFunctionsProp.boolValue);

                AssetDatabase.Refresh();
            }
        }
    }
}
