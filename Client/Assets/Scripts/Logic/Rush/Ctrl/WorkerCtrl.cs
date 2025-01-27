using System.Collections;
using UnityEngine;
using Xaz;

public class WorkerCtrl : ActorController
{
    private UserSceneWorkerData ctrldata;
    protected override void Awake()
    {
        base.Awake();
    }

    public void SetCtrlData(UserSceneWorkerData info)
    {
        ctrldata = info;
        SetActorType(ctrldata.GetActorType());
        UpdateAttr();
    }

    public Vector3 GetSleepPoint()
    {
        return GetBindPoint();
    }
    public Vector3 GetBindPoint()
    {
        return RushManager.Instance.GetScenePoint(GetCtrlData().bindposId);
    }

    public bool GetProductFin()
    {
        return Stack.productIds.IndexOf(taskproductId) != -1 && Stack.GetNumById(taskproductId) >= taskproductNum;
    }

    public void CheckNeedSleep()
    {
        if (Stack.Count <= 0)
        {
            GetCtrlData().CheckSetSleep();
        }
    }

    public int Getuid()
    {
        return ctrldata.uid;
    }

    public UserSceneWorkerData GetCtrlData()
    {
        return ctrldata;
    }

    public bool IsWakeuping = false;
    public long startWakeupGuard = 0;

    public float GetWakeupBarVal()
    {
        if (startWakeupGuard <= 0)
        {
            return 0;
        }
        return (float)(TimeUtil.GetNowTicks() - this.startWakeupGuard) / Const.GuardBarDuration;
    }
    public void WakeupAction()
    {
        if (GetCtrlData().IsSleep() && !IsWakeuping)
        {
            IsWakeuping = true;
            if(GetCtrlData().GetActorType() == (int)Const.ActorType.Guard)
            {
                WakeupOver();
            }
            else
            {
                StartCoroutine(WakeupStepAction());
            }
        }
    }
    private IEnumerator WakeupStepAction()
    {
        SetSpineAnimState(SpineAnimCtrl.SpineAnimState.wakeup);
        yield return new WaitForSeconds(Const.FixWakeupDuration);
        WakeupOver();
    }

    private void WakeupOver()
    {
        GetCtrlData().SetOverSleepData();
        //SetWorkSpine();
        IsWakeuping = false;
        SetActorStep(Const.ActorStep.None);
        EventMgr.DispatchEvent(EventEnum.NOTICE_REFRESH);
    }

    public override void SetActorStep(Const.ActorStep st)
    {
        base.SetActorStep(st);
    }


    public override void UpdateAttr()
    {
        base.UpdateAttr();
        GetCtrlData().RefreshData();
        agent.SetAgentSpeed(ctrldata.GetSpeedVal());
        if (Stack != null)
        {
            Stack.MaxStack = ctrldata.GetCapacity();
        }
    }
}

