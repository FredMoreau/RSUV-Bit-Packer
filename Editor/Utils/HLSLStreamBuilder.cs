using System.Collections.Generic;
using System.Globalization;
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

        static string ReflectionFunctionHint(string functionName)
        {
            return @$"/// <funchints>
///     <sg:ProviderKey>{functionName}</sg:ProviderKey>
/// </funchints>
";
        }

        static string Indent(int level)
        {
            string indent = string.Empty;
            for (int i = 0; i < level; i++)
                indent += "    ";
            return indent;
        }

        static string ParameterDeclaration(string type, string name)
        {
            return $"{Indent(1)}out {type} {name}";
        }

        static string ParameterBlockDeclaration(List<(string type, string name)> parameters)
        {
            string paramBlock = parameters.Count > 1 ? "(\n" : "(";
            for(int i = 0; i < parameters.Count; i++)
            {
                paramBlock += $"{ParameterDeclaration(parameters[i].type, parameters[i].name)}";
                paramBlock += i < parameters.Count - 1 ? ",\n" : ")\n";
            }
            return paramBlock;
        }

        static string FunctionBody(List<string> assignments)
        {
            string funcBody = $"{Indent(1)}uint rsuv = {rsuvUniformName};\n";
            for (int i = 0; i < assignments.Count; i++)
            {
                funcBody += $"{Indent(1)}{assignments[i]}";
                funcBody += i < assignments.Count - 1 ? "\n" : "";
            }
            return funcBody;
        }

        static string FunctionBlock(string funcName, List<(string type, string name)> parameters, List<string> assignments)
        {
            string functionBlock = string.Empty;
            functionBlock += $"void {funcName}";
            functionBlock += ParameterBlockDeclaration(parameters);
            functionBlock += $"{{\n{FunctionBody(assignments)}\n}}\n\n";
            return functionBlock;
        }

        static string ShaderFunction(string funcName, List<(string type, string name)> parameters, List<string> assignments)
        {
            string shaderFunction = string.Empty;
#if UNITY_6000_5_OR_NEWER
            shaderFunction += ReflectionFunctionHint(funcName);
            shaderFunction += $"{sfrapiMacro}\n";
            shaderFunction += FunctionBlock(funcName, parameters, assignments);
#else
            shaderFunction += FunctionBlock($"{funcName}_half", parameters, assignments);
#endif
            return shaderFunction;
        }

        internal static string ShaderInclude(string name, List<RendererPropertyBase> properties, bool splitFunctions = false)
        {
            List<(string type, string name)> parameters = new();
            List<string> assignments = new();
            uint index = 0;
            foreach (RendererPropertyBase property in properties)
            {
                string paramName;
                if (string.IsNullOrWhiteSpace(property.Name))
                    paramName = $"NoName{index}";
                else
                    paramName = cultureTextInfo.ToTitleCase(property.Name).Replace(" ", "");

                parameters.Add(new(property.hlslType, paramName));
                assignments.Add(property.hlslDecoder(paramName, index));
                index++;
            }

            string shaderInclude = string.Empty;
#if UNITY_6000_5_OR_NEWER
            shaderInclude += $"{sfrapiInclude}\n\n";
#endif

            if (splitFunctions)
            {
                for(int i = 0; i < parameters.Count; i++)
                {
                    shaderInclude += ShaderFunction($"{name}_{parameters[i].name}", new() { parameters[i] }, new() { assignments[i] });
                }
            }
            else
            {
                shaderInclude += ShaderFunction(name, parameters, assignments);
            }

            return shaderInclude;
        }
    }
}
