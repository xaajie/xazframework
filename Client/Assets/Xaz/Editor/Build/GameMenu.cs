#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using XazEditor;
public class GameMenu
{
    static public void GenerateStaticAssetsIndex(bool compressed)
    {
        var staticAssetsPath = XazHelper.staticAssetsPath;
        var staticIndexPath = XazHelper.staticAssetsIndexPath;
        if (Directory.Exists(staticAssetsPath))
        {
            var files = new List<string>();
            var fullPath = Path.GetFullPath(staticAssetsPath).Replace("\\", "/");
            foreach (var file in Directory.GetFiles(staticAssetsPath, "*.*", SearchOption.AllDirectories))
            {
                if (file.EndsWith(".meta") || Path.GetFileName(file) == "PkgAssets.dat")
                    continue;
                files.Add(Path.GetFullPath(file).Replace("\\", "/").Replace(fullPath + "/", "") + "," + XazEditor.FileUtil.GetMD5Hash(file) + "," + (compressed ? "-1" : "0"));
            }
            XazEditor.FileUtil.WriteString(staticIndexPath, string.Join("\n", files.ToArray()));
        }
        else
        {
            if (File.Exists(staticIndexPath))
            {
                File.Delete(staticIndexPath);
            }
        }
        UnityEditor.AssetDatabase.Refresh();
    }

    static public void GenerateXazUpdateIndex()
    {
        var staticAssetsPath = XazHelper.staticAssetsPath;
        var staticIndexPath = XazHelper.staticAssetsPath + "/XazUpdateIndex";
        if (Directory.Exists(staticAssetsPath))
        {
            var files = new List<string>();
            var fullPath = Path.GetFullPath(staticAssetsPath).Replace("\\", "/");
            foreach (var file in Directory.GetFiles(staticAssetsPath, "*.*", SearchOption.AllDirectories))
            {
                if (file.EndsWith(".meta") || Path.GetFileName(file) == "PkgAssets.dat" || Path.GetFileName(file) == "XazUpdateIndex")
                    continue;

                byte[] data = File.ReadAllBytes(file);
                int size = data.Length;
                files.Add(Path.GetFullPath(file).Replace("\\", "/").Replace(fullPath + "/", "") + "," + XazEditor.FileUtil.GetMD5Hash(file) + "," + size + "," + size);
            }
            XazEditor.FileUtil.WriteString(staticIndexPath, string.Join("\n", files.ToArray()));
        }
        else
        {
            if (File.Exists(staticIndexPath))
            {
                File.Delete(staticIndexPath);
            }
        }
        UnityEditor.AssetDatabase.Refresh();
    }


    [MenuItem("打包/制作热更包", false, 60)]
    public static void MenuBuildAssetBundles()
    {
        string version = "0.1";
        string outPath = XazHelper.staticAssetsPath;  // "D:\\t2\\bundle";
        //string luaPath = XazHelper.staticAssetsPath + "/" + LuaManager.LUA_PreDIR;
        BuildAssetBundles(version, outPath);
        GenerateStaticAssetsIndex(false);
        GenerateXazUpdateIndex();
    }
    //[MenuItem("打包/制作安卓整包", false, 60)]
    //public static void BuildAndroidWindows()
    //{
    //    if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
    //    {
    //        EditorUtility.DisplayDialog("制作安卓包", "当前平台不是安卓平台，请先切换至安卓平台再build", "OK");
    //        return;
    //    }
    //    MenuBuildAssetBundles();
    //    Dictionary<string, string> args = new Dictionary<string, string>{
    //         {"apkname" , "AVGGame"},
    //        { "defines","USE_LUA;LUA_5_3;LUA_ASSETBUNDLE;USE_ASSETBUNDLE"}
    //        //{"keystoreName" , "D:/UnityProgram/viewro.keystore" },
    //        //{"keystorePass" , "Virtools5.0"},
    //        //{"keyaliasName" , "viewro"},
    //        //{"keyaliasPass" , "Virtools5.0"}
    //    };
    //    BuildProject.BuildAndroidDemo(args);
    //}

    //[MenuItem("打包/生成IOS的Xcode工程", false, 60)]
    //public static void BuildIOSXcode()
    //{
    //    if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
    //    {
    //        EditorUtility.DisplayDialog("制作IOS的Xcode工程", "当前平台不是iOS平台，请先切换至iOS平台再build", "OK");
    //        return;
    //    }
    //    MenuBuildAssetBundles();
    //    Dictionary<string, string> args = new Dictionary<string, string>{
    //         {"apkname" , "viewro"},
    //        { "exportProjectPath",System.IO.Directory.GetCurrentDirectory() + "/iOS"},
    //        { "defines","USE_LUA;LUA_5_3;LUA_ASSETBUNDLE;USE_ASSETBUNDLE"},
    //    };
    //    BuildProject.BuildIOS(args);
    //}


    [MenuItem("打包/制作windows包", false, 60)]
    public static void BuildWindows()
    {
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows
         && EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows64)
        {
            EditorUtility.DisplayDialog("制作Windows包", "当前平台不是PC平台，请先切换至PC平台再build", "OK");
            return;
        }
        MenuBuildAssetBundles();
        Dictionary<string, string> args = new Dictionary<string, string>{
            { "exportProjectPath",System.IO.Directory.GetCurrentDirectory() + "/windows/PinTu.exe"},
            { "defines","USE_ASSETBUNDLE"},
        };
        BuildProject.BuildWindows(args);
    }

    private static void BuildAssetBundles(string version, string path)
    {

    }
}
#endif

