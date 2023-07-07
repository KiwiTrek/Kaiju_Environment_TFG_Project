using ManaCode.ShaderGraph.Utility;
using UnityEditor.ShaderGraph;

namespace ManaCode.HighDefinitionRP.Utility
{
    public class ShaderGraphTemplateHDRP
    {
        public static void Create(string shaderName)
            => ShaderGraphHighDefinitionUtil.CreateShaderGraphTemplate(shaderName, ShaderGraphTemplateData.EditorGUI);
    }
}