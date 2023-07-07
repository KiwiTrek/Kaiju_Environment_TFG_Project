using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    public class ShaderGraphTemplate : EndNameEditAction
    {
        internal static readonly string ShaderPath = "Shader Graphs";

        private GraphData _graphData;

        internal GraphData GraphData 
        { 
            get => _graphData ?? new GraphData(); 
            set => _graphData = value; 
        }

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            GraphData.path = ShaderPath;
            FileUtilities.WriteShaderGraphToDisk(pathName, _graphData);
            AssetDatabase.Refresh();

            Object unityObject = AssetDatabase.LoadAssetAtPath<Shader>(pathName);
            Selection.activeObject = unityObject;
        }
    }
}