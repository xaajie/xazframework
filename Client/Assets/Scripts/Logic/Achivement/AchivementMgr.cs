//----------------------------------------------------------------------------
//-- AchivementMgr模块管理
//-- @author xiejie
//----------------------------------------------------------------------------

using System.Collections.Generic;
using Table;

public class AchivementMgr
{
    public Dictionary<int, List<UserAchivementData>> userAchives;
    public static int MAX_AchiveNum = 3;
    public AchivementMgr()
    {
    }

    public void SetInfo(List<UserAchivementChallengeDataBase> datalist)
    {
        userAchives = new Dictionary<int, List<UserAchivementData>>();
        if (datalist != null)
        {
            for (int i = 0; i < datalist.Count; i++)
            {
                List<UserAchivementData> rawachs = GetAchives(datalist[i].challengeId);
                for (int m = 0; m < datalist[i].datas.Count; m++)
                {
                    for (int n = 0; n < rawachs.Count; n++)
                    {
                        if (rawachs[n].GetID()== datalist[i].datas[m].achivementId)
                        {
                            rawachs[n].SetData(datalist[i].datas[m]);
                        }
                    }
                }
            }
        }
    }

    private int SortFilter(UserAchivementData a, UserAchivementData b)
    {
        //float xImportant = a.GetSliderVal();
        //float yImportant = b.GetSliderVal();

        //if (yImportant > xImportant)
        //{
        //    return 1;
        //}
        //else if(yImportant < xImportant)
        //{
        //    return -1;
        //}
        //else
        //{
        return a.GetSort().CompareTo(b.GetSort());
        //  }
    }
    public List<UserAchivementData> GetShowTopAchives(int curChallagetId)
    {
        List<UserAchivementData> resinfo = GetAchives(curChallagetId);
        resinfo.Sort(SortFilter);
        HashSet<int> seenTypes = new HashSet<int>(); // 记录出现过的类型
        List<UserAchivementData> uniqueTopThree = new List<UserAchivementData>();
        foreach (int challengeId in userAchives.Keys)
        {
            List<UserAchivementData> perres = userAchives[challengeId];
            for (int i = 0; i < perres.Count; i++)
            {
                if (perres[i].CanCrossChallengeShow())
                {
                    uniqueTopThree.Add(perres[i]);
                }
            }
        }
        if(uniqueTopThree.Count< MAX_AchiveNum)
        {
            foreach (UserAchivementData data in resinfo)
            {
                if (!data.NeedHide() && !seenTypes.Contains(data.GetAchiveType())) // 检查类型是否已存在
                {
                    uniqueTopThree.Add(data);
                    seenTypes.Add(data.GetAchiveType());

                    if (uniqueTopThree.Count >= MAX_AchiveNum) // 已取满三个
                        break;
                }
            }
        }
        return uniqueTopThree;
    }
    public List<UserAchivementData> GetAchives(int curChallagetId)
    {
        if (!userAchives.ContainsKey(curChallagetId))
        {
            UserChallengeShowData info = ModuleMgr.ChallengeMgr.GetChallengeShowData(curChallagetId);
            int groupId = info.GetAchiveGroupId();
            List<UserAchivementData> achs = new List<UserAchivementData>();
            foreach (achivement cha in StaticDataMgr.Instance.achivementInfo.Values)
            {
                if (cha.groupId == groupId)
                {
                    UserAchivementData newInfo = GenerAchivementData(cha);
                    achs.Add(newInfo);
                }
            }
            userAchives.Add(curChallagetId, achs);
        }
        return userAchives[curChallagetId];
    }

    public UserAchivementData GenerAchivementData(achivement cha)
    {
        UserAchivementData newInfo = new UserAchivementData();
        newInfo.achivementId = cha.id;
        return newInfo;
    }

    public void UpdateAchivement(Const.AchivementType aType, int[] parms=null)
    {
        List<UserAchivementData> dats = GetAchives(ModuleMgr.ChallengeMgr.GetCurChallege().GetID());
        for (int i = 0; i < dats.Count; i++)
        {
            dats[i].CheckMatchUpdate(aType, parms);
        }
        EventMgr.DispatchEvent(EventEnum.UIAchivement_REFRESH);
    }

    public bool HasAchivementRed()
    {
        List<UserAchivementData> dats = GetShowTopAchives(ModuleMgr.ChallengeMgr.GetCurChallege().GetID());
        for (int i = 0; i < dats.Count; i++)
        {
            if (dats[i].CanAward())
            {
                return true;
            }
        }
        return false;
    }

    public void GetAchivementAward(UserAchivementData dat,bool getad)
    {
        dat.getawa = true;
        List<UserCategoryData> awalist;
        if (getad)
        {
            awalist = dat.GetAdAwas();
        }
        else
        {
            awalist = dat.GetAwas();
        }
        UIMgr.Open<UIAward>(uiView => uiView.SetData(awalist));
        ModuleMgr.AwardMgr.AwardList(awalist, true);
        NetMgr.NetLogin.SendSynUser();
        EventMgr.DispatchEvent(EventEnum.UIAchivement_REFRESH);
    }
    public void Release()
    {
        userAchives.Clear();
    }

}
