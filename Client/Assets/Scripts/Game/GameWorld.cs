//------------------------------------------------------------
// Xaz Framework
// 游戏世界
// Feedback: qq515688254
//------------------------------------------------------------

using System.Collections;
using Table;
using UnityEngine;
using Xaz;

public class GameWorld : MonoSingleton<GameWorld>
{
    public void EndMap()
    {
        if (RushManager.Instance != null)
        {
            RushManager.Instance.EndRush();
        }
    }

    public void ReloadGame()
    {
        StartCoroutine(ReloadGameAction());
    }

    private IEnumerator ReloadGameAction()
    {
        UIMgr.CloseAll();
        yield return new WaitForEndOfFrame();
        UIMgr.Open<UILoading>();
        ModuleMgr.MainMgr.isInit = true;
        Profile.Instance.ClearInfo();
        // NetMgr.Release();
        yield return new WaitForEndOfFrame();
        ModuleMgr.Release();
        EndMap();
        yield return new WaitForSeconds(1f);
        ModuleMgr.Init();
        ModuleMgr.LoginMgr.EnterLogin();
        UIMgr.Close<UILoading>();

    }

    public void StartGame()
    {
        PoolManager.Instance.BeginStart();
        EndMap();
        SceneMgr.Instance.LoadMap(ModuleMgr.ChallengeMgr.GetCurChallege().GetSceneName(), FinishLoad);
    }

    public void ChangeMap()
    {
        StartCoroutine(ChangeMapAction());
    }
    private IEnumerator ChangeMapAction()
    {
        UIMgr.CloseAll();
        UIMgr.Open<UILoading>();
        EndMap();
        yield return new WaitForEndOfFrame();
        SceneMgr.Instance.LoadMap(ModuleMgr.ChallengeMgr.GetCurChallege().GetSceneName(), FinishLoad);
        yield return new WaitForSeconds(1f);
        ModuleMgr.LoginMgr.EnterLogin();
        UIMgr.Close<UILoading>();
    }
    //场景加载完成
    public void FinishLoad()
    {
        if (UIMgr.Get<UIMain>() == null)
        {
            UIMgr.Open<UIMain>();
        }
        Scheduler.Timeout(delegate ()
        {
            ModuleMgr.HappeningMgr.GenerHappens(ModuleMgr.ChallengeMgr.GetCurChallege().GetHappens());
            RushManager.Instance.StartRush();
        }, 1);
    }
}