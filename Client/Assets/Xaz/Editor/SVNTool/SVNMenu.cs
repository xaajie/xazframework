using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SVNMenu : MonoBehaviour
{
    [MenuItem("提交管理/[SVN]  Update &d", false, 0)]
    public static void SVNUpdate()
    {
        SVNTool.SvnToolAllUpdate();
    }

    [MenuItem("提交管理/[SVN]  Commit-新功能 &2", false, 0)]
    public static void SVNCommit()
    {
        SVNTool.CommitAtPaths(new List<string>() { Application.dataPath, Application.dataPath+"/../"+ "/ExcelData/" }, SVNTool.GetCodeLog(true));

    }

    [MenuItem("提交管理/[SVN]  Log", false, 0)]
    public static void SVNLog()
    {
        SVNTool.SvnToolAllLog();
    }

}
