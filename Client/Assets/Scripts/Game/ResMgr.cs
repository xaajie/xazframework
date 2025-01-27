//------------------------------------------------------------
// Xaz Framework
// 资源管理器
// 项目缓存策略
// Feedback: qq515688254
//------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// 资源管理Mgr
/// </summary>
public class ResMgr
{
    const bool DEBUG_LOG = false;
    const bool OPEN_DICT_FUNCTION = true;

    //缓存资源，不让Assets卸载
    private static Dictionary<string, Object> cacheHastSet = new Dictionary<string, Object>();


    ///// <summary>
    ///// 缓存异步加载的回调，避免同时异步加载同一个资源
    ///// </summary>
    //static Dictionary <string, List<ResLoadGameObjCallBack>> s_asyncGameObjectDict = new Dictionary<string,List<ResLoadGameObjCallBack>>();

    /// <summary>
    /// 缓存异步加载的回调，避免同时异步加载同一个资源
    /// </summary>
    static Dictionary<string, List<System.Action<Object>>> s_asyncObjectDict = new Dictionary<string, List<System.Action<Object>>>();


    #region 同步加载接口
    /// <summary>
    /// 资源加载，从资源路径，指定组件类型
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="type">指定组件类型</param>
    /// <returns></returns>
    static public Object LoadByPath(string path, System.Type type)
    {
        return Xaz.Assets.LoadAsset(path, type);
    }
    #endregion

    /// <summary>
    /// 卸载bundle
    /// </summary>
    static public void UnloadAssetBundles()
    {
        Xaz.Assets.UnloadUnusedAssets();
    }


    #region 异步加载接口
    /// <summary>
    /// 异步预加载除GameObject以外的资源，从资源路径
    /// </summary>
    /// <param name="path">资源路径.</param>
    /// <param name="path">资源类型.</param>
    static public void PreLoadOtherAssetAsync(string path, System.Type type)
    {
        LoadAssetAsync(path, type, null);
    }


    ///// <summary>
    ///// 异步加载资源，从资源路径
    ///// </summary>
    ///// <param name="path">资源路径.</param>
    ///// <param name="callback">回调方法.</param>
    //static public void LoadByPathToGameObjAsync(string path, System.Action<GameObject> callback)
    //{
    //    StartLoadByPathToGameObjAsync(path, callback, false);
    //}

    ///// <summary>
    ///// 实例化一个异步加载资源，从资源路径
    ///// </summary>
    ///// <param name="path">资源路径.</param>
    ///// <param name="callback">回调方法.</param>
    //static public void InstantiatePrefabAsync(string path, System.Action<GameObject> callback)
    //{
    //    StartLoadByPathToGameObjAsync(path, callback, true);
    //}
    static public void PreLoadSpriteAtals(string name)
    {
        LoadSpriteAtlas(name,null);
    }

    static public SpriteAtlas CheckGetSpriteAtlas(string name)
    {
        string key = Xaz.Assets.GetFileNameWithType(XazConfig.SpritPath + name, typeof(SpriteAtlas));
        if (cacheHastSet.ContainsKey(key))
        {
            return cacheHastSet[key] as SpriteAtlas;
        }
        else
        {
            return null;
        }
    }
    static public void LoadSpriteAtlas(string name, System.Action<Object> callback)
    {
        string key = Xaz.Assets.GetFileNameWithType(XazConfig.SpritPath + name, typeof(SpriteAtlas));
        if (cacheHastSet.ContainsKey(key))
        {
            if (callback != null)
            {
                callback(cacheHastSet[key]);
            }
        }
        else
        {
            LoadAssetAsync(XazConfig.SpritPath + name, typeof(SpriteAtlas), callback);
        }
    }
    /// <summary>
    /// 异步加载资源，从资源路径,指定组件类型
    /// </summary>
    /// <param name="path">资源路径.</param>
    /// <param name="type">资源类型.</param>
    /// <param name="callback">回调方法.</param>
    static public void LoadAssetAsync(string path, System.Type type, System.Action<Object> callback)
    {
        string name = Xaz.Assets.GetFileNameWithType(path, type);
        if (s_asyncObjectDict.ContainsKey(name))
        {
            if (callback != null)
            {
                s_asyncObjectDict[name].Add(callback);
            }
        }
        else
        {
            List<System.Action<Object>> list = new List<System.Action<Object>>();
            list.Add(callback);
            if (OPEN_DICT_FUNCTION)
            {
                s_asyncObjectDict.Add(name, list);
            }
            Xaz.Assets.LoadAssetAsync(name, type, ProObjCallBack);
        }
    }

    /// <summary>
    /// 清除所有异步加载的字典
    /// </summary>
    public static void ClearResMgrDict()
    {
        cacheHastSet.Clear();
        //s_asyncGameObjectDict.Clear();
        s_asyncObjectDict.Clear();
    }

    #endregion

    //static void ProCallBack(GameObject obj, string key)
    //{
    //    StartProCallBack(obj, key);
    //}

    static void ProObjCallBack(Object obj, string key)
    {
        if (s_asyncObjectDict.ContainsKey(key))
        {
            for (int i = 0; i < s_asyncObjectDict[key].Count; i++)
            {
                if (s_asyncObjectDict[key][i] != null)
                {
                    s_asyncObjectDict[key][i](obj);
                }
            }
            if (obj is SpriteAtlas && !cacheHastSet.ContainsKey(key))
            {
                cacheHastSet.Add(key, obj);
            }
            s_asyncObjectDict.Remove(key);
        }
    }

    //static void StartProCallBack(GameObject obj, string key)
    //{
    //    if (s_asyncGameObjectDict.ContainsKey(key))
    //    {
    //        if (DEBUG_LOG)
    //        {
    //            if (s_asyncGameObjectDict [key].Count > 1)
    //            {
    //                Debug.Log("Async load res return key " + key + " call back count : " + s_asyncGameObjectDict [key].Count);
    //            }
    //        }
    //        for (int i = 0; i < s_asyncGameObjectDict [key].Count; i++)
    //        {
    //            ResLoadGameObjCallBack data = s_asyncGameObjectDict [key] [i];
    //            if (data != null)
    //            {
    //                if(data.m_callBack != null)
    //                {
    //                    if (data.m_isInstance)
    //                    {
    //                        if (obj == null)
    //                        {
    //                            Debug.LogError("StartProCallBack Instantiate null " + key);
    //                        }else
    //                        {
    //                            if(data.m_parent != null)
    //                            {
    //                                data.m_callBack(GameObject.Instantiate(obj,data.m_parent));
    //                            }else
    //                            {
    //                                data.m_callBack(GameObject.Instantiate(obj));
    //                            }
    //                        }
    //                    } else
    //                    {
    //                        data.m_callBack(obj);
    //                    }
    //                }
    //            }
    //        }
    //        s_asyncGameObjectDict.Remove(key);
    //    }
    //}

    //static void StartLoadByPathToGameObjAsync(string path, System.Action<GameObject> callback, bool isInstance, Transform parent = null)
    //{
    //    string name = Xaz.Assets.GetFileNameWithType(path, typeof(GameObject));
    //    if (s_asyncGameObjectDict.ContainsKey(name))
    //    {
    //        if(callback != null)
    //        {
    //            if (DEBUG_LOG)
    //            {
    //                Debug.Log("!!!Repeat Async load GameObject res path " + path);
    //            }
    //            ResLoadGameObjCallBack data = new ResLoadGameObjCallBack(callback,isInstance,parent);
    //            s_asyncGameObjectDict [name].Add(data);
    //        }
    //    } else
    //    {    
    //        if (DEBUG_LOG)
    //        {
    //            Debug.Log("load res path : " + path + " GameObject isinstance : " + isInstance);
    //        }
    //        List<ResLoadGameObjCallBack> list = new List<ResLoadGameObjCallBack>();
    //        ResLoadGameObjCallBack data = new ResLoadGameObjCallBack(callback,isInstance,parent);
    //        list.Add(data);
    //        if (OPEN_DICT_FUNCTION)
    //        {
    //            s_asyncGameObjectDict.Add(name, list);  
    //        }
    //        Xaz.Assets.LoadAssetAsync<GameObject>(name, ProCallBack, false);
    //    }
    //}
}

//public class ResLoadGameObjCallBack
//{
//    public System.Action<GameObject> m_callBack;

//    public bool m_isInstance;

//    public Transform m_parent;

//    public ResLoadGameObjCallBack(System.Action<GameObject> callBack,bool isInstance,Transform parent)
//    {
//        m_callBack = callBack;
//        m_isInstance = isInstance;
//        m_parent = parent;
//    }
//}

