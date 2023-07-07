using System.Reflection;
using System;

namespace ManaCode.ShaderGraph.Utility
{
    public static class ShaderGraphTemplateData
    {
        public const string BASE_CREATE_PATH = "Assets/Create/Shader Graph";
        public const string BASE_SHADER_NAME = "ManaCode/Shader Graph Template";

        public static string EditorGUI
        {
            get
            {
                Type type = Type.GetType("ManaCode.ShaderGraph.Utility.ShaderGraphCustomEditorGUI");
                FieldInfo fieldInfo = type?.GetField("EditorGUI");
                string editorGUI = fieldInfo?.GetValue(null) as string;
                return editorGUI ?? string.Empty;
            }
        }
    }
}