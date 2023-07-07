using UnityEditor.Rendering.Universal.ShaderGraph;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    using static ShaderGraphUtil;

    public static class ShaderGraphUniversalUtil
    {
        public static void CreateShaderGraphTemplate(string shaderName, string customEditorGUI = "")
            => CreateShaderGraphTemplate(Shader.Find(shaderName), customEditorGUI);
        public static void CreateShaderGraphTemplate(Shader shader, string customEditorGUI = "")
        {
            GraphData graphData = GetGraphData(shader);

            if (customEditorGUI != "")
            {
                foreach (var target in graphData.activeTargets)
                {
                    if (target is UniversalTarget universalTarget)
                    {
                        universalTarget.customEditorGUI = customEditorGUI;
                        break;
                    }
                }
            }

            CreateShaderGraph(graphData, customEditorGUI);
        }
    }
}