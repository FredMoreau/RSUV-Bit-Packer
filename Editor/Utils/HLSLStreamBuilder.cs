using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.RSUVBitPacker;

namespace UnityEditor.RSUVBitPacker
{
    // TODO namespace + path
    public static class HLSLStreamBuilder
    {
        const string sfrapiInclude = "#include \"ShaderApiReflectionSupport.hlsl\"";
        const string sfrapiMacro = "UNITY_EXPORT_REFLECTION";
        const string rsuvUniformName = "unity_RendererUserValue";
        static TextInfo cultureTextInfo = CultureInfo.CurrentCulture.TextInfo;

        static void ReflectionFunctionHint(StreamWriter streamWriter, string functionName)
        {
            streamWriter.Write(@$"/// <funchints>
///     <sg:ProviderKey>{functionName}</sg:ProviderKey>
/// </funchints>
");
        }

        static string Indent(int level)
        {
            string indent = string.Empty;
            for (int i = 0; i < level; i++)
                indent += "    ";
            return indent;
        }

        static string ParameterDeclaration(string type, string name, int indent = 1)
        {
            return $"{Indent(indent)}out {type} {name}";
        }

        static void ParameterBlockDeclaration(StreamWriter streamWriter, List<(string type, string name)> parameters)
        {
            var blockOpening = parameters.Count > 1 ? streamWriter.NewLine : string.Empty;
            streamWriter.Write($"({blockOpening}");

            for(int i = 0; i < parameters.Count; i++)
            {
                streamWriter.Write($"{ParameterDeclaration(parameters[i].type, parameters[i].name, parameters.Count > 1 ? 1 : 0)}");
                streamWriter.Write(i < parameters.Count - 1 ? $",{streamWriter.NewLine}" : $"){streamWriter.NewLine}");
            }
        }

        static void FunctionBody(StreamWriter streamWriter, List<string> assignments)
        {
            streamWriter.WriteLine("{");
            streamWriter.WriteLine($"{Indent(1)}uint rsuv = {rsuvUniformName};");
            for (int i = 0; i < assignments.Count; i++)
            {
                streamWriter.Write($"{Indent(1)}{assignments[i]}{streamWriter.NewLine}");
            }
            streamWriter.WriteLine("}");
            streamWriter.WriteLine(string.Empty);
        }

        static void FunctionBlock(StreamWriter streamWriter, string funcName, List<(string type, string name)> parameters, List<string> assignments)
        {
            streamWriter.Write($"void {funcName}");
            ParameterBlockDeclaration(streamWriter, parameters);
            FunctionBody(streamWriter, assignments);
        }

        static void ShaderFunction(StreamWriter streamWriter, string funcName, List<(string type, string name)> parameters, List<string> assignments)
        {
#if UNITY_6000_5_OR_NEWER
            ReflectionFunctionHint(streamWriter, funcName);
            streamWriter.WriteLine(sfrapiMacro);
            FunctionBlock(streamWriter, funcName, parameters, assignments);
#else
            FunctionBlock(streamWriter, $"{funcName}_half", parameters, assignments);
#endif
        }

        internal static void ShaderInclude(StreamWriter streamWriter, string name, List<RendererPropertyBase> properties, bool splitFunctions = false)
        {
            List<(string type, string name)> parameters = new();
            List<string> assignments = new();
            uint index = 0;
            uint offset = 0;
            foreach (RendererPropertyBase property in properties)
            {
                string paramName;
                if (string.IsNullOrWhiteSpace(property.Name))
                    paramName = $"NoName{index}";
                else
                    paramName = cultureTextInfo.ToTitleCase(property.Name).Replace(" ", "");

                parameters.Add(new(property.hlslType, paramName));
                assignments.Add(property.hlslDecoder(paramName, offset));
                index++;
                offset += property.Length;
            }

            using (streamWriter)
            {
                streamWriter.NewLine = Environment.NewLine;

#if UNITY_6000_5_OR_NEWER
                streamWriter.WriteLine($"{sfrapiInclude}{streamWriter.NewLine}");
#endif

                if (splitFunctions)
                {
                    for(int i = 0; i < parameters.Count; i++)
                    {
                        ShaderFunction(streamWriter, $"{name}_{parameters[i].name}", new() { parameters[i] }, new() { assignments[i] });
                    }
                }
                else
                {
                    ShaderFunction(streamWriter, name, parameters, assignments);
                }
            }
        }
    }
}
