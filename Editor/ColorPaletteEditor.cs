using System.IO;
using UnityEngine;
using UnityEngine.RSUVBitPacker;

namespace UnityEditor.RSUVBitPacker
{
    [CustomEditor(typeof(ColorPalette))]
    public class ColorPaletteEditor : Editor
    {
        SerializedProperty colorsProp;
        SerializedProperty shaderIncludeProp;

        private void OnEnable()
        {
            colorsProp = serializedObject.FindProperty("colors");
            shaderIncludeProp = serializedObject.FindProperty("shaderInclude");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(colorsProp, new GUIContent("Colors", "The Palette Colors."));
            EditorGUILayout.PropertyField(shaderIncludeProp);
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
            serializedObject.ApplyModifiedProperties();
        }

        void CreateShaderInclude()
        {
            var assetPath = AssetDatabase.GetAssetPath(target).Replace(".asset", ".hlsl");
            var colorPalette = target as ColorPalette;

            HLSLStreamBuilder.ShaderInclude(File.CreateText(assetPath), target.name, colorPalette);

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
                var colorPalette = target as ColorPalette;

                HLSLStreamBuilder.ShaderInclude(new StreamWriter(path), name, colorPalette);

                AssetDatabase.Refresh();
            }
        }
    }
}
