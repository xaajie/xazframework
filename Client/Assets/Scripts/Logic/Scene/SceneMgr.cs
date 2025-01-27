//------------------------------------------------------------
// Xaz Framework
// 场景管理器
// Feedback: qq515688254
//------------------------------------------------------------
using System;
using UnityEngine;

public delegate void FinshiEvent();

public class SceneMgr:MonoSingleton<SceneMgr>
{

    System.Action<float> pronum;
    Action finishFun;
    public SceneMgr(){}

    public void LoadMap(string mapName, Action finish)
    {
        finishFun = finish;
        SceneLoader.LoadScene(string.Format(XazConfig.ScenePath, mapName), pronum, UnityEngine.SceneManagement.LoadSceneMode.Single, LoadFinish, true);
    }


    public void LoadFinish()
    {
        //预留场景自己的处理

        //执行用户请求时的方法
        if (finishFun!=null)
        {
            finishFun();
        }
    }
}