//----------------------------------------------------------------------------
//-- UserChallengeData数据封装
//-- @author xiejie
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Table;
using Xaz;

public class UserChallengeShowData : Data

{
    public int id;
    private int index;
    private int preIndex=-1;
    public UserChallengeShowData(int sid) : base()
    {
        id = sid;
        GetBuildBarMaxLv();
        foreach (challenge item in StaticDataMgr.Instance.challengeInfo.Values)
        {
            if(item.nextId == id)
            {
                preIndex = item.id;
                break;
            }
        }
    }

    public challenge GetInfo()
    {
        return StaticDataMgr.Instance.challengeInfo[id];
    }

    public int GetAchiveGroupId()
    {
        return GetInfo().agroupId;
    }
    public string GetSceneName()
    {
        return GetInfo().sceneName;
    }

    public int GetPerMoney()
    {
        return GetInfo().permoney;
    }

    public int GetUnlockGroupId()
    {
        return GetInfo().unlockGroupId;
    }
    public int GetPoolGroupId()
    {
        return GetLevelInfo().orderpool;
    }

    public int GetLevel()
    {
        return GetUserInfo().level;
    }
    public challengelv GetLevelInfo()
    {
        return StaticDataMgr.Instance.challengelvInfo[GetLevel()];
    }
    public int GetMaxCustomerNum()
    {
        return GetLevelInfo().maxCustomer;
    }

    public string GetIcon()
    {
        return GetInfo().img;
    }
    public string GetAtlas()
    {
        return Const.AtlasChallenge;
    }

    public void SetIndex(int inx)
    {
        index = inx;
    }

    public int GetCIndex()
    {
        return index;
    }

    public int GetChallengeMaxCount()
    {
        List<UserChallengeShowData> vt = ModuleMgr.ChallengeMgr.GetShowlist(GetInfo().chapterId);
        return vt.Count;
    }

    public int GetCurCustormerNum() => UnityEngine.Random.Range(GetLevelInfo().customerpernum[0], GetLevelInfo().customerpernum[1]);
    public int GenerNextCustormerCd() => UnityEngine.Random.Range(GetLevelInfo().customercd[0], GetLevelInfo().customercd[1]);
    public UserChallengeDataBase GetUserInfo()
    {
        return Profile.Instance.GetChallengeInfoCreate(id);
    }
    //public UserChallengeDataBase GetCheckUserInfo()
    //{
    //    return Profile.Instance.CheckHasChallengeInfo(id);
    //}

    public string GetName()
    {
        // return string.Format("{0}.{1}", GetCIndex(), GetInfo().name);
        return string.Format("{0}", GetInfo().name);
    }

    public int GetChapterId()
    {
        return GetInfo().chapterId;
    }

    public int GetNextId()
    {
        return GetInfo().nextId;
    }

    public string GetDesc()
    {
        return GetInfo().desc;
    }

    public UserChallengeShowData GetNextChallengeInfo()
    {
        if (GetInfo().nextId > 0)
        {
            return ModuleMgr.ChallengeMgr.GetChallengeShowData(GetInfo().nextId);
        }
        else
        {
            return null;
        }
    }

    public UserChallengeShowData GetPreChallengeInfo()
    {
        if (preIndex > 0)
        {
            return ModuleMgr.ChallengeMgr.GetChallengeShowData(preIndex);
        }
        else
        {
            return null;
        }
    }

    public bool IsFinishBar()
    {
        return GetBuildBarVal() >= 1;
    }

    public float GetBuildBarVal()
    {
        int maxval = GetBuildBarMaxLv();
        int curval = GetBuildBarLv();
        return ((float)curval / (float)maxval);
    }
    private int maxBuildLv=-1;
    private List<int> checkBarBuildLv = new List<int> { };
    public int GetBuildBarMaxLv()
    {
        if (maxBuildLv < 0)
        {
            maxBuildLv = 0;
            checkBarBuildLv = new List<int> {  };
            foreach (sceneunlock info in StaticDataMgr.Instance.sceneunlockInfo.Values)
            {
                if (info.unlockId == GetUnlockGroupId())
                {
                    List<UserCategoryData> unlockars = ModuleMgr.CategoryMgr.CreateFromList(info.category);
                    for (int m = 0; m < unlockars.Count; m++)
                    {
                        if (unlockars[m].itemType == Const.Category.BUILD&& checkBarBuildLv.IndexOf(unlockars[m].GetID())==-1)
                        {
                            maxBuildLv = maxBuildLv + ModuleMgr.BuildMgr.GetBuildMaxLv(unlockars[m].GetID());
                            checkBarBuildLv.Add(unlockars[m].GetID());
                        }
                    }
                }
            }
        }
        return maxBuildLv;
    }
    public int GetBuildBarLv()
    {
        int curlv = 0;
        for (int i = 0; i < checkBarBuildLv.Count; i++)
        {
            for (int m= 0; m < GetUserInfo().builds.Count; m++)
            {
                if (GetUserInfo().builds[m].id == checkBarBuildLv[i])
                {
                    curlv = curlv + GetUserInfo().builds[m].level;
                    break;
                }
            }
        }
        return curlv;
    }

    public int GetID()
    {
        return id;
    }

    public bool IsOpen()
    {
        UserChallengeShowData preInfo = GetPreChallengeInfo();
        return preInfo==null || preInfo.IsFinishBar();
    }

    public bool IsFirstChallenge()
    {
        UserChallengeShowData preInfo = GetPreChallengeInfo();
        return preInfo == null;
    }
    public bool IsFinishClickOpen()
    {
        if (IsFirstChallenge())
        {
            return true;
        }
        return IsOpen() && GetUserInfo().clickOpen;
    }

    public List<UserCategoryData> GetAward()
    {
        return ModuleMgr.CategoryMgr.CreateFromList(GetInfo().award);
    }

    public List<UserCategoryData> GetCost()
    {
        return ModuleMgr.CategoryMgr.CreateFromList(GetInfo().cost);
    }

    public List<string> GetGoldSupplyConfig()
    {
        return StaticDataMgr.Instance.goldsupplyInfo[GetInfo().goldsupply].config;
    }

    public int GetGoldSupplyNum(int shortageAmount)
    {
        List<string> ranges = GetGoldSupplyConfig();
        foreach (var range in ranges)
        {
            string[] parts = range.Split(':');
            string[] limits = parts[0].Split('~');

            int minValue = int.Parse(limits[0]);
            int maxValue = limits[1] == string.Empty? int.MaxValue : int.Parse(limits[1]);
            int popupValue = int.Parse(parts[1]);

            if (shortageAmount >= minValue && shortageAmount <= maxValue)
            {
                return popupValue;
            }
        }
        return 0; // 默认值
    }

    //------------------------------------------------------
    public void DelUnlockData(int id)
    {
        for (int i = 0; i < GetUserInfo().unlocks.Count; i++)
        {
            if (GetUserInfo().unlocks[i].id == id)
            {
                GetUserInfo().unlocks.RemoveAt(i);
                break;
            }
        }
    }

    public void GenerHappenInfo(int id)
    {
        if (id > 0)
        {
            UserHappeningDataBase vinfo = new UserHappeningDataBase();
            vinfo.infoid = id;
            vinfo.createTime = TimeUtil.GetNowInt();
            GetUserInfo().happenings.Add(vinfo);
            ModuleMgr.HappeningMgr.AddHappenInfo(vinfo);
        }
    }

    public List<UserHappeningDataBase> GetHappens()
    {
        return GetUserInfo().happenings;
    }

    public  List<UserSceneUnlockDataBase> GenerUnlockData(int sortId)
    {
        GetUserInfo().unlocks = new List<UserSceneUnlockDataBase>();
        if (sortId > 0)
        {
            List<int> unlockids = ModuleMgr.FightMgr.GetUnlockIdList(GetUnlockGroupId(), sortId);
            if (unlockids.Count > 0)
            {
                GetUserInfo().unlocks = new List<UserSceneUnlockDataBase>();
                for (int i = 0; i < unlockids.Count; i++)
                {
                    UserSceneUnlockDataBase vinfo = new UserSceneUnlockDataBase();
                    vinfo.id = unlockids[i];
                    GetUserInfo().unlocks.Add(vinfo);
                }
            }
        }
        return GetUserInfo().unlocks;
    }
    public List<UserCategoryData> GetOfflineAward()
    {
        return ModuleMgr.CategoryMgr.CreateFromList(GetInfo().offlineAward);
    }

}
