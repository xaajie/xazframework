//----------------------------------------------------------------------------
//-- buff数据封装
//-- @author xiejie
//----------------------------------------------------------------------------
using System;
using Table;
using UnityEngine;
using Xaz;

public class UserBuffData : UserBuffDataBase
{
    public bool IsValid()
    {
        return GetEffectTime() <=0 || TimeUtil.GetNowInt() < GetEndTime();
    }

    public buff GetbuffInfo()
    {
        return StaticDataMgr.Instance.buffinfo[buffId];
    }

    public int GetBuffType()
    {
        return GetbuffInfo().buffType;
    }

    public float GetAttrVal()
    {
        return GetbuffInfo().attrval;
    }

    public bool IsAttrMatch(int attrId, int attrTar)
    {
        return GetbuffInfo().attrId == attrId && GetActionTarget() == attrTar;
    }
    public int GetActionTarget()
    {
        return GetbuffInfo().buffparm;
    }


    public int GetEffectTime()
    {
        return GetbuffInfo().effectTime;
    }

    public int GetLeftTime()
    {
        return GetEndTime() - TimeUtil.GetNowInt();
    }

    public int GetEndTime()
    {
        return createTime + GetbuffInfo().effectTime;
    }

    private int createActorId;
    public void CreatBuffEffect()
    {
        createTime = TimeUtil.GetNowInt();
        if(GetBuffType()==(int)Const.BuffType.Woker)
        {
            UserSceneWorkerDataBase info = new UserSceneWorkerDataBase();
            info.id = GetbuffInfo().buffparm;
            var randomCircle = UnityEngine.Random.insideUnitCircle * 1f;
            Vector3 newPos = RushManager.Instance.mainplayer.transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
            EffectMgr.Instance.PlayEffectPos("MagicPillarBlastBlue", newPos, RushManager.Instance.objNode.transform);
            UserSceneWorkerData nt = RushManager.Instance.GenerOneWorker(info, newPos);
            createActorId = nt.uid;
        }
    }
    public void DelBuffEffect()
    {
        if (GetBuffType() == (int)Const.BuffType.Woker)
        {
            for (int i = 0; i < RushManager.Instance.workers.Count; i++)
            {
                WorkerCtrl wt = RushManager.Instance.workers[i];
                if (wt.Getuid() == createActorId)
                {
                    RushManager.Instance.DelWorker(wt);
                    EffectMgr.Instance.PlayEffect("MagicPillarBlastBlue", wt.transform);
                }
            }
        }
    }
}
