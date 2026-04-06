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
            (target as IRendererProperties).SanitizeProperties();

            rendererPropertyListView = new RendererPropertyListView(serializedObject, target);
            // disabling live update for now, until we have a reliable way to get the updated object.
            //rendererPropertyListView.OnChangeCallback += () =>
            //{
            //    UpdateShaderInclude();
            //};
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

            var projectNamespace = GetNamespace(assetPath);
            HLSLStreamBuilder.ShaderInclude(File.CreateText(assetPath), target.name, rendererProperties, splitFunctionsProp.boolValue, projectNamespace);

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

                var projectNamespace = GetNamespace(path);
                HLSLStreamBuilder.ShaderInclude(new StreamWriter(path), name, rendererProperties, splitFunctionsProp.boolValue, projectNamespace);

                AssetDatabase.Refresh();
            }
        }

        struct AsmdefData
        {
            public string name;
            public string rootNamespace;
        }

        public static string GetNamespace(string assetPath)
        {
            var asmdefPath = GetAsmdefPath(assetPath);
            if (asmdefPath != null)
            {
                var json = File.ReadAllText(asmdefPath);
                var data = JsonUtility.FromJson<AsmdefData>(json);
                if (!string.IsNullOrEmpty(data.rootNamespace))
                    return data.rootNamespace;
            }
            return EditorSettings.projectGenerationRootNamespace;
        }

        public static string GetAsmdefPath(string assetPath)
        {
            var fullPath = Path.GetFullPath(assetPath);
            var directory = Path.GetDirectoryName(fullPath);

            while (!string.IsNullOrEmpty(directory))
            {
                var asmdefs = Directory.GetFiles(directory, "*.asmdef", SearchOption.TopDirectoryOnly);

                if (asmdefs.Length > 0)
                    return asmdefs[0];

                directory = Directory.GetParent(directory)?.FullName;
            }

            return null;
        }
    }
}
