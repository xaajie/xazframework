//----------------------------------------------
//  查询反向依赖工具窗口
// modifyby :xiejie
//----------------------------------------------
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class ResFindReferencesWindow : EditorWindow
{
    private static int Wid = 400;
    private int UIINV = 10;
    private List<string> assetPaths = new List<string>() { };
    Vector2 scrollPosition;
    private float curPross = 0;
    private string infostr;
    private GUIStyle _style;

    public GUIStyle btnStyle {
        get
        {
            if (_style == null)
            {
                _style = new GUIStyle(GUI.skin.button);
                _style.richText = true;
                _style.alignment = TextAnchor.MiddleLeft;
            }
            return _style;
        }
        set
        {
            _style = value;
        }

    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField(infostr);
        EditorGUI.ProgressBar(new Rect(3, 25, Wid - 6, 20), curPross, (curPross >= 1 ? "查询结束" : "正在运行中"));
        EditorGUILayout.Space(UIINV*3);
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        foreach (string path in assetPaths)
        {
            string fileName = Path.GetFileName(path);
            string name = "<color=yellow>" + fileName + "</color>";
            if (GUILayout.Button(name, btnStyle))
            {
                string assetpath = XazEditorTools.FullPathToAssetPath(path);
                Selection.activeObject = AssetDatabase.LoadAssetAtPath(assetpath, typeof(Object));
            }
        }
        GUILayout.EndScrollView();
    }


    [MenuItem("Assets/查询反向依赖", false, 35)]
    private static void Find()
    {
        ResFindReferencesWindow resWin = EditorWindow.GetWindowWithRect(typeof(ResFindReferencesWindow), new Rect(0, 0, Wid, 400), false, "查询反向依赖") as ResFindReferencesWindow;
        resWin.Show();
        resWin.assetPaths.Clear();
        //EditorSettings.serializationMode = SerializationMode.ForceText;
        Object selectObject = Selection.activeObject;
        string filePath = AssetDatabase.GetAssetPath(selectObject);
        string fileName = selectObject.name;
        resWin.infostr = string.Format("检测资源:{0}", fileName);
        if (!string.IsNullOrEmpty(filePath))
        {
            string guid = AssetDatabase.AssetPathToGUID(filePath);
            //xiejietodo 。。。。。。。。
            //项目目录不同，查找的范围也不同
            List<string> includeExtensions = new List<string>() { ".prefab", ".unity", ".mat", ".asset" }; //需要搜索的资源文件（预制体 场景 材质）,可以自己根据情况自定义
            string[] files = Directory.GetFiles(Application.dataPath, "*", SearchOption.AllDirectories).Where(s => includeExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();

            int index = 0;
            int count = 0;
            //每帧检测个数
            int checkpeUpdate = 10;
            //EditorApplication.update，通用更新的委托，将我们的函数添加到此委托以获取更新。
            EditorApplication.update = delegate ()
            {
                for (int i = 0; i < checkpeUpdate; i++)
                {
                    string file = Path.GetFullPath(files[index]);
                    float percent = (float)(System.Convert.ToDouble(index) / System.Convert.ToDouble(files.Length));
                    resWin.curPross = percent;
                    //EditorUtility.DisplayProgressBar($"正在查找中... ({index}/{files.Length})", file, percent); 
                    if (Regex.IsMatch(File.ReadAllText(file), guid))
                    {
                        count++;
                        resWin.assetPaths.Add(file);
                    }
                    index++;
                    if (index >= files.Length)
                    {
                        resWin.curPross = 1;
                        //EditorUtility.ClearProgressBar();
                        EditorApplication.update = null;
                        index = 0;
                        break;
                    }
                }
            };
        }
    }
}
