using System.Collections.Generic;
using Table;
using Xaz;

public class UserSceneWorkerData : UserSceneWorkerDataBase
{
    public float moveSpeed;
    public int capacity;
    public UserSceneWorkerData() : base()
    {
    }

    public int GetChallengeOwnId()
    {
        return this.ownId > 0 ? this.ownId : 1;
    }
    public float GetRawSpeedVal()
    {
        return (float)GetInfo().walkspeed / 100;
    }

    public void RefreshData()
    {
        moveSpeed = GetRawSpeedVal() + ModuleMgr.AttrMgr.GetCountAddVal(GetRawSpeedVal(), (int)AttrEnum.movespeed, GetInfo().actorType);
        capacity = GetInfo().Capacity + (int)ModuleMgr.AttrMgr.GetCountAddVal(GetInfo().Capacity, (int)AttrEnum.handstacknum, GetInfo().actorType);
    }
    public actor GetInfo()
    {
        return StaticDataMgr.Instance.actorInfo[id];
    }

    public string GetDesc()
    {
        return GetInfo().desc;
    }

    public string GetHelpDesc()
    {
        return GetInfo().funcDesc;
    }

    public string GetName()
    {
        return GetInfo().name;
    }

    public string GetAtlas()
    {
        return GetInfo().atlas;
    }
    public string GetIcon()
    {
        return GetInfo().icon;
    }

    public string GetSmallIcon()
    {
        return GetInfo().headIcon;
    }

    public bool CanShowFloating()
    {
        return GetInfo().actorType == (int)Const.ActorType.Guard;
    }
    public int GetCapacity()
    {
        return capacity;
    }
    public float GetSpeedVal()
    {
        return moveSpeed;
    }

    public int GetActorType()
    {
        return GetInfo().actorType;
    }

    public int GetModelId()
    {

        return GetInfo().modelId;
    }

    public List<int> GetWorkForBuild(bool isputShelf)
    {
        if (isputShelf)
        {
            return GetInfo().putBuildIds;

        }
        else
        {
            return GetInfo().rawBuildIds;
        }
    }

    //修理时的数据处理
    public void SetOverSleepData()
    {
        this.IsSleeping = false;
        this.overSleepTime = TimeUtil.GetNowTicks();
    }

    public bool IsSleep()
    {
        return this.IsSleeping;
    }

    public int GetSleepCd()
    {
        return GetInfo().sleepCd;
    }

    public void CheckSetSleep()
    {
        if (!this.IsSleeping && GetSleepCd() > 0)
        {
            if (this.overSleepTime <= 0)
            {
                this.overSleepTime = TimeUtil.GetNowTicks();
                this.IsSleeping = false;
            }
            else
            {
                this.IsSleeping = (TimeUtil.GetNowTicks() - this.overSleepTime) >= GetSleepCd();
            }
            if(this.IsSleeping)
            {
                EventMgr.DispatchEvent(EventEnum.NOTICE_REFRESH);
            }
        }
    }
}
