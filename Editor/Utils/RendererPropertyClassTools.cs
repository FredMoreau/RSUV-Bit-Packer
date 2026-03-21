
namespace UnityEditor.RSUVBitPacker
{
    public static class RendererPropertyClassTools
    {
        const string TemplatesRoot = "Packages/com.unity.rsuv-bit-packer/Editor/Templates";

        [MenuItem("Assets/Create/Scripting/RendererProperty")]
        static void CreateRendererPropertyClassFromTemplate()
        {
            CreateNewFromTemplate("RendererPropertyTemplate", "NewRendererProperty.cs");
        }

        public static void CreateNewFromTemplate(string template, string filename)
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile($"{TemplatesRoot}/{template}.txt", filename);
        }
    }
}
