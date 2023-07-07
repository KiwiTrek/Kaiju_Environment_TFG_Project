using UnityEngine.Rendering;
using UnityEditor;

namespace ManaCode.ShaderGraph.Editor
{
    using static UnityEditor.MenuItemsPriority;
    using static Utility.ShaderGraphTemplateBuiltIn;
    using static Utility.ShaderGraphTemplateData;

    internal static class CreateShaderGraphTemplateBuiltIn
    {
        // BASE PATH
        private const string PATH_CREATE = BASE_CREATE_PATH + "/BuiltIn Template";
        private const string SHADER_NAME = BASE_SHADER_NAME + "/BuiltIn";

        //PRIORITY
        private const int BUILTIN_PRIORITY =
#if URP_12_OR_NEWER || HDRP_12_OR_NEWER
            LAST_SECTION;
#else
            SECTION_1;
#endif

        // 2D PATH
        private const string PATH_CREATE_SPRITE = PATH_CREATE + "/Sprite Shader Graph";
        private const string SPRITE_SHADER = SHADER_NAME + "/Sprite";

        // 3D PATH
        private const string PATH_CREATE_LIT_METALLIC = PATH_CREATE + "/Lit Metallic Shader Graph";
        private const string LIT_METALLIC_SHADER = SHADER_NAME + "/LitMetallic";

        private const string PATH_CREATE_LIT_SPECULAR = PATH_CREATE + "/Lit Specular Shader Graph";
        private const string LIT_SPECULAR_SHADER = SHADER_NAME + "/LitSpecular";

        private const string PATH_CREATE_UNLIT = PATH_CREATE + "/Unlit Shader Graph";
        private const string UNLIT_SHADER = SHADER_NAME + "/Unlit";

        // CREATE 2D ITEMS
        [MenuItem(PATH_CREATE_SPRITE, priority =
             CoreUtils.Priorities.assetsCreateShaderMenuPriority + SECTION_2 + BUILTIN_PRIORITY)]
        private static void CreateSpriteShaderGraph() => Create(SPRITE_SHADER);

        //CREATE 3D ITEMS
        [MenuItem(PATH_CREATE_LIT_METALLIC, priority =
             CoreUtils.Priorities.assetsCreateShaderMenuPriority + SECTION_1 + BUILTIN_PRIORITY)]
        private static void CreateLitMetalicShaderGraph() => Create(LIT_METALLIC_SHADER);

        [MenuItem(PATH_CREATE_LIT_SPECULAR, priority =
             CoreUtils.Priorities.assetsCreateShaderMenuPriority + SECTION_1 + BUILTIN_PRIORITY)]
        private static void CreateLitSpecularShaderGraph() => Create(LIT_SPECULAR_SHADER);

        [MenuItem(PATH_CREATE_UNLIT, priority = 
            CoreUtils.Priorities.assetsCreateShaderMenuPriority + SECTION_1 + BUILTIN_PRIORITY)]
        private static void CreateUnlitShaderGraph() => Create(UNLIT_SHADER);

    }
}