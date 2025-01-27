using System.Collections.Generic;
using Table;
using UnityEngine;

public class UserSceneUnlockData : UserSceneUnlockDataBase
{
    private List<UserCategoryData> unlockDat;
    public UserSceneUnlockData() : base()
    {
    }

    public void RefreshData()
    {
      
    }

    public int GetOpenLevel()
    {
        return GetUnlockInfo().challengeLv;
    }
    public sceneunlock GetUnlockInfo()
    {
        return StaticDataMgr.Instance.sceneunlockInfo[this.id];
    }
    public int GetUnlockIndex()
    {
        return GetUnlockInfo().sort;
    }

    public int GetNextUnlockIndex()
    {
        return GetUnlockInfo().sort + 1;
    }
    public int GetUnlockPay()
    {
        return this.pay;
    }

    public int GetUnlockPrice()
    {
        return GetUnlockInfo().cost;
    }

    public int GetInitBuildlv()
    {
        return 1;
    }
    public List<UserCategoryData> GetUnlockTargets()
    {
        if (unlockDat == null)
        {
            unlockDat = ModuleMgr.CategoryMgr.CreateFromList(GetUnlockInfo().category);
        }
        return unlockDat;
    }

    public int GetHappenId()
    {
        return GetUnlockInfo().happenId;
    }

    public int GetBuyPointId()
    {
        return GetUnlockInfo().posId;
    }

    public int GetCreatePointId(int index)
    {
        return GetUnlockInfo().createPos[index];
    }
    //public int GetModelId(int index)
    //{
    //    int modelid = -1;
    //    switch (GetUnlockInfo().category)
    //    {
    //        case (int)Const.Category.BUILD:
    //            UserBuildvInfoData buildinfo = ModuleMgr.BuildMgr.GetBuildlvInfo(GetUnlockTarget().itemId, GetInitBuildlv());
    //            modelid = buildinfo.GetModelId();
    //            break;
    //        case (int)Const.Category.ACTOR:
    //            modelid = GetUnlockTarget().GetActorInfo().modelId;
    //            break;
    //    }
    //    return modelid;

    //}

}
