using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.RSUVBitPacker;

namespace UnityEditor.RSUVBitPacker
{
    public static class HLSLStreamBuilder
    {
        const string sfrapiInclude = "#include \"ShaderApiReflectionSupport.hlsl\"";
        const string sfrapiMacro = "UNITY_EXPORT_REFLECTION";
        const string rsuvUniformName = "unity_RendererUserValue";
        static TextInfo cultureTextInfo = CultureInfo.CurrentCulture.TextInfo;

        static void ReflectionFunctionHint(StreamWriter streamWriter, string functionName, string nameSpace = null)
        {
            var nameSpacePrefix = string.IsNullOrEmpty(nameSpace) ? string.Empty : $"{nameSpace}.";
            var name = functionName.Split('_');
            var nodeName = name.Length > 1 ? name[1] : name[0];
            var nodePath = name.Length > 1 ? $"/{name[0]}" : string.Empty;

            streamWriter.Write(@$"/// <funchints>
///     <sg:ProviderKey>{nameSpacePrefix}{functionName.Replace('_', '.')}</sg:ProviderKey>
///     <sg:DisplayName>{nodeName}</sg:DisplayName>
///     <sg:SearchCategory>Input/Property Sheets{nodePath}</sg:SearchCategory>
///     <sg:SearchName>{nodeName}</sg:SearchName>
///     <sg:SearchTerms>RSUV, {functionName.Replace("_", ", ")}</sg:SearchTerms>
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

        static string ParameterDeclaration(string type, string name, string modifier = "out", int indent = 1)
        {
            if (string.IsNullOrEmpty(modifier))
                return $"{Indent(indent)}{type} {name}";
            else
                return $"{Indent(indent)}{modifier} {type} {name}";
        }

        static void ParameterBlockDeclaration(StreamWriter streamWriter, List<(string type, string name, string modifier)> parameters)
        {
            var blockOpening = parameters.Count > 1 ? streamWriter.NewLine : string.Empty;
            streamWriter.Write($"({blockOpening}");

            for(int i = 0; i < parameters.Count; i++)
            {
                streamWriter.Write($"{ParameterDeclaration(parameters[i].type, parameters[i].name, modifier:parameters[i].modifier, indent:parameters.Count > 1 ? 1 : 0)}");
                streamWriter.Write(i < parameters.Count - 1 ? $",{streamWriter.NewLine}" : $"){streamWriter.NewLine}");
            }
        }

        static void FunctionBody(StreamWriter streamWriter, List<string> assignments)
        {
            streamWriter.WriteLine("{");
            for (int i = 0; i < assignments.Count; i++)
            {
                streamWriter.Write($"{Indent(1)}{assignments[i]}{streamWriter.NewLine}");
            }
            streamWriter.WriteLine("}");
            streamWriter.WriteLine(string.Empty);
        }

        static void FunctionBlock(StreamWriter streamWriter, string funcName, List<(string type, string name, string modifier)> parameters, List<string> assignments)
        {
            streamWriter.Write($"void {funcName}");
            ParameterBlockDeclaration(streamWriter, parameters);
            FunctionBody(streamWriter, assignments);
        }

        static void ShaderFunction(StreamWriter streamWriter, string funcName, List<(string type, string name, string modifier)> parameters, List<string> assignments, string nameSpace = null)
        {
#if UNITY_6000_5_OR_NEWER
            ReflectionFunctionHint(streamWriter, funcName, nameSpace);
            streamWriter.WriteLine(sfrapiMacro);
            FunctionBlock(streamWriter, funcName, parameters, assignments);
#else
            FunctionBlock(streamWriter, $"{funcName}_half", parameters, assignments);
#endif
        }

        internal static void ShaderInclude(StreamWriter streamWriter, string name, List<IRendererProperty> properties, bool splitFunctions = false, string nameSpace = null, uint shaderGraphPreviewValue = 0)
        {
            List<(string type, string name, string modifier)> parameters = new();
            List<string> assignments = new();
            uint index = 0;
            uint offset = 0;
            foreach (IRendererProperty property in properties)
            {
                if (offset + property.Length > 32)
                    break;

                if (string.IsNullOrEmpty(property.HlslType))
                    continue;

                string paramName;
                if (string.IsNullOrWhiteSpace(property.Name))
                    paramName = $"NoName{index}";
                else
                    paramName = cultureTextInfo.ToTitleCase(property.Name).Replace(" ", "");

                parameters.Add(new(property.HlslType, paramName, "out"));
                assignments.Add(property.HlslDecoder(paramName, offset));
                index++;
                offset += property.Length;
            }

            using (streamWriter)
            {
                streamWriter.NewLine = Environment.NewLine;

#if UNITY_6000_5_OR_NEWER
                streamWriter.WriteLine($"{sfrapiInclude}{streamWriter.NewLine}");
#endif
                streamWriter.Write(@$"#ifndef RSUV
#if defined (SHADERGRAPH_PREVIEW) || defined (SHADERGRAPH_PREVIEW_MAIN)
#define RSUV {shaderGraphPreviewValue}
#else
#define RSUV {rsuvUniformName}
#endif
#endif

");

#if UNITY_6000_5_OR_NEWER // HLSL namespaces are not supported in Custom Function Nodes
                if (!string.IsNullOrEmpty(nameSpace))
                {
                    streamWriter.WriteLine($"namespace {nameSpace.Replace('.', '_')}"); // HLSL namespaces don't support dots, replace with underscores
                    streamWriter.WriteLine("{");
                }
#endif

                if (parameters.Count == 0)
                {
                    streamWriter.WriteLine($"// No valid properties found for {name}");
                }
                else
                {
                    if (splitFunctions)
                    {
                        for(int i = 0; i < parameters.Count; i++)
                        {
                            ShaderFunction(streamWriter, $"{name}_{parameters[i].name}", new() { parameters[i] }, new() { assignments[i] }, nameSpace);
                        }
                    }
                    else
                    {
                        ShaderFunction(streamWriter, name, parameters, assignments, nameSpace);
                    }
                }

#if UNITY_6000_5_OR_NEWER
                if (!string.IsNullOrEmpty(nameSpace))
                    streamWriter.WriteLine("}");
#endif
            }
        }

        internal static void ShaderInclude(StreamWriter streamWriter, string name, ColorPalette colorPalette)
        {
            if (colorPalette == null)
                return;

            List<(string type, string name, string modifier)> parameters = new();
            List<string> assignments = new();

            parameters.Add(("uint", "Index", ""));
            parameters.Add(("float4", "Color", "out"));

            StringBuilder hlslBody = new StringBuilder();
            hlslBody.AppendLine("[branch] switch(Index)");
            hlslBody.AppendLine("    {");
            for (int i = 0; i < colorPalette.Count; i++)
            {
                var c = colorPalette[i];
                hlslBody.Append(@$"        case {i}:
            Color = float4({c.r}, {c.g}, {c.b}, {c.a});
            break;
");
            }
            hlslBody.Append(@$"        default:
            Color = float4(0,0,0,1);
            break;
");
            hlslBody.Append("    }");

            assignments.Add(hlslBody.ToString());

            using (streamWriter)
            {
                streamWriter.NewLine = Environment.NewLine;

#if UNITY_6000_5_OR_NEWER
                streamWriter.WriteLine($"{sfrapiInclude}{streamWriter.NewLine}");
#endif

                ShaderFunction(streamWriter, name, parameters, assignments);
            }
        }
    }
}
