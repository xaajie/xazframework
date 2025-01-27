//------------------------------------------------------------
// Xaz Framework
// 场景切换控制器
// 场景的load在热更策略上有特殊处理
// Feedback: qq515688254
//------------------------------------------------------------
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Xaz;
using System;

public class SceneLoader
{

    static private AsyncOperation asyncOperation = null;
    static private Action<float> progress = null;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="pro"></param>
    /// <param name="mode"></param>
    /// <param name="finish"></param>
    /// <param name="useInterEmptyScene">是否使用中间空场景用于减少内存尖刺</param>
    static public void LoadScene(string name, System.Action<float> pro, LoadSceneMode mode, System.Action finish, bool useInterEmptyScene)
    {
        if (useInterEmptyScene)
        {
            SceneManager.LoadScene("empty");
        }
        XazHelper.StartCoroutine(LoadCoroutineScene(name, mode, finish, useInterEmptyScene));
        progress = pro;
        Scheduler.Update(Update);
    }

    public static void DirectLoadScene(string path, LoadSceneMode mode)
    {
        //Assets.LoadScene(path);
        SceneManager.LoadScene(path, mode);
    }

    /// <summary>
    /// 直接加载场景
    /// </summary>
    /// <param name="name">Name.</param>
    /// <param name="mode">Mode.</param>
    public static void LoadSceneAsync(string name, string path, LoadSceneMode mode, bool isActive, string[] strs, System.Action finish)
    {
        XazHelper.StartCoroutine(StartLoadCoroutineScene(name, path, mode, isActive, strs, finish));
    }

    static private IEnumerator StartLoadCoroutineScene(string name, string path, LoadSceneMode mode, bool isActive, string[] strs, System.Action finish)
    {
        //Assets.LoadScene(path);
        asyncOperation = SceneManager.LoadSceneAsync(path, mode);
        yield return asyncOperation;
        if (!isActive)
        {
            SetSceneActive(name, false, strs);
        }
        if (finish != null)
        {
            finish();
        }
    }

    static private IEnumerator LoadCoroutineScene(string name, LoadSceneMode mode, System.Action finish, bool delay)
    {

        if (delay)
        {
            // 先阻塞切到空场景的话要显示进度必须要等2帧
            yield return XazHelper.waitFrame;
            yield return XazHelper.waitFrame;
        }
        Assets.LoadScene(name);
        asyncOperation = SceneManager.LoadSceneAsync(name, mode);
        yield return asyncOperation;
        yield return XazHelper.waitFrame;
        Scheduler.Remove(Update);
        asyncOperation = null;
        progress = null;
        if (finish != null)
        {
            finish();
        }

    }

    static private void Update()
    {
        if (progress != null && asyncOperation != null)
        {
            progress(asyncOperation.progress);
        }
    }


    static public void AsyncExecute(System.Action executeAction, System.Action finish)
    {
        XazHelper.StartCoroutine(AsyncExecuteCoroutine(executeAction, finish));
    }

    static private IEnumerator AsyncExecuteCoroutine(System.Action executeAction, System.Action finish)
    {
        if (executeAction != null)
        {
            executeAction();
        }

        yield return null;

        if (finish != null)
        {
            finish();
        }
    }


    /// <summary>
    /// 在多场景时设置场景激活不激活方法
    /// </summary>
    /// <param name="name">Name.</param>
    /// <param name="isUse">If set to <c>true</c> 是否激活.</param>
    /// <param name="isNotProRootObjName">要处理的根结点对象的名字.</param>
    public static void SetSceneActive(string name, bool isUse, string[] isNotProRootObjName)
    {
        Scene scene = SceneManager.GetSceneByName(name);
        if (scene != null)
        {
            GameObject[] objs = scene.GetRootGameObjects();
            if (isUse)
            {
                foreach (GameObject obj in objs)
                {
                    if (Array.IndexOf(isNotProRootObjName, obj.name) != -1)
                    {
                        Utils.SetActive(obj, true);
                    }
                }
                SceneManager.SetActiveScene(scene);
            }
            else
            {
                foreach (GameObject obj in objs)
                {
                    if (Array.IndexOf(isNotProRootObjName, obj.name)!=-1)
                    {
                        Utils.SetActive(obj, false);
                    }
                }
            }
        }
    }
}
