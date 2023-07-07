using UnityEditor;

namespace ManaCode.ShaderGraph.Editor
{
#if HDRP_12_OR_NEWER
    using static UnityEditor.MenuItemsPriority;
    using static HighDefinitionRP.Utility.ShaderGraphTemplateHDRP;
    using static Utility.ShaderGraphTemplateData;
#endif

    internal static class CreateShaderGraphTemplateHDRP
    {
#if HDRP_12_OR_NEWER
        // BASE PATH
        private const string PATH_CREATE = BASE_CREATE_PATH + "/HDRP Template";
        private const string SHADER_NAME = BASE_SHADER_NAME + "/HDRP";

        // TEST PATH
        private const string PATH_CREATE_TEST = PATH_CREATE + "/Test Shader Graph";
        private const string TEST_SHADER = SHADER_NAME + "/Test";

        [MenuItem(PATH_CREATE_TEST, priority = FIRST_SECTION)]
        private static void CreateTestShaderGraph() => Create(TEST_SHADER);
#endif
    }
}