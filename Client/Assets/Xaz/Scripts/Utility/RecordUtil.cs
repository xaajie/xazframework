//------------------------------------------------------------
// Xaz Framework
// 本地文件存储
// 存档功能
// Feedback: qq515688254
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using WeChatWASM;
using Xaz;

public class RecordUtil
{
//#if USE_WX
    private static WXFileSystemManager fileMgr = WX.GetFileSystemManager();
//#endif
    //游戏存档保存的跟目录
    public static string RecordRootPath
    {
        get
        {
#if USE_WX
            return WX.env.USER_DATA_PATH + "/Record/";
#elif (UNITY_EDITOR || UNITY_STANDALONE)
            return Application.dataPath + "/../Record/";
#else
            return Application.persistentDataPath + "/Record/";
#endif
        }
    }

    //游戏存档
    static Dictionary<string, string> recordDic = new Dictionary<string, string>();
    //标记某个游戏存档是否需要重写写入
    static List<string> recordDirty = new List<string>();
    //标记某个游戏存档是否需要删除
    static List<string> deleteDirty = new List<string>();
    //表示某个游戏存档读取时需要从新从文件中读取
    static List<string> readDirty = new List<string>();

    static RecordUtil()
    {
        ReadInitInfo();
    }

    public static void CreatPath()
    {
#if USE_WX
        if (fileMgr.AccessSync(RecordRootPath) != "access:ok")
        {
            try
            {
                fileMgr.MkdirSync(WX.env.USER_DATA_PATH + "/Record/", false);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
            }
        }
#endif

    }

    public static void ReadInitInfo()
    {
        readDirty.Clear();

#if USE_WX
        if (fileMgr.AccessSync(RecordRootPath) != "access:ok")
        {
            try
            {
                fileMgr.MkdirSync(WX.env.USER_DATA_PATH + "/Record/", false);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
            }
        }
        if (fileMgr.AccessSync(RecordRootPath) == "access:ok")
        {
            string[] files = fileMgr.ReaddirSync(RecordRootPath);
            foreach (string file in files)
            {
                string name = file.Split('.')[0];
                if (!readDirty.Contains(name))
                {
                    readDirty.Add(name);
                    Get(name);
                }
            }
        }
#else
        if (Directory.Exists(RecordRootPath))
        {
            foreach (string file in Directory.GetFiles(RecordRootPath, "*.record", SearchOption.TopDirectoryOnly))
            {
                string name = Path.GetFileNameWithoutExtension(file);
                if (!readDirty.Contains(name))
                {
                    readDirty.Add(name);
                    Get(name);
                }
            }
        }
#endif
        else
        {
            Debug.Log("没有找到" + RecordRootPath);
        }
    }

    //强制写入文件
    public static void Save()
    {
        foreach (string key in deleteDirty)
        {
            try
            {
#if USE_WX
                string path = string.Format("{0}{1}.record", RecordRootPath, key);
                if (recordDirty.Contains(key))
                {
                    recordDirty.Remove(key);
                }
                if (fileMgr.AccessSync(path) == "access:ok")
                {
                    fileMgr.UnlinkSync(path);
                }
#else
                string path = Path.Combine(RecordRootPath, key + ".record");
                if (recordDirty.Contains(key))
                {
                    recordDirty.Remove(key);
                }
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
#endif
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }
        deleteDirty.Clear();
        foreach (string key in recordDirty)
        {
            string value;
            if (recordDic.TryGetValue(key, out value))
            {
                if (!readDirty.Contains(key))
                {
                    readDirty.Add(key);
                }
                try
                {
#if USE_WX && !UNITY_EDITOR
                    string path = string.Format("{0}{1}.record", RecordRootPath, key);
                    fileMgr.WriteFileSync(path, value);
                    recordDic[key] = value;
#else
                    string path = Path.Combine(RecordRootPath, key + ".record");
                    recordDic[key] = value;
                    FileHelper.WriteString(path, value);
#endif
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message);
                }

            }
        }
        recordDirty.Clear();
    }



    public static void Set(string key, string value)
    {
        recordDic[key] = value;
        if (!recordDirty.Contains(key))
        {
            recordDirty.Add(key);
        }
// 为了重新进入程序的时刻，全部打开
//#if UNITY_EDITOR || UNITY_STANDALONE
        Save();
//#endif
    }


    public static string Get(string key)
    {
        return Get(key, string.Empty);
    }

    public static string Get(string key, string defaultValue)
    {
        if (readDirty.Contains(key))
        {
            try
            {
#if USE_WX
                string path = string.Format("{0}{1}.record", RecordRootPath, key);
                string readStr = fileMgr.ReadFileSync(path, "utf8");
                recordDic[key] = readStr;
#else
                string path = Path.Combine(RecordRootPath, key + ".record");
                string readStr = FileHelper.ReadString(path);
                recordDic[key] = readStr;
#endif
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }

            readDirty.Remove(key);
        }

        string value;
        if (recordDic.TryGetValue(key, out value))
        {
            return value;
        }
        else
        {
            return defaultValue;
        }
    }


    public static void Delete(string key)
    {
        if (recordDic.ContainsKey(key))
        {
            recordDic.Remove(key);

        }
        if (!deleteDirty.Contains(key))
        {
            deleteDirty.Add(key);
        }
        //删除要及时清楚重写
        Save();
    }
    public static void DeleteFile(string key)
    {
        if (recordDic.ContainsKey(key))
        {
            recordDic.Remove(key);

        }
        if (!deleteDirty.Contains(key))
        {
            deleteDirty.Add(key);
        }
    }
}