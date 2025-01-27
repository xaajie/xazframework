//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//  代码生成器
//  @author xiejie 
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using XazEditor;
public class GenerCodeWindow : EditorWindow
{
    public string sysname = "UIFriend";
    public string viewname = "UIFriend";
    public static int UIINV = 20;
    public static string UIPre =  "Assets/Scripts/UI";
    public static string MgrPre = "Assets/Scripts/Logic";
    private List<string> OnOpcodeList = new List<string>() { };
    public static string[] nameList = new string[] { };

    private UnityEngine.Object sprotoData;
    private string sprotoText = "";
    public string netLuaName = "";
    public string moduleName = "";
    public GUISkin myStyle;
    [MenuItem("程序工具/代码生成器", false, 60)]
    public static void OpenWindow()
    {
        GenerCodeWindow mapWin = EditorWindow.GetWindowWithRect(typeof(GenerCodeWindow), new Rect(0, 0, 400,500), false, "GenerCodeWindow") as GenerCodeWindow;
        mapWin.Show();
    }


    public static bool IsSpecUser()
    {
        Debug.Log(SystemInfo.deviceUniqueIdentifier);
        return SystemInfo.deviceUniqueIdentifier == "8ecf60c63ef15c84e01a5d9141f4da2904fa31ed";
    }

    void OnEnable()
    {
        myStyle = (GUISkin)AssetDatabase.LoadAssetAtPath<GUISkin>(XazConfig.guiskinPath);
    }
    /* 
     * <summary>
     * 显示窗体里面的内容
     * </summary>
    */
    private bool createNewFolder = false;
    private Texture2D vt;
    private void OnGUI()
    {
        GUI.skin = myStyle;
        if (nameList.Length == 0)
        {
            nameList = EditorTool.GetFileNameByPath(UIPre);
        }
        EditorGUILayout.Separator();
        GUILayout.BeginVertical("box");
        GUILayout.Label("界面代码生成");
        viewname = EditorGUILayout.TextField("界面名称", viewname);
        createNewFolder = EditorGUILayout.Toggle("是否新建UI模块目录", createNewFolder);
        if (createNewFolder)
        {
            sysname = EditorGUILayout.TextField("UI模块目录名称", sysname);
        }
        else
        {
            sysname = EditorTool.PopupArray("选择UI模块目录", sysname, nameList);
        }
        if (GUILayout.Button("生成"))
        {
            string subPath = UIPre + "/" + sysname;
            Debug.Log(Application.dataPath + subPath);
            if (!Directory.Exists(subPath))
            {
                AssetDatabase.CreateFolder(UIPre, sysname);
                AssetDatabase.Refresh();
                GenerCode.CreatNewCode(GenerCode.TEMPLATE_DEFAULT, viewname, subPath);
                AssetDatabase.Refresh();
            }
            else
            {
                if (!Directory.Exists(subPath + "/" + viewname ))
                {
                    GenerCode.CreatNewCode(GenerCode.TEMPLATE_DEFAULT, viewname, subPath);
                }
            }

        }
        GUILayout.EndVertical();
        GUILayout.BeginVertical("box");
        GUILayout.Label("manager代码生成");
        moduleName = EditorGUILayout.TextField("模块名称", moduleName);
        if (GUILayout.Button("生成"))
        {
            moduleName = moduleName.ToUpperFirst();
            string subPathv = MgrPre +"/" + moduleName;
            if (!Directory.Exists(subPathv))
            {
                AssetDatabase.CreateFolder(MgrPre, moduleName);
                AssetDatabase.Refresh();
                GenerCode.CreatNewCode(GenerCode.TEMPLATE_NET, "Net"+ moduleName, subPathv);
                AssetDatabase.Refresh();
                GenerCode.CreatNewCode(GenerCode.TEMPLATE_DATAEXT, "User" + moduleName+"Data", subPathv);
                AssetDatabase.Refresh();
                GenerCode.CreatNewCode(GenerCode.TEMPLATE_MGR, moduleName+"Mgr", subPathv);
                AssetDatabase.Refresh();
            }
        }
        GUILayout.EndVertical();
       // GUILayout.Space(UIINV);
        GUILayout.BeginVertical("box");
        GUILayout.Label("协议片段代码生成");
        netLuaName = EditorGUILayout.TextField("NetXXX名称", netLuaName);
        if (GUILayout.Button("打开proto"))
        {
            System.Diagnostics.Process.Start(Application.dataPath + "/../../protoBuf_cob/file/protocolCmd.proto");
        }
        sprotoData = EditorGUILayout.ObjectField(sprotoData, typeof(UnityEngine.Object), true) as UnityEngine.Object;
        EditorGUILayout.LabelField("复制protocolCmd.proto中的的新CMD到下框");
        sprotoText = EditorGUILayout.TextArea(sprotoText, GUILayout.Width(400), GUILayout.Height(80));
        if (GUILayout.Button("生成"))
        {
            OnOpcodeList = new List<string>() { };
            if (sprotoText == "" && sprotoData != null)
            {
                GenerNetCodeFromFile();
            }
            else
            {
                GenerNetCode(sprotoText);
            }
        }
        GUILayout.EndVertical();
    }
    private void GenerNetCodeFromFile()
    {
        var fileAddress = UnityEditor.AssetDatabase.GetAssetPath(this.sprotoData);
        FileInfo fInfo0 = new FileInfo(fileAddress);
        string s = "";
        if (fInfo0.Exists)
        {
            StreamReader r = new StreamReader(fileAddress);
            s = r.ReadToEnd();
            GenerNetCode(s);
        }
    }

