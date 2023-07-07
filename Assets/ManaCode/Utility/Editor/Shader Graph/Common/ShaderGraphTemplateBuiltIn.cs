using UnityEditor.ShaderGraph;

namespace ManaCode.ShaderGraph.Utility
{
    public static class ShaderGraphTemplateBuiltIn
    {
        public static void Create(string shaderName) 
            => ShaderGraphUtil.CreateShaderGraphTemplate(shaderName, ShaderGraphTemplateData.EditorGUI);
    }
}