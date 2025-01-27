//----------------------------------------------
//  图集检查工具窗口
//  @author xiejie 
//----------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Xaz;
public class UICheckWindow : EditorWindow
{
    static int UIINV = 20;
    private static string[] nameList;
    public static string atlasName = "Map";
    public static string luafolder = "Assets/LuaScripts/";//Assets/LuaScripts/Config/TableData/
    //报告写入目录
    static string WritePath = Application.dataPath + "/../Report";
    public static string folderPrefab = "Assets/AssetsPackage/UI/Prefabs/";
    public static string cSharpfolder = "Assets/Scripts/GameLaunch/";
    public static string cSharpfolder2 = "Assets/Scripts/GameLogic/";
    public static string atlasPreUrl = XazConfig.AtlasPath;
    public static string loadpicUrl = "Assets/AssetsPackage/UI/Img/";
    public static string bigTexUrl = "Assets/Art/UI/BigSprite/";
    //删除完全没用到的
    private static bool deleteNoUse = false;
    ////当前工程设置文本化文件，要和工程当前设置统一
    //static bool IsForceTextNow = true;
    //是否生成其他引用关系的报告
    private static bool isReportAll = true;

    //是否显示在预制件中的摆放路径
    private static bool isShowResPath = false;

    //是否检测大图
    private static bool isCheckbigPic = false;
    //是否挪动大图到指定目录
    private static bool isMoveBigPic = false;

    private static string folder;
    private static int msize = 512;
    private static string barStr = "进度";

    static private Dictionary<string, string> lualist;
    [MenuItem("资源工具/图集分析", false, 40)]
    public static void OpenWindow()
    {
        nameList = new string[] { };
        UICheckWindow mapWin = EditorWindow.GetWindowWithRect(typeof(UICheckWindow), new Rect(0, 0, 400, 400), false, "图集分析") as UICheckWindow;
        mapWin.Show();
    }

    private void OnGUI()
    {
        if (nameList.Length == 0)
        {
            nameList = EditorTool.GetFileNameByPath(atlasPreUrl);
        }
        GUILayout.Label(string.Format("图集目录:{0}\n代码目录:{1}\n界面目录:{2}",
            atlasPreUrl,
            luafolder,
            folderPrefab), GUILayout.MaxHeight(80));
        atlasName = EditorTool.PopupArray("图集选择", atlasName, nameList);
        GUILayout.Space(UIINV);
        isReportAll = EditorGUILayout.Toggle("是否显示所有引用关系", isReportAll);
        deleteNoUse = EditorGUILayout.Toggle("是否立即删除未用到的图片", deleteNoUse);
        isShowResPath = EditorGUILayout.Toggle("是否输出具体摆放路径", isShowResPath);
        isCheckbigPic = EditorGUILayout.Toggle("是否检测图集中的大图", isCheckbigPic);
        if (isCheckbigPic)
        {
            isMoveBigPic = EditorGUILayout.Toggle("是否挪动大图到指定目录", isMoveBigPic);
            GUILayout.Label(string.Format("限定:超过{0}尺寸的图片整理至目录:{1}", msize, bigTexUrl));
        }
        GUILayout.Space(UIINV);
        if (GUILayout.Button("检测图集", GUILayout.Width(240), GUILayout.Height(40)))
        {
            CheckAtlasUI(atlasPreUrl + atlasName + "/", atlasName);
        }
        GUILayout.Space(UIINV);
        GUILayout.Label(string.Format("动态加载的单张图目录:{0}", loadpicUrl), GUILayout.MaxHeight(30));
        if (GUILayout.Button("检测动态加载的单张图目录", GUILayout.Width(240), GUILayout.Height(40)))
        {
            CheckAtlasUI(loadpicUrl, "");
        }

    }

    [MenuItem("资源工具/界面prefab分析", false, 40)]
    public static void CheckPrefabUI()
    {
        lualist = new Dictionary<string, string>() { };
        string[] files = Directory.GetFiles(folderPrefab, "*.prefab", SearchOption.AllDirectories);
        string[] luaAllFiles = Directory.GetFiles(luafolder, "*.lua", SearchOption.AllDirectories);
        string[] cAllFiles1 = Directory.GetFiles(cSharpfolder, "*.cs", SearchOption.AllDirectories);
        // string[] cAllFiles2 = Directory.GetFiles(cSharpfolder2, "*.cs", SearchOption.AllDirectories);
        List<string> cAllFiles = new List<string>();
        cAllFiles.AddRange(cAllFiles1);
        // cAllFiles.AddRange(cAllFiles2);
        StringBuilder sb = new StringBuilder();
        StringBuilder sball = new StringBuilder();
        sb.AppendLine("-----------------------疑似未用到的界面prefab资源报告-----------------------");
        sb.AppendLine(string.Format("-----------------------检测目录:{0}-----------------------", folderPrefab));
        sb.AppendLine(string.Format("-----------------------检测时间:{0}-----------------------", TimeUtil.GetNow().ToString()));
        sb.AppendLine("-----------------------注：导出的base也需要检查如果没用到随prefab一起删除-----------------------");
        sb.AppendLine("-----------------------注：若确定是用到的填入白名单UICheckWhiteList.cs【prefabName】-----------------------");
        FileIn resFile = new FileIn();
        int allnum = files.Length;
        float curInx = 0f;
        bool hasRes = false;
        foreach (string file in files)
        {
            curInx++;
            EditorUtility.DisplayProgressBar(barStr, "", curInx / (float)allnum);
            string filename = Path.GetFileNameWithoutExtension(file);

            if (UICheckWhiteList.prefabName.IndexOf(filename) != -1)
            {
                continue;
            }
            else
            {
                resFile = SelectUIPrefebNameInLuaFile(resFile, filename, luaAllFiles, cAllFiles);
                if (!resFile.isIn)
                {
                    hasRes = true;
                    sb.AppendLine(string.Format("\tprefabName：|---------{0}-------{1}|", filename, GetFileCreateTimeStr(file)));
                    sb.AppendLine();
                }
            }
        }
        if (!hasRes)
        {
            sb.AppendLine("结果：暂无需要删除的界面prefab");
        }
        Report(sb.ToString(), string.Format("预制件分析.txt"));
        EditorUtility.ClearProgressBar();
    }
    // [MenuItem("资源工具/图集：图片分析", false, 40)]
    public void CheckAtlasUI(string folder, string atlasName)
    {
        //if (IsForceTextNow)
        //{
        //    EditorSettings.serializationMode = SerializationMode.ForceText;
        //}
        lualist = new Dictionary<string, string>() { };
        string[] files2 = Directory.GetFiles(folder, "*.png", SearchOption.AllDirectories);
        //图片格式不统一的需要工具检测吗？
        //string[] files3 = Directory.GetFiles(folder, "*.jpg", SearchOption.AllDirectories);
        Dictionary<string, List<String>> comondic = new Dictionary<string, List<String>>();
        int allnum = 0;
        foreach (string file in files2)
        {
            allnum = allnum + 1;
            comondic.Add(file, new List<String>() { });
        }
        string[] prefabAllFiles = Directory.GetFiles(folderPrefab, "*.prefab", SearchOption.AllDirectories);
        float curInx = 0f;
        foreach (KeyValuePair<string, List<String>> kv in comondic)
        {
            curInx++;
            string guid = AssetDatabase.AssetPathToGUID(kv.Key);
            foreach (string prefabFile in prefabAllFiles)
            {
                //if (IsForceTextNow && !isShowResPath)
                //{
                //string filetxt = File.ReadAllText(prefabFile);
                //    if (Regex.IsMatch(filetxt, guid))
                //    {
                //        kv.Value.Add(prefabFile);
                //        if (!isReportAll)
                //        {
                //            break;
                //        }
                //    }
                //}
                string[] dependencies = AssetDatabase.GetDependencies(prefabFile, false);
                Dictionary<string, string> preIn = new Dictionary<string, string>();
                if (isShowResPath)
                {
                    preIn = SpriteInPrefab(prefabFile);
                }
                for (int i = 0; i < dependencies.Length; i++)
                {
                    string guids = AssetDatabase.AssetPathToGUID(dependencies[i]);
                    if (guids == guid)
                    {
                        string filename = Path.GetFileNameWithoutExtension(kv.Key);
                        if (isShowResPath && preIn.ContainsKey(filename))
                        {
                            kv.Value.Add(prefabFile + preIn[filename]);
                        }
                        else
                        {
                            kv.Value.Add(prefabFile);
                        }
                        if (!isReportAll)
                        {
                            break;
                        }
                    }
                }
                if (kv.Value.Count > 0 && !isReportAll)
                {
                    break;
                }
            }
            EditorUtility.DisplayProgressBar(barStr, "", curInx / (float)allnum * 0.5f);
        }
        bool isCommonAtatls = IsCommonAtlas(atlasName);
        StringBuilder sb = new StringBuilder();
        StringBuilder sball = new StringBuilder();
        StringBuilder sball2 = new StringBuilder();
        StringBuilder bigstr = new StringBuilder();
        StringBuilder useCountstr = new StringBuilder();
        StringBuilder vtt = new StringBuilder();
        List<string> unUseList = new List<string>();
        sb.AppendLine(string.Format("【图片分析报告】：\n【检测目录】:{0}\n【检测时间】:{1}", folder, TimeUtil.GetNow().ToString()));
        sb.AppendLine();
        sb.AppendLine(string.Format("-----------------------一：疑似可删除的图片资源——需要整理----------------------------------------------"));
        sb.AppendLine("-----------------------注：若确定是用到的可填入白名单UICheckWhiteList.cs【pngName】");
        sb.AppendLine();
        curInx = 0f;
        FileIn resFile;
        string[] luaFiles = Directory.GetFiles(luafolder, "*.lua", SearchOption.AllDirectories);
        foreach (KeyValuePair<string, List<String>> kv in comondic)
        {
            curInx++;
            EditorUtility.DisplayProgressBar(barStr, "", curInx / (float)allnum + 0.5f);
            string filename = Path.GetFileNameWithoutExtension(kv.Key);
            if (UICheckWhiteList.pngName.IndexOf(filename) != -1)
            {
                continue;
            }
            resFile = new FileIn();
            resFile.isIn = false;
            resFile = SelectFileNameInLuaFile(resFile, filename, luaFiles);
            if (kv.Value.Count <= 0)
            {
                if (!resFile.isIn)
                {
                    sb.AppendLine(string.Format("\tSpriteName：{0}\t创建时间：{1}", filename, GetFileCreateTimeStr(kv.Key)));
                    sb.AppendLine();
                    unUseList.Add(kv.Key);
                }
                else
                {
                    sball.AppendLine(string.Format("\tSpriteName：{0}---------|prefab引用为0，代码中含有再{1}", filename, resFile.path));
                }
            }
            else
            {
                if (isCheckbigPic)
                {
                    bigstr = CheckBigSprite(kv.Key, bigstr);
                }
                if (isReportAll)
                {
                    vtt.Clear();
                    for (int j = 0; j < kv.Value.Count; j++)
                    {
                        vtt.Append(kv.Value[j] + "\n\t\t|");
                    }
                    if (isCommonAtatls && kv.Value.Count <= 1 && !resFile.isIn)
                    {
                        //通用图集中，被预制件引用为1个，且代码中不含的，被认为复用率很低，建议挪出
                        //（代码调用该图片的地方不好评判，因为有可能是个共用组件）
                        useCountstr.AppendLine(string.Format("\tSpriteName：{0}\n\t\t|{1}\n", filename, vtt));
                    }
                    else
                    {
                        //非通用图集，图片引用信息把代码也都列出来，只在代码中的写入sball，代码和预制件都有的在sball2
                        if (!resFile.isIn)
                        {
                            sball2.AppendLine(string.Format("\tSpriteName：{0}\n\t\t|{1}\n", filename, vtt));
                        }
                        else
                        {
                            sball2.AppendLine(string.Format("\tSpriteName：{0}\n\t代码中含有在{1}\n\t\t|{2}\n", filename, resFile.path, vtt));
                        }
                    }
                    sball2.AppendLine();
                }
            }
        }
        if (isCommonAtatls&& useCountstr.Length>0)
        {
            sb.AppendLine(string.Format("-----------------------复用率过低的图片，建议挪出通用图集--------------------------------------"));
            sb.AppendLine(useCountstr.ToString());
        }
        if (isCheckbigPic)
        {
            sb.AppendLine(string.Format("-----------------------二：图集中的过大图片--------------------------------------"));
            sb.AppendLine(bigstr.ToString());
        }
        else
        {
            sb.AppendLine(string.Format("-----------------------二：图集中的过大图片（未开启检测）--------------------------------------"));
        }
        sb.AppendLine(string.Format("-----------------------三：图片资源引用信息--------------------------------------"));
        sb.AppendLine(sball.ToString());
        sb.AppendLine(sball2.ToString());

        if (atlasName != "")
        {
            Report(sb.ToString(), string.Format("{0}图集分析.txt", atlasName), atlasName);
        }
        else
        {
            Report(sb.ToString(), string.Format("大图目录分析.txt"));
        }
        if (deleteNoUse)
        {
            for (int i = 0; i < unUseList.Count; i++)
            {
                File.Delete(unUseList[i]);
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    private static List<string> commonAtlas = new List<string> { "Common" };
    private static bool IsCommonAtlas(string atlasName)
    {
        return commonAtlas.IndexOf(atlasName) >= 0;
    }
    private static StringBuilder CheckBigSprite(string file, StringBuilder bigstr)
    {
        //string[] files2 = Directory.GetFiles(folder, "*.png", SearchOption.AllDirectories);
        //foreach (string file in files2)
        //{
        Sprite objv = AssetDatabase.LoadAssetAtPath<Sprite>(file) as Sprite;
        if (objv.textureRect.width > msize || objv.textureRect.height > msize)
        {
            string filename = Path.GetFileNameWithoutExtension(file);
            bigstr.AppendLine(string.Format("\t过大图片SpriteName：{0}尺寸{1}*{1}\n", filename, objv.textureRect.width, objv.textureRect.height));
            if (isMoveBigPic)
            {
                File.Move(file, bigTexUrl + filename + ".png");
                File.Move(file + ".meta", bigTexUrl + filename + ".png" + ".meta");
            }
        }
        //}
        //if (doNow)
        //{
        //    AssetDatabase.SaveAssets();
        //    AssetDatabase.Refresh();
        //}
        return bigstr;
    }

    private static string GetFileCreateTimeStr(string assetPath)
    {
        // 获取资源文件的创建时间
        // string assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
        System.IO.FileInfo assetFileInfo = new System.IO.FileInfo(assetPath);
        System.DateTime creationTime = assetFileInfo.CreationTime;
        return creationTime.ToString();
    }

    private static void Report(string content, string filename, string newpath = "")
    {
        string filePath = WritePath + "/" + newpath;
        EditorTool.Report(filePath);
        File.WriteAllText(Path.Combine(filePath, filename), content.Replace(Environment.NewLine, "\r\n"), Encoding.UTF8);
    }

    /// <summary>
    /// 查询lua里的图片名称
    /// </summary>
    /// <param name="sname"></param>
    /// <returns></returns>
    static public FileIn SelectFileNameInLuaFile(FileIn v, string sname, string[] luaFiles)
    {
        foreach (string file in luaFiles)
        {
            string filex = XazEditor.FileUtil.GetValidLuaFile(file, lualist);
            v.path = file;
            string Pattern = @"\b" + sname + "\\b";
            if (Regex.IsMatch(filex, Pattern) || filex.IndexOf(sname) != -1)
            {
                v.isIn = true;
                return v;
            }
            string vname = RemoveEndNum(sname);
            if ((vname.Length != sname.Length && filex.IndexOf(vname, StringComparison.Ordinal) != -1))
            {
                v.isIn = true;
                return v;
            }
        }
        return v;
    }

    //去除末位数字https://blog.csdn.net/afloatboat/article/details/128377041
    static char[] digits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
    static public string RemoveEndNum(string input)
    {
        return input.TrimEnd(digits);
    }

    static public FileIn SelectUIPrefebNameInLuaFile(FileIn v, string sname, string[] luaFiles, List<string> cFiles)
    {
        v.isIn = false;
        string vt3 = string.Format("{0}.prefab", sname);
        string vname = RemoveEndNum(sname);
        foreach (string file in luaFiles)
        {
            string filex = XazEditor.FileUtil.GetValidLuaFile(file, lualist);
            v.path = file;
            // 使用正则表达式搜索剩余的文本，看是否有符合要求的字符串
            string pattern = @"OpenWindow\(UIWindowNames\.\w*" + sname;
            if (Regex.IsMatch(filex, pattern) || filex.IndexOf(vt3, StringComparison.Ordinal) != -1 || (vname.Length != sname.Length && filex.IndexOf(vname, StringComparison.Ordinal) != -1))
            {
                v.isIn = true;
                return v;
            }
            else
            {
                //有的界面完全通过配表或配置打开的
                //此处是项目填特殊界面配置的地方，不包括uiwindows.lua
                if (file.IndexOf("Const.lua") > 0 || file.IndexOf("FunctionShift.lua") > 0)
                {
                    if (filex.IndexOf(string.Format("UIWindowNames.{0}", sname)) != -1 || filex.IndexOf(sname) != -1)
                    {
                        v.isIn = true;
                        return v;
                    }
                }
            }
        }
        if (!v.isIn)
        {
            foreach (string file in cFiles)
            {
                string filex = XazEditor.FileUtil.ReadString(file);
                v.path = file;
                if (filex.IndexOf(vt3, StringComparison.Ordinal) != -1)
                {
                    v.isIn = true;
                    return v;
                }
            }
        }
        return v;
    }

    //先嵌套着后期考虑优化结构
    static private Dictionary<string, Dictionary<string, string>> allPrefabSpriteInfo = new Dictionary<string, Dictionary<string, string>>();
    static private Dictionary<string, string> SpriteInPrefab(string prefabFile)
    {
        Dictionary<string, string> spriteVal = new Dictionary<string, string>();
        if (!allPrefabSpriteInfo.ContainsKey(prefabFile))
        {
            Dictionary<Sprite, List<string>> resDict = GetDependencies(AssetDatabase.LoadAssetAtPath<GameObject>(prefabFile));
            StringBuilder tt = new StringBuilder();
            foreach (KeyValuePair<Sprite, List<string>> dep in resDict)
            {
                tt.Clear();
                foreach (string s in dep.Value)
                {
                    tt.Append("\n\t--------" + s);
                }
                spriteVal.Add(dep.Key.name, tt.ToString());
            }
            allPrefabSpriteInfo.Add(prefabFile, spriteVal);
        }
        allPrefabSpriteInfo.TryGetValue(prefabFile, out spriteVal);
        return spriteVal;
    }

    //获取图片在预制件里的路径关系
    private static Dictionary<Sprite, List<string>> GetDependencies(GameObject prefab)
    {
        Dictionary<Sprite, List<string>> tmp = new Dictionary<Sprite, List<string>>();
        if (prefab)
        {
            foreach (Component c in prefab.GetComponentsInChildren<Component>(true))
            {
                if (c)
                {
                    SerializedObject o = new SerializedObject(c);
                    SerializedProperty p = o.GetIterator();
                    do
                    {
                        if (p.propertyType == SerializedPropertyType.ObjectReference)
                        {
                            if (p.objectReferenceValue != null && p.objectReferenceValue is Sprite)
                            {
                                Sprite s = p.objectReferenceValue as Sprite;
                                if (!tmp.ContainsKey(s))
                                {
                                    tmp[s] = new List<string>();
                                }
                                string url = AnimationUtility.CalculateTransformPath(c.transform, prefab.transform.parent);
                                if (!string.IsNullOrEmpty(url))
                                {
                                    tmp[s].Add(url);
                                }
                            }
                        }
                    } while (p.Next(true));
                }
            }
        }
        return tmp;
    }

    public class FileIn
    {
        public bool isIn;
        public string path;
    }

    public class CheckRes
    {
        public Sprite spric;
        public string path;
    }

    [MenuItem("资源工具/图集：重复图片报告", false, 40)]
    public static void CheckDuplicateSprite()
    {
        lualist = new Dictionary<string, string>() { };
        string[] files2 = Directory.GetFiles(atlasPreUrl, "*.png", SearchOption.AllDirectories);
        List<CheckRes> allSprite = new List<CheckRes>() { };
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("---------------------------重复的资源-----------------------");
        sb.AppendLine(string.Format("-----------------------检测时间:{0}-----------------------", TimeUtil.GetNow().ToString()));
        foreach (string file in files2)
        {
            Sprite objv = AssetDatabase.LoadAssetAtPath<Sprite>(file) as Sprite;
            if (objv)
            {
                CheckRes nt = new CheckRes();
                nt.spric = objv;
                nt.path = file;
                allSprite.Add(nt);
            }
        }
        Dictionary<int, int> initv = new Dictionary<int, int>();
        for (int i = 0; i < allSprite.Count; i++)
        {
            EditorUtility.DisplayProgressBar(barStr, "", (float)i / (float)allSprite.Count);
            CheckRes v = allSprite[i];
            for (int j = 0; j < allSprite.Count; j++)
            {
                if (i != j)
                {
                    CheckRes vt = allSprite[j];
                    if (!initv.ContainsKey(j) && IsDuplicate(v.spric, vt.spric))
                    {
                        initv.Add(j, 1);
                        sb.AppendLine(string.Format("\tPath：{0} \n\t和Path:{1}", v.path, vt.path));
                        sb.AppendLine();
                    }
                }
            }
        }
        Report(sb.ToString(), string.Format("重复图片分析.txt"));
        EditorUtility.ClearProgressBar();
    }

    //比较Sprite是否重复
    private static bool IsDuplicate(Sprite t1, Sprite t2)
    {
        if (t1 == null || t2 == null) return false;
        if (t1 == t2) return false;
        byte[] b1 = t1.texture.GetRawTextureData();
        byte[] b2 = t2.texture.GetRawTextureData();
        if (b1.Length != b2.Length) return false;
        for (int i = 0; i < b1.Length; i++)
            if (b1[i] != b2[i])
                return false;
        return true;
    }

}