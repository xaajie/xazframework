//----------------------------------------------------------------------------
//-- ChallengeMgr模块管理
//-- @author xiejie
//----------------------------------------------------------------------------
using System.Collections.Generic;
using Table;

public class ChallengeMgr
{
    private Dictionary<int, UserChallengeShowData> challengeDataList= new Dictionary<int, UserChallengeShowData>();
    private UserChallengeShowData curChallengeInfo;
    private int curChallengeId = -1 ;

    public ChallengeMgr()
    {
        curChallengeId = -1;
    }

    public UserChallengeShowData GetCurChallege()
    {
        if (curChallengeId < 0)
        {
            SetCurChallege();
        }
        return GetChallengeShowData(curChallengeId);
    }

    public void SetCurChallege()
    {
        if (curChallengeId < 0)
        {
            foreach (challenge cha in StaticDataMgr.Instance.challengeInfo.Values)
            {
                curChallengeId = cha.id;
                break;
            }
        }
        foreach (UserChallengeShowData cha in challengeDataList.Values)
        {
            if (curChallengeId < cha.id && cha.IsOpen())
            {
                curChallengeId = cha.id;
            }
        }
    }
    public void CheckEnterChallengeScene(int challengeId)
    {
        if (curChallengeId == challengeId)
        {
            return;
        }
        curChallengeId = challengeId;
        GameWorld.Instance.ChangeMap();
    }

    public UserChallengeShowData GetChallengeShowData(int challengeId)
    {
        if (!challengeDataList.ContainsKey(challengeId))
        {
            UserChallengeShowData userchallenge = new UserChallengeShowData(challengeId);
            challengeDataList[challengeId] = userchallenge;
        }
        return challengeDataList[challengeId];
    }
    public List<UserChallengeShowData> GetShowlist(int chapter)
    {
        List<UserChallengeShowData> datList = new List<UserChallengeShowData>() { };
        foreach (challenge cha in StaticDataMgr.Instance.challengeInfo.Values)
        {
            if (cha.chapterId == chapter)
            {
                UserChallengeShowData userchallenge = GetChallengeShowData(cha.id);
                datList.Add(userchallenge);
                userchallenge.SetIndex(datList.Count);
            }
        }
        return datList;
    }


    public void ResetSaveChallenge()
    {

        Profile.Instance.user.level = 1;
        Profile.Instance.user.lvexp = 0;
        Profile.Instance.challenges.Clear();
        NetMgr.NetLogin.SendSynUser();
    }


    public void Release()
    {
        curChallengeInfo = null;
        challengeDataList.Clear();
        curChallengeId = -1;
    }
}
