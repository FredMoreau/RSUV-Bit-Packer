using System.IO;
using UnityEngine;
using UnityEngine.RSUVBitPacker;

namespace UnityEditor.RSUVBitPacker
{
    [CustomEditor(typeof(RSUVPropertySheet))]
    public class RSUVPropertySheetEditor : Editor
    {
        RendererPropertyListView rendererPropertyListView;

        SerializedProperty shaderIncludeProp;
        [SerializeField] bool splitFunctions; // TODO store in meta

        private void OnEnable()
        {
            rendererPropertyListView = new RendererPropertyListView(serializedObject, target);
            rendererPropertyListView.OnChangeCallback += () =>
            {
                UpdateShaderInclude();
            };
            shaderIncludeProp = serializedObject.FindProperty("shaderInclude");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            rendererPropertyListView.DoLayoutList();
            EditorGUILayout.PropertyField(shaderIncludeProp);

            EditorGUILayout.BeginHorizontal();
            splitFunctions = GUILayout.Toggle(splitFunctions, new GUIContent("Split Functions", "Shall Properties be split in individual functions."));
            if (shaderIncludeProp.objectReferenceValue == null && GUILayout.Button(new GUIContent("NEW")))
                CreateShaderInclude();
            else if (GUILayout.Button(new GUIContent("Update")))
                UpdateShaderInclude();
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        void CreateShaderInclude()
        {
            var assetPath = AssetDatabase.GetAssetPath(target).Replace(".asset", ".hlsl");
            var rendererProperties = (target as RSUVPropertySheet).rendererProperties;

            using (StreamWriter sw = File.CreateText(assetPath))
            {
                sw.NewLine = "\n";
                string include = HLSLStreamBuilder.ShaderInclude(name, rendererProperties, splitFunctions);
                sw.Write(include);
            }
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

                var rendererProperties = (target as RSUVPropertySheet).rendererProperties;

                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.NewLine = "\n";
                    string include = HLSLStreamBuilder.ShaderInclude(name, rendererProperties, splitFunctions);
                    sw.Write(include);
                }

                AssetDatabase.Refresh();
            }
        }
    }
}
