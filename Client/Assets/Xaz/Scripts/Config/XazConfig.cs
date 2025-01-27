public static class XazConfig
{

#if UNITY_EDITOR
    static public string autoAuthorName = "xiejie";
    //工程中需要添加下列tag
    static public string UIViewTagName = "UIView";
    static public string UIIgnoreTagName = "UIIgnore";
    static public string UIPropertyTagName = "UIProperty";
    public static string XazPath = "Assets/Xaz/";
    public static string guiskinPath = "Assets/Xaz/Editor/Res/xjGuiCode.guiskin";
#endif
    public static string DontDestoryNodeName = "DontDestroy";
    public static string viewRootNode = "Canvas";
    //热更目录
    public static string AssetsPath = "Assets/Resources/";
    public static string DEFAULT_Font = AssetsPath + "Fonts/GROBOLD.TTF";
    public static string UIPrefabPath = AssetsPath + "UI/UIPrefab/";
    public static string ScenePath = AssetsPath + "Scene/{0}.unity";

    public static string AtlasPath = AssetsPath + "UI/Atlas/";
    public static string SpritPath = AssetsPath + "UI/SpritAtlas/";

    public static string AudioPath = AssetsPath + "Audio/";
    public static string AudioSuffix = ".wav";

    //数据目录
    public static string DBFolderName = "DB";

    public static class LayerDefine
    {
        public const string UILAYER = "UI";
        public const string UIINVISIBLE = "UIHideView";

        public const string SceneDoor = "Entrance";
        public const string SceneActor = "Actor";
        public const string SceneObj = "SceneObj";
    }

    public static class TagDefine
    {
        public const string PLAYER = "Player";
    }
}