using UnityEditor.Rendering.BuiltIn.ShaderGraph;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
using System;

namespace UnityEditor.ShaderGraph
{
    public static class ShaderGraphUtil
    {
        internal readonly static string FirstNameShaderGraph = $"New Shader Graph Template.{ShaderGraphImporter.Extension}";

        internal static GraphData GetGraphData(Shader shader)
        {
            if (!GraphUtil.IsShaderGraphAsset(shader))
                throw new Exception("This shader is not a shader graph");

            string pathShader = AssetDatabase.GetAssetPath(shader);
            string textShader = FileUtilities.SafeReadAllText(pathShader);

            GraphData graphData = new GraphData();
            MultiJson.Deserialize(graphData, textShader);

            return graphData;
        }

        internal static void CreateShaderGraph(GraphData graphData, string customEditorGUI = "")
        {
            if (customEditorGUI != "")
            {
                foreach (var target in graphData.activeTargets)
                {
                    if (target is BuiltInTarget builtInTarget)
                    {
                        builtInTarget.customEditorGUI = customEditorGUI;
                        break;
                    }
                }
            }

            ShaderGraphTemplate shaderGraph = ScriptableObject.CreateInstance<ShaderGraphTemplate>();
            shaderGraph.GraphData = graphData;

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, shaderGraph, FirstNameShaderGraph, null, null);
        }

        public static void CreateShaderGraphTemplate(Shader shader, string customEditorGUI = "")
            => CreateShaderGraph(GetGraphData(shader), customEditorGUI);
        public static void CreateShaderGraphTemplate(string shaderName, string customEditorGUI = "")
            => CreateShaderGraphTemplate(Shader.Find(shaderName), customEditorGUI);

    }
}