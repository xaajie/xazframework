using System.Collections;
using UnityEngine;
using Xaz;
/// <summary>
///  启动总入口
///  author xiejie
/// </summary>
public class GameMain : MonoBehaviour
{
    static private GameObject canvasObj = null;
    private IEnumerator Start()
    {
        //jietodo
        //必须在最前面，保证资源加载配置已初始化。ui已改为异步处理！！！
        Assets.InitializeLocal();
        UIMgr.Open<UILoading>();

        //预加载图集
        ResMgr.PreLoadSpriteAtals(Const.AtlasBuild);
        //不看到打印堆栈
        //Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        //不销毁的对象总节点
        DontDestroyOnLoad(GameObject.Find("DontDestroy"));
        AudioMgr.Instance.Init();
        //静态表解析
        yield return StartCoroutine(StaticDataMgr.Instance.Init());
        //禁止休眠
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        
        yield return XazHelper.waitFrame;
#if USE_CLOUD
        yield return StartCoroutine(CloudSDKMgr.Instance.InitSDK());
        yield return new WaitUntil(CloudSDKMgr.Instance.IsFinish);
#endif
        yield return StartCoroutine(SDKMgr.Instance.InitSDK(() =>
        {
            SDKMgr.Instance.SetFramesPerSecond();
        }));
        yield return new WaitUntil(SDKMgr.Instance.IsSDKFinish);
        //语言包配置
        Localization.language = "Local/Chinese.txt";
        NetMgr.Init();
        ModuleMgr.Init();
        yield return XazHelper.waitFrame;
        AudioMgr.Instance.Play(AudioEnum.bgm);
        yield return XazHelper.waitFrame;
        while (ResMgr.CheckGetSpriteAtlas(Const.AtlasBuild)==null)
        {
            yield return new WaitForEndOfFrame();
        }
        ModuleMgr.LoginMgr.EnterLogin();
    }

    //    void Update()
    //    {

    //#if UNITY_ANDROID || UNITY_EDITOR
    //        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Home))
    //        {
    //            Application.Quit();
    //#if UNITY_EDITOR
    //            UnityEditor.EditorApplication.isPlaying = false;
    //#endif
    //        }
    //#endif
    //        //World.Instance.Update();//不用点击
    //    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // 暂停游戏逻辑
            Time.timeScale = 0;
        }
        else
        {
            // 恢复游戏逻辑
            Time.timeScale = 1;
        }
        if (NetMgr.NetLogin != null)
        {
            NetMgr.NetLogin.SendSynUser();
        }
    }

    private void OnApplicationQuit()
    {
        if (NetMgr.NetLogin != null)
        {
            SDKMgr.Instance.UpdateOpTime();
            NetMgr.NetLogin.SendSynUser();
        }
        //RecordUtil.Save();
    }
    void OnDestroy()
    {

    }
    static public GameObject GetCanvasObj()
    {
        if (canvasObj == null)
        {
            canvasObj = GameObject.Find("Canvas");
        }
        return canvasObj;
    }
}
