using System.Collections.Generic;
using System.Globalization;
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

        private void OnEnable()
        {
            rendererPropertyListView = new RendererPropertyListView(serializedObject, target);
            rendererPropertyListView.OnChangeCallback += () =>
            {
                Debug.Log((target as RSUVPropertySheet).rendererProperties.Count);
                UpdateShaderInclude();
            };
            shaderIncludeProp = serializedObject.FindProperty("shaderInclude");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            rendererPropertyListView.DoLayoutList();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(shaderIncludeProp);
            if (GUILayout.Button(new GUIContent("NEW")))
                CreateShaderInclude();
            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }

        void CreateShaderInclude()
        {
            var assetPath = AssetDatabase.GetAssetPath(target).Replace(".asset", ".hlsl");
            //AssetDatabase.CreateAsset(new TextAsset(), assetPath);
            using (StreamWriter sw = File.CreateText(assetPath))
            {
                
            }
            AssetDatabase.Refresh(ImportAssetOptions.Default);
            ShaderInclude incl = AssetDatabase.LoadAssetAtPath<ShaderInclude>(assetPath);
            shaderIncludeProp.objectReferenceValue = incl;
        }

        void UpdateShaderInclude()
        {
            //serializedObject.ApplyModifiedProperties();
            //serializedObject.Update();
            ShaderInclude shaderInclude = (ShaderInclude)shaderIncludeProp.objectReferenceValue;
            if (shaderInclude != null)
            {
                var path = AssetDatabase.GetAssetPath(shaderInclude);
                var name = shaderInclude.name;

                var rendererProperties = (target as RSUVPropertySheet).rendererProperties;
                //Debug.Log(rendererProperties.Count);

                TextInfo info = CultureInfo.CurrentCulture.TextInfo;
                List<(string type, string name)> parameters = new();
                foreach (RendererPropertyBase prop in rendererProperties)
                {
                    string paramName;
                    if (string.IsNullOrWhiteSpace(prop.Name))
                        paramName = $"Empty{rendererProperties.IndexOf(prop)}";
                    else
                        paramName = info.ToTitleCase(prop.Name).Replace(" ", "");
                    parameters.Add(new(prop.hlslType, paramName));
                }

                string parametersStr = "\n";
                for (int p = 0; p < parameters.Count; p++)
                {
                    parametersStr += $"out {parameters[p].type} {parameters[p].name}";
                    if (p < parameters.Count - 1)
                        parametersStr += ",\n";
                }

                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine("#include \"ShaderApiReflectionSupport.hlsl\"\n");
                    string functionHints = @$"/// <funchints>
///     <sg:ProviderKey>{name}</sg:ProviderKey>
/// </funchints>
";
                    sw.Write(functionHints);
                    sw.WriteLine("UNITY_EXPORT_REFLECTION");
                    string functionSignature = $"void {name}({parametersStr})";
                    sw.WriteLine(functionSignature);
                    sw.WriteLine("{");
                    sw.WriteLine("    uint rsuv = unity_RendererUserValue;");
                    uint offset = 0;
                    for (int i = 0; i < rendererProperties.Count; i++)
                    {
                        var prop = rendererProperties[i];
                        sw.WriteLine($"    {prop.hlslDecoder(parameters[i].name, offset)}");
                        offset += prop.Length;
                    }
                    sw.WriteLine("}");
                }

                AssetDatabase.Refresh();
            }
        }
    }
}
