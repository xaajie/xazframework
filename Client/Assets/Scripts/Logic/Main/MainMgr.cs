using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using Xaz;

public class MainMgr
{
    public bool isInit = false;
    private Dictionary<int, UserMainBtnData> mainData = new Dictionary<int, UserMainBtnData>();
    public MainMgr()
    {
        
    }

    public Vector3 GetCurrencyEndTarget(int cid)
    {
        if (UIMgr.Get<UIMainBottom>() != null)
        {
            return UIMgr.Get<UIMainBottom>().GetCurrencyIconPos(cid);
        }
        else
        {
            return Vector3.zero;
        }
    }
    public List<UserMainBtnData> GetListByType(int typev)
    {
        List<UserMainBtnData> list = new List<UserMainBtnData>() { };
        foreach (mainshow cha in StaticDataMgr.Instance.mainshowInfo.Values)
        {
            if (cha.showtype == typev && !cha.debugClose)
            {
                list.Add(GetMainBtnDataById(cha.id));
            }
        }
        list.Sort((a, b) => a.GetSort().CompareTo(b.GetSort()));
        return list;
    }

    public UserMainBtnData GetMainBtnDataById(int key)
    {
        if(!mainData.ContainsKey(key))
        {
            UserMainBtnData dat = new UserMainBtnData();
            dat.id = key;
            mainData.Add(key, dat);
        }
        return mainData[key];
    }

    public bool CheckShowOffLine()
    {
        bool hasaward = false;
        int intev = TimeUtil.GetNowInt() - Profile.Instance.user.onLineTime;
        if (Profile.Instance.user.onLineTime > 0 && intev >= Constant.Offline_MinTime)
        {
            intev = Math.Min(intev, Constant.Offline_MaxTime);
            Profile.Instance.user.onLineAwardTime += intev;
            if(GetShowOffLineAward() != null)
            {
                hasaward = true;
            }
        }
        //Profile.Instance.user.onLineTime = TimeUtil.GetNowInt();
        NetMgr.NetLogin.SendSynUser();
        return hasaward;
    }
    public List<UserCategoryData> GetShowOffLineAward()
    {
        Dictionary<string, UserCategoryData> dataDic = new Dictionary<string, UserCategoryData>();
        List<UserCategoryData> award = null;
        float rate;
        foreach (challenge cha in StaticDataMgr.Instance.challengeInfo.Values)
        {
            UserChallengeShowData info = ModuleMgr.ChallengeMgr.GetChallengeShowData(cha.id);
            rate = 0;
            if (info.IsFinishBar())
            {
                rate = 1;
            }
            else if (info.IsOpen())
            {
                rate = info.GetBuildBarVal();
            }
            else
            {
                continue;
            }
            if (rate == 0)
            {
                continue;
            }
            foreach (UserCategoryData data in info.GetOfflineAward())
            {
                string key = data.GetItemType() + "_" + data.GetID();
                if (dataDic.ContainsKey(key))
                {
                    dataDic[key].itemNum += (int)Math.Ceiling(data.itemNum * rate);
                }
                else
                {
                    data.itemNum = (int)Math.Ceiling(data.itemNum * rate);
                    dataDic[key] = data;
                }
            }
        }
        if (dataDic.Count > 0)
        {
            award = new List<UserCategoryData>();
            foreach (string key in dataDic.Keys)
            {
                award.Add(dataDic[key]);
            }
        }
        return award;
    }
    // Update is called once per frame
    public void Release()
    {
        
    }
}
