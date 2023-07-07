using UnityEngine.Rendering;
using UnityEditor;

namespace ManaCode.ShaderGraph.Editor
{
#if URP_12_OR_NEWER
    using static UnityEditor.MenuItemsPriority;
    using static UniversalRP.Utility.ShaderGraphTemplateURP;
    using static Utility.ShaderGraphTemplateData;
#endif

    internal static class CreateShaderGraphTemplateURP
    {
#if URP_12_OR_NEWER
        // BASE PATH
        private const string PATH_CREATE = BASE_CREATE_PATH + "/URP Template";
        private const string SHADER_NAME = BASE_SHADER_NAME + "/URP";

        //PRIORITY
        private const int URP_PRIORITY = 2000;

        // 2D PATH
        private const string PATH_CREATE_CUSTOM_SPRITE_LIT = PATH_CREATE + "/Sprite Custom Lit Shader Graph";
        private const string SPRITE_CUSTOM_LIT_SHADER = SHADER_NAME + "/SpriteCustomLit";

        private const string PATH_CREATE_SPRITE_LIT = PATH_CREATE + "/Sprite Lit Shader Graph";
        private const string SPRITE_LIT_SHADER = SHADER_NAME + "/SpriteLit";

        private const string PATH_CREATE_SPRITE_UNLIT = PATH_CREATE + "/Sprite Unlit Shader Graph";
        private const string SPRITE_UNLIT_SHADER = SHADER_NAME + "/SpriteUnlit";

        // 3D PATH
        private const string PATH_CREATE_DECAL = PATH_CREATE + "/Decal Shader Graph";
        private const string DECAL_SHADER = SHADER_NAME + "/Decal";

        private const string PATH_CREATE_LIT = PATH_CREATE + "/Lit Shader Graph";
        private const string LIT_SHADER = SHADER_NAME + "/Lit";

        private const string PATH_CREATE_LIT_METALLIC = PATH_CREATE + "/Lit Metallic Shader Graph";
        private const string LIT_METALLIC_SHADER = SHADER_NAME + "/LitMetallic";

        private const string PATH_CREATE_LIT_SPECULAR = PATH_CREATE + "/Lit Specular Shader Graph";
        private const string LIT_SPECULAR_SHADER = SHADER_NAME + "/LitSpecular";

        private const string PATH_CREATE_UNLIT = PATH_CREATE + "/Unlit Shader Graph";
        private const string UNLIT_SHADER = SHADER_NAME + "/Unlit";

        // CREATE 2D ITEMS
        [MenuItem(PATH_CREATE_CUSTOM_SPRITE_LIT, priority =
             CoreUtils.Priorities.assetsCreateShaderMenuPriority + SECTION_3 + URP_PRIORITY)]
        private static void CreateCustomSpriteLitShaderGraph() => Create(SPRITE_CUSTOM_LIT_SHADER);

        [MenuItem(PATH_CREATE_SPRITE_LIT, priority = 
             CoreUtils.Priorities.assetsCreateShaderMenuPriority + SECTION_3 + URP_PRIORITY)]
        private static void CreateSpriteLitShaderGraph() => Create(SPRITE_LIT_SHADER);

        [MenuItem(PATH_CREATE_SPRITE_UNLIT, priority = 
             CoreUtils.Priorities.assetsCreateShaderMenuPriority + SECTION_3 + URP_PRIORITY)]
        private static void CreateSpriteUnlitShaderGraph() => Create(SPRITE_UNLIT_SHADER);

        //CREATE 3D ITEMS
        [MenuItem(PATH_CREATE_DECAL, priority = 
             CoreUtils.Priorities.assetsCreateShaderMenuPriority + SECTION_4 + URP_PRIORITY)]
        private static void CreateDecalShaderGraph() => Create(DECAL_SHADER);

        [MenuItem(PATH_CREATE_LIT, priority = 
             CoreUtils.Priorities.assetsCreateShaderMenuPriority + SECTION_1 + URP_PRIORITY)]
        private static void CreateLitShaderGraph() => Create(LIT_SHADER);

        [MenuItem(PATH_CREATE_LIT_METALLIC, priority = 
             CoreUtils.Priorities.assetsCreateShaderMenuPriority + SECTION_2 + URP_PRIORITY)]
        private static void CreateLitMetalicShaderGraph() => Create(LIT_METALLIC_SHADER);

        [MenuItem(PATH_CREATE_LIT_SPECULAR, priority = 
             CoreUtils.Priorities.assetsCreateShaderMenuPriority + SECTION_2 + URP_PRIORITY)]
        private static void CreateLitSpecularShaderGraph() => Create(LIT_SPECULAR_SHADER);

        [MenuItem(PATH_CREATE_UNLIT, priority = 
             CoreUtils.Priorities.assetsCreateShaderMenuPriority + SECTION_1 + URP_PRIORITY)]
        private static void CreateUnlitShaderGraph() => Create(UNLIT_SHADER);
#endif
    }
}