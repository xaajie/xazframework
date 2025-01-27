//----------------------------------------------------------------------------
//-- 成就数据封装
//-- @author xiejie
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Table;

public class UserAchivementData : UserAchivementDataBase
{

    List<UserCategoryData> _awas;
    List<UserCategoryData> _adawas;
    private int needCount = -1;
    public void SetData()
    {

    }

    public int GetSort()
    {
        return GetInfo().sort;
    }
    public int GetID()
    {
        return achivementId;
    }

    public int GetNeedCount()
    {
        if (needCount < 0)
        {
            List<string> vt = GetParms();
            needCount =int.Parse(vt[vt.Count - 1]);
        }
        return needCount;
    }

    public int GetAchiveType()
    {
        return GetInfo().aType;
    }

    public bool CheckMatchUpdate(Const.AchivementType aType, int[] parms = null, bool change = true)
    {
        return CheckMatchUpdate((int)aType,parms,change);
    }
    public bool CheckMatchUpdate(int aType, int[] parms = null, bool change = true)
    {
        if (GetAchiveType() != (int)aType)
        {
            return false;
        }
        bool isMatch = false;
        List<string> tableparms = GetParms();
        switch (aType)
        {
            case (int)Const.AchivementType.UnlockupBuild:
                if (parms[0] == int.Parse(tableparms[0]))
                {
                    isMatch = true;
                    if (change)
                    {
                        count = Math.Max(count, parms[1]);
                    }
                }
                break;
            case (int)Const.AchivementType.FinishOrder:
                isMatch = true;
                if (change)
                {
                    count = count + 1;
                }
                break;
            case (int)Const.AchivementType.UnlockupBuildType:
                if (parms[0] == int.Parse(tableparms[0]))
                {
                    isMatch = true;
                    if (change)
                    {
                        count = count + 1;
                    }
                }
                break;
            case (int)Const.AchivementType.Attrup:
                if (parms[0] == int.Parse(tableparms[0]) && (parms[1] == int.Parse(tableparms[1])))
                {
                    isMatch = true;
                    if (change)
                    {
                        count = Math.Max(count, parms[2]);
                    }
                }
                break;
            case (int)Const.AchivementType.CountGold:
                isMatch = true;
                count = Math.Max(count, Profile.Instance.user.GetGold());
                break;
            case (int)Const.AchivementType.CountActor:
                if (tableparms[0].IndexOf(parms[0].ToString())!=-1)
                {
                    isMatch = true;
                    if (change)
                    {
                        count = count + 1;
                    }
                }
                break;
            case (int)Const.AchivementType.OpenChallenge:
                if (parms[0] == int.Parse(tableparms[0]))
                {
                    isMatch = true;
                    if (change)
                    {
                        count = count + 1;
                    }
                }
                break;
            default:
                break;
        }
        return isMatch;
    }
    public List<string> GetParms()
    {
        return GetInfo().param;
    }
    public achivement GetInfo()
    {
        return StaticDataMgr.Instance.achivementInfo[GetID()];
    }
    public string GetAtlas()
    {
        return GetInfo().atlas;
    }

    public string GetIcon()
    {
        return GetInfo().icon;
    }

    public string GetDesc()
    {
        return GetInfo().desc;
    }

    public string GetName()
    {
        return GetInfo().name;
    }

    public float GetSliderVal()
    {
        return (float)count / GetNeedCount();
    }

    public string GetSliderTxt()
    {
        return string.Format("{0}/{1}", count, GetNeedCount());
    }

    public List<UserCategoryData> GetAwas()
    {
        if (_awas == null)
        {
            _awas = ModuleMgr.CategoryMgr.CreateFromList(GetInfo().award);
        }
        return _awas;
    }

    public List<UserCategoryData> GetAdAwas()
    {
        if (_adawas == null)
        {
            _adawas = ModuleMgr.CategoryMgr.CreateFromList(GetInfo().adAward);
        }
        return _adawas;
    }

    public bool HadAdAward()
    {
        return GetInfo().adAward.Count>0;
    }
    //public bool IsFinish()
    //{
    //    return GetNeedCount()<=count;
    //}
    public bool NeedHide()
    {
        return getawa;
    }
    public bool CanAward()
    {
        return count >= GetNeedCount() && !getawa;
    }

    //可跨关卡显示的条目
    public bool CanCrossChallengeShow()
    {
        return GetAchiveType() == (int)Const.AchivementType.OpenChallenge && CanAward();
    }
    public bool CanShowGoto()
    {
        switch (GetAchiveType())
        {
            case (int)Const.AchivementType.UnlockupBuild:
            case (int)Const.AchivementType.UnlockupBuildType:
            case (int)Const.AchivementType.Attrup:
            case (int)Const.AchivementType.OpenChallenge:
                return true;
            case (int)Const.AchivementType.FinishOrder:
                return RushManager.Instance.customers.Count > 0;
            case (int)Const.AchivementType.CountActor:
            case (int)Const.AchivementType.CountGold:
                return false;
            default:
                return false;
        }
    }

    public void GotoAction()
    {
        switch (GetAchiveType())
        {
            case (int)Const.AchivementType.UnlockupBuild:
                List<string> parms = GetParms();
                BuildController buid = ModuleMgr.FightMgr.GetBuildByUid(int.Parse(parms[0]));
                if (buid==null)
                {
                    if (RushManager.Instance.unlockLands.Count > 0)
                    {
                        CameraMgr.Instance.SetFollowCam(CameraController.CameraMode.FOUCUS, RushManager.Instance.unlockLands[0].transform, true);
                    }
                }
                else
                {
                    UIMgr.Open<UIBuilding>();
                }
                break;
            case (int)Const.AchivementType.CountActor:
                break;
            case (int)Const.AchivementType.FinishOrder:
                for (int i = 0; i < RushManager.Instance.customers.Count; i++)
                {
                    CustomerCtrl vt = RushManager.Instance.customers[i];
                    CameraMgr.Instance.SetFollowCam(CameraController.CameraMode.FOUCUS, vt.transform,true);
                }
                break;
            case (int)Const.AchivementType.UnlockupBuildType:
                if (RushManager.Instance.unlockLands.Count > 0)
                {
                    CameraMgr.Instance.SetFollowCam(CameraController.CameraMode.FOUCUS, RushManager.Instance.unlockLands[0].transform, true);
                }
                break;
            case (int)Const.AchivementType.Attrup:
                UIMgr.Open<UIAttr>();
                break;
            case (int)Const.AchivementType.CountGold:
                break;
            case (int)Const.AchivementType.OpenChallenge:
                //锁定第二关，jie........
                UIMgr.Open<UIChallengeFin>();
                //UIMgr.Open<UIChallenge>();
                //UserChallengeShowData dat = ModuleMgr.ChallengeMgr.GetChallengeShowData(GetParms()[0]);
                //UIMgr.Open<UIChallengeInfo>(uiview => uiview.SetData(dat,false));
                break;
            default:
                UIMgr.Close<UIAchivement>();
                break;
        }
        UIMgr.Close<UIAchivement>();
    }
}
