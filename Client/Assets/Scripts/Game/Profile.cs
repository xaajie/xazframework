using System;
using System.Collections.Generic;
using Table;
/// <summary>
/// 用户信息
///  author xiejie
/// </summary>
public class Profile : MonoSingleton<Profile>
{
    public UserData user;

    public string sdkUserName = "test1";
    public string password = "test2";
    public string avatarUrl = string.Empty;
    public Dictionary<int, UserChallengeDataBase> challenges;
    public int fixChapterId = 1;
    //测试数
    public string testID = "";//1090007_5;1010001_11;1020001_2;1030002_3;1030005_20;//1010013_2;1060003_3
    //1020018_1;1060004_2;
    //1060023_1;1070001_2;1030018_12;1060017_16;1040012_6;1060023_5;1020016_19
    public string testItemID = "";
    public bool noTriggerRatio = false;

    public List<UserAttrupDataBase> attrIdLv;

    public Profile() : base()
    {
        user = new UserData();
        //List<string> arr = new List<string>();
        //for (int i = 1; i <= 20; i++)
        //{
        //    arr.Add("1030018_"+i);
        //}
        //testID = string.Join(";", arr);
    }

    public void ClearInfo()
    {
        user = new UserData();
        challenges.Clear();
    }

    public string GetRecordKey(string loginname, string loginkey)
    {
        return string.Format("{0}_{1}", loginname, loginkey);
    }

    public UserChallengeDataBase CheckHasChallengeInfo(int id)
    {
        if (challenges != null && challenges.ContainsKey(id))
        {
            return challenges[id];
        }
        return null;
    }
    public UserChallengeDataBase GetChallengeInfoCreate(int id)
    {
        UserChallengeDataBase rInfo = CheckHasChallengeInfo(id);
        if (rInfo == null)
        {
            challenge chainfo = StaticDataMgr.Instance.challengeInfo[id];
            rInfo = new UserChallengeDataBase();
            rInfo.id = id;
            rInfo.level = 0;
            rInfo.player = new UserScenePlayerDataBase();
            rInfo.player.id = chainfo.playerId;
            rInfo.player.ownId = chainfo.playerId;
            rInfo.unlocks = new List<UserSceneUnlockDataBase>();
            rInfo.builds = new List<UserSceneBuildDataBase>();
            rInfo.workers = new List<UserSceneWorkerDataBase>();
            rInfo.happenings = new List<UserHappeningDataBase>();
            List<int> unlockids = ModuleMgr.FightMgr.GetUnlockIdList(chainfo.unlockGroupId, 1);
            for (int i = 0; i < unlockids.Count; i++)
            {
                UserSceneUnlockDataBase vinfo = new UserSceneUnlockDataBase();
                vinfo.id = unlockids[i];
                rInfo.unlocks.Add(vinfo);
            }
            challenges.Add(id, rInfo);
        }
        return rInfo;
    }

    public void SetChallenge(List<UserChallengeDataBase> datalist)
    {
        challenges = new Dictionary<int, UserChallengeDataBase>();
        if (datalist != null)
        {
            for (int i = 0; i < datalist.Count; i++)
            {
                challenges.Add(datalist[i].id, datalist[i]);
            }
        }
    }

    public void SetAttrupList(List<UserAttrupDataBase> datalist)
    {
        attrIdLv = datalist;
        ModuleMgr.AttrMgr.CheckGetShowAttrupList();
    }

    public UserAttrupDataBase GetAttrupRecord(int attrGourpId)
    {
        for (int i = 0; i < attrIdLv.Count; i++)
        {
            if (attrIdLv[i].attrGroupId == attrGourpId)
            {
                return attrIdLv[i];
            }
        }
        return null;
    }
    public int GetAttrupLevel(int attrGourpId)
    {
        UserAttrupDataBase rt = GetAttrupRecord(attrGourpId);
        if(rt != null){
            return rt.level;
        }
        return 0;
    }

    public bool IsProductWorker(int actorType)
    {
        return actorType == (int)Const.ActorType.Worker_common ||
            actorType == (int)Const.ActorType.Worker_machine ||
            actorType == (int)Const.ActorType.Worker_ranch;
    }

    private int _initId = -1;
    public int GetInitChallengeId()
    {
        if (_initId < 0)
        {
            foreach (challenge cha in StaticDataMgr.Instance.challengeInfo.Values)
            {
                _initId = cha.id;
                break;
            }
        }
        return _initId;
    }

    public void UpdateChallengeInfo(UserChallengeDataBase newInfo)
    {
        if (challenges.ContainsKey(newInfo.id))
        {
            challenges[newInfo.id].SetData(newInfo);
        }
        else
        {
            challenges.Add(newInfo.id, newInfo);
        }
    }

    public int GetCurrencyNum(int itemId)
    {
        int num = 0;
        Const.CurrencyType _itemId = (Const.CurrencyType)Enum.Parse(typeof(Const.CurrencyType), itemId.ToString());
        switch (_itemId)
        {
            case Const.CurrencyType.GOLD:
                num = Profile.Instance.user.gold;
                break;
            case Const.CurrencyType.LVEXP:
                num = Profile.Instance.user.lvexp;
                break;
            case Const.CurrencyType.SKIPAD:
                num = Profile.Instance.user.skipad;
                break;
            case Const.CurrencyType.FISH:
                num = Profile.Instance.user.fish;
                break;
            default:
                break;
        }
        return num;
    }

}