    private void GenerNetCode(string s)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        string[] ar = s.Split('\n');
        string st = "";
        ParmDesc parmStr = new ParmDesc();
        for (int i = 0; i < ar.Length; i++)
        {
            st = ar[i];
            if (st != "")
            {
                int t = st.LastIndexOf(".");
                string name = st.Substring(t + 1);
                if (name.IndexOf("@") != -1)
                {
                    sb.AppendLine();
                    string[] parmArr = name.Split('_');
                    parmStr = selectParmInLuaFile(parmArr[1]);
                    sb.AppendFormat("--{0}", name);//.AppendLine();
                }
                else if (name.IndexOf(";") != -1)
                {
                    string[] art = name.Split(';');
                    sb.AppendFormat("--{0}", art[1].Replace("\n", "").Trim()).AppendLine();
                }
                if (name.IndexOf("REQ") != -1)
                {
                    string sys = GetProtoName(name);
                    sb.AppendFormat("function {0}:Send{1}({2})", netLuaName, GetFuncName(sys), parmStr.sendParm).AppendLine();
                    string[] art = parmStr.rawParm.Split('\n');
                    for (int c = 0; c < art.Length; c++)
                    {
                        if (art[c] != "" && art[c] != "\r")
                        {
                            sb.Append("\t").AppendFormat("--{0}", art[c]);
                        }
                    }
                    sb.Append("\t").AppendFormat("self:Request(ServerConstant.ProtocolCmd.{0},{{{1}}})", sys, parmStr.sendParmCode).AppendLine();
                    sb.AppendLine("end");
                }
                else if (name.IndexOf("CMD") != -1)
                {
                    // sb.AppendLine("\n");
                    string sys = GetProtoName(name);
                    string funcname = GetFuncName(sys);
                    OnOpcodeList.Add(sys);
                    OnOpcodeList.Add(funcname);
                    sb.AppendFormat("function {0}:On{1}(msg)", netLuaName, funcname).AppendLine();
                    sb.Append("\t").AppendLine("if self:OnReturn(msg) then");
                    string[] art = parmStr.rawParm.Split('\n');
                    for (int c = 0; c< art.Length; c++)
                    {
                        if (art[c] != "" && art[c] != "\r")
                        {
                            sb.Append("\t").AppendFormat("--{0}", art[c]);
                        }
                    }
                    sb.Append("\t").AppendLine("end");
                    sb.AppendLine("end");
                }
            }
        }
        sb.AppendLine("--------------------------------------------------------");
        for (int i = 0; i < OnOpcodeList.Count; i += 2)
        {
            sb.AppendFormat("[ServerConstant.ProtocolCmd.{0}] = self.On{1},", OnOpcodeList[i], OnOpcodeList[i + 1]).AppendLine();
        }
        WriteString(sb.ToString());
    }

    void WriteString(string content)
    {
        string path = Path.Combine(Application.dataPath, netLuaName + ".txt");
        File.WriteAllText(path, content.Replace(Environment.NewLine, "\r\n"),
            System.Text.Encoding.UTF8);
        System.Diagnostics.Process.Start(path);
    }

    private string GetProtoName(string name)
    {
        int t1 = name.LastIndexOf("=");
        return name.Substring(0, t1).Trim();
    }

    /// <summary>
    /// 生成方法名
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private string GetFuncName(string name)
    {
        string[] ar = name.Split('_');
        string st = "";
        string funcname = "";
        for (int i = 0; i < ar.Length; i++)
        {
            st = ar[i];
            if (i != 0)
                if (i != 0 && i != ar.Length - 1)
                //if (i != 0 && i != 1 && i != ar.Length - 1)
                {
                    funcname = funcname + GetUpperName(st);
                }
        }
        return funcname;
    }

    /// <summary>
    /// 首字母大写,其余小写
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private string GetUpperName(string str)
    {
        return str.Substring(0, 1) + str.Substring(1).ToLower();
    }


    public class ParmDesc
    {
        public string sendParm="";
        public string sendParmCode="";
        public string rawParm="";
    }

    private  string luafolder = "Assets/LuaScripts/Net/Protol/";//Assets/LuaScripts/Config/TableData/\()";
    private ParmDesc selectParmInLuaFile(string sname)
    {
        ParmDesc parmInfo = new ParmDesc();
        string[] luaFiles = Directory.GetFiles(luafolder, "*.lua", SearchOption.AllDirectories);
        List<string> parmarr = new List<string>() { };
        List<string> parmarr2 = new List<string>() { };
        List<string> parmarr3 = new List<string>() { };
        string rawtxt = "";
        foreach (string file in luaFiles)
        {
            string filex = XazEditor.FileUtil.ReadString(file);
            int ind = filex.IndexOf(sname.Replace("\r",""));
            if (ind != -1)
            {
                filex = filex.Substring(ind);
                Match mach = Regex.Match(filex, @"(?<=\{)[^}]*(?=\})");
                if (mach.Success)
                {
                    rawtxt = mach.Value;
                    MatchCollection results = Regex.Matches(mach.Value, @"[\w]+[\s]+(?=\=)");
                    foreach (Match mt in results)
                    {
                        parmarr.Add(mt.Value);
                        parmarr2.Add(mt.Value + "="+mt.Value);
                    }
                }
            }
        }
        parmInfo.sendParm = String.Join(",", parmarr);
        parmInfo.sendParmCode = String.Join(",", parmarr2);
        parmInfo.rawParm = rawtxt;
        return parmInfo;
    }
}