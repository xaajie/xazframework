//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
// 自动生成lua样例代码（界面类 模块类（todo））
// @author xiejie
//------------------------------------------------------------
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
public class GenerCode
{
    public const string TEMPLATE_DEFAULT = "Assets/Xaz/Editor/GenerCode/luas.txt";
    public const string TEMPLATE_NET ="Assets/Xaz/Editor/GenerCode/lua_net.txt";
    public const string TEMPLATE_MGR = "Assets/Xaz/Editor/GenerCode/lua_mgr.txt";
    public const string TEMPLATE_DATAEXT = "Assets/Xaz/Editor/GenerCode/lua_extdata.txt";
    private const string extention = ".cs";
    [MenuItem("Assets/Create/Lua Script", false, 80)]
    public static void CreatNewLua()
    {
        string locationPath = GetSelectedPathOrFallback();
        string templateFullPath = GetLuaTemplateByLocation(locationPath);

        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
        ScriptableObject.CreateInstance<MyDoCreateScriptAsset>(),
        locationPath + "/New Lua.lua",
        null,
        templateFullPath);
    }

    public static string GetSelectedPathOrFallback()
    {
        string path = "Assets";
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }

    public static string GetLuaTemplateByLocation(string locationPath)
    {
        return TEMPLATE_DEFAULT;
    }

    public static void CreatNewCode(string templateFullPath, string system, string locationPath)
    {
        Debug.Log(locationPath + "/" + system + extention);
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,ScriptableObject.CreateInstance<MyDoCreateScriptAsset>(),
        locationPath + "/" + system + extention, null, templateFullPath);
    }

    public static string GetAuthor()
    {
        return XazConfig.autoAuthorName;
    }
}


class MyDoCreateScriptAsset : EndNameEditAction
{
    public override void Action(int instanceId, string pathName,  string resourceFile)
    {
        Object o = CreateScriptAssetFromTemplate(pathName, resourceFile);
        ProjectWindowUtil.ShowCreatedAsset(o);
    }

    internal static Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
    {
        string fullPath = Path.GetFullPath(pathName);
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullPath);
        string sysname = fileNameWithoutExtension;
        if (fileNameWithoutExtension.IndexOf("Net") != -1)
        {
            sysname = fileNameWithoutExtension.Replace("Net", "");
            resourceFile = GenerCode.TEMPLATE_NET;
        }
        else if (fileNameWithoutExtension.IndexOf("Data") != -1)
        {
            resourceFile = GenerCode.TEMPLATE_DATAEXT;
        }
        StreamReader streamReader = new StreamReader(resourceFile);
        string text = streamReader.ReadToEnd();
        streamReader.Close();
        text = Regex.Replace(text, "#NAME#", fileNameWithoutExtension);
        text = Regex.Replace(text, "#SYS#", sysname);
        text = Regex.Replace(text, "#AUTHOR#", GenerCode.GetAuthor());
        bool encoderShouldEmitUTF8Identifier = false;
        bool throwOnInvalidBytes = false;
        UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
        bool append = false;
        StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
        streamWriter.Write(text);
        streamWriter.Close();
        AssetDatabase.ImportAsset(pathName);
        return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
    }

}