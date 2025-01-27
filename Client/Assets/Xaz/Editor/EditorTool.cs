//----------------------------------------------
//  编辑器辅助方法-列表及导出相关
//  @author xiejie 
//----------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class EditorTool
{
    public static string sceneInFolder = "";
    /// <summary>
    /// 获取scene列表
    /// </summary>
    /// <returns></returns>
    public static List<string> GetScenes()
    {
        List<string> es = new List<string>();
        es.Add("");
        EditorTool.FindFileList(Application.dataPath + sceneInFolder, ".unity", es, Application.dataPath + sceneInFolder);
        return es;
    }

    public static void OpenScenes(string mapName)
    {
        //if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(string.Format("{0}{1}.unity", Application.dataPath + sceneInFolder, mapName));
        }
    }
    public static void EmptyScene()
    {
        OpenScenes("map_empty");
    }
    public static List<string> GetScenes(string listFrom)
    {
        return GetFileAll(listFrom, ".unity");
    }

    public static List<string> GetPrefabs(string listFrom)
    {
        return GetFileAll(listFrom, ".prefab");
    }

    public static List<string> GetFileAll(string listFrom, string typeStr)
    {
        List<string> es = new List<string>();
        es.Add("");
        EditorTool.FindFileList(Application.dataPath + listFrom, typeStr, es, Application.dataPath + listFrom);
        return es;
    }
    /// <summary>
    /// 获得指定目录，指定类型，所有文件名称列表
    /// </summary>
    /// <param name="path"></param>
    /// <param name="suffix"></param>
    /// <param name="list"></param>
    /// <param name="basePath">基于此字段之外的路径去输出名称</param>
    /// <returns></returns>
    public static List<string> FindFileList(string path, string suffix, List<string> list, string basePath = null)
    {
        if (list == null)
        {
            list = new List<string>();
        }
        if (Directory.Exists(path))
        {
            foreach (string file in Directory.GetFiles(path))
            {
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.Extension == suffix)
                {
                    String name = GetPath(fileInfo, basePath);
                    name = name.Substring(0, name.Length - suffix.Length);
                    list.Add(name);
                }
            }
            foreach (string folder in Directory.GetDirectories(path))
            {
                FindFileList(folder, suffix, list, basePath);
            }
        }

        return list;
    }

    public static string GetPath(FileInfo file, string basePath = null)
    {
        return basePath == null ? file.Name : file.FullName.Replace("\\", "/").Replace(basePath, "");
    }

    public static string GetPathUrl(string url)
    {
        return Application.dataPath + url;
    }
    /// <summary>
    /// 开始写入报告 ,报告专用，会删除之前的报告内容
    /// </summary>
    public static void Report(string WritePath)
    {
        if (!Directory.Exists(WritePath))
        {
            Directory.CreateDirectory(WritePath);
        }
        foreach (string files in Directory.GetFiles(WritePath, "*.*", SearchOption.AllDirectories))
        {
            FileUtil.DeleteFileOrDirectory(files);
        }
        //打开文件夹
        System.Diagnostics.Process.Start(WritePath);
    }
    /// <summary>
    /// 写报告
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="content"></param>
    static public void WriteString(string filePath, string content)
    {
        File.WriteAllText(filePath, content.Replace(Environment.NewLine, "\r\n"), Encoding.UTF8);
    }
    /// <summary>
    /// 获得指定目录所有文件名称
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string[] GetFileNameByPath(string path)
    {
        string[] nameList = Directory.GetDirectories(path);
        for (int i = 0; i < nameList.Length; i++)
        {
            nameList[i] = nameList[i].Replace(path, "");
        }
        return nameList;
    }
    /// <summary>
    /// 形成选择列表
    /// </summary>
    /// <param name="label"></param>
    /// <param name="selected"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    public static string PopupList(string label, string selected, List<string> list)
    {
        return PopupArray(label, selected, list.ToArray());
    }

    public static string PopupArray(string label, string selected, string[] array)
    {
        Dictionary<string, int> map = new Dictionary<string, int>();
        if (array == null || array.Length <= 0)
        {
            return null;
        }
        for (int i = 0; i < array.Length; i++)
        {
            if (!map.ContainsKey(array[i]))
                map.Add(array[i], i);
        }
        if (label == null)
        {
            return array[EditorGUILayout.Popup(selected == null || !map.ContainsKey(selected) ? 0 : map[selected], array)];
        }
        else
        {
            return array[EditorGUILayout.Popup(label, selected == null || !map.ContainsKey(selected) ? 0 : map[selected], array)];
        }
    }
}
