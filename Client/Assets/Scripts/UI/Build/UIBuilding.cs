//----------------------------------------------------------------------------
//-- view
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xaz;

public class UIBuilding : BaseUIBuilding
{
    private bool isInit = false;
    protected override void OnOpened()
    {
        base.OnOpened();
        m_Title.text = Utils.GetLang(ModuleMgr.MainMgr.GetMainBtnDataById((int)Const.MainViewID.buildup).GetName());
        AddEventListener(EventEnum.CHANGE_CATEGORY, Refresh);
        List<UserSceneBuildData> listdat = ModuleMgr.BuildMgr.GetShowLvList();
        listdat.Sort(SortFilter);
        m_List.Clear(false);
        m_List.AddDataList(listdat);
        scheduler.Timeout(() =>
        {
            isInit = true;
        }, 0.5f);
        
    }

    protected override void OnClosed()
    {
        base.OnClosed();
        NetMgr.NetLogin.SendSynUser();
    }

    private int SortFilter(UserSceneBuildData a, UserSceneBuildData b)
    {
        bool aIsFullLv = a.IsFullLv();
        bool bIsFullLv = b.IsFullLv();

        if (aIsFullLv && !bIsFullLv)
        {
            return 1;// a 在 b 后面
        }
        else if (!aIsFullLv && bIsFullLv)
        {
            return -1;
        }
        return a.GetInfo().GetSort().CompareTo(b.GetInfo().GetSort());
    }
    private void Refresh()
    {
        m_List.Refresh();
    }



    protected override void OnButtonClick(Component com)
    {
        base.OnButtonClick(com);
        if (com == this.m_CloseBN || com == this.m_CloseBN2)
        {
            UIMgr.Close<UIBuilding>();
        }
    }

    string cellstate_full = "2";
    string cellstate_lvup = "0";
    string cellstate_change = "1";
    override protected void OnTableViewCellInit(UITableView tableView, UITableViewCell tableCell, object data)
    {
        base.OnTableViewCellInit(tableView, tableCell, data);
        if (tableView == m_List)
        {
            UserSceneBuildData info = data as UserSceneBuildData;
            TV_List.Cell0 cell = this.GetCellView(tableView, tableCell) as TV_List.Cell0;
            cell.Nametxt.text = info.GetInfo().GetName();
            cell.Lvtxt.text = string.Format("Lv.{0}", info.GetInfo().GetLevel());
            string modelImg = info.GetInfo().GetModelImg();
            Utils.SetActive(cell.Iconbg.gameObject, !string.IsNullOrEmpty(modelImg));
            cell.Iconbg.SetSprite(Const.AtlasBuild, info.GetInfo().GetModelImg());
            string iconImg = info.GetInfo().GetIcon();
            cell.Buildicon.SetSprite(info.GetInfo().GetAtlas(), info.GetInfo().GetIcon());
            Utils.SetActive(cell.Buildicon.gameObject, !string.IsNullOrEmpty(iconImg));
            bool isfull = info.IsFullLv();
            if (isfull)
            {
                cell.Cellstate.SetState(cellstate_full);
            }
            else
            {
                bool isChangeLvup = info.GetInfo().IsChangeLv();
                cell.Cellstate.SetState(isChangeLvup ? cellstate_change : cellstate_lvup);
                if (!info.IsAdUp())
                {
                    cell.Cost.SetBoxData(info.GetInfo().GetLvupCostInfo());
                }
                Utils.SetActive(cell.Cost.gameObject, !info.IsAdUp());
                Utils.SetActive(cell.Adcost.gameObject, info.IsAdUp());
            }
            cell.BnTxt.text = Utils.GetLang("buildlvup" + cell.Cellstate.currentStateName);

            Utils.SetActive(cell.CanlvupArrow.gameObject, info.CheckCanLvup());
            int maxlv;
            int curlv = info.GetInfo().GetBarVal(out maxlv);
            cell.BarTxt.text = string.Format("{0}/{1}", curlv, maxlv);
            cell.Slider.value = (float)curlv / (float)maxlv;
            cell.Attrlist.Clear(false);
            List<UserAttrData> nextinfos = info.GetNextAttrInfos();
            if (nextinfos != null)
            {
                cell.Attrlist.AddDataList(nextinfos);
            }
        }
    }

    override protected void OnSubGroupCellInit(UISubGroup tableView, UITableViewCell tableCell, object data, string SubGroupname)
    {
        base.OnSubGroupCellInit(tableView, tableCell, data, SubGroupname);
        UserAttrData info = data as UserAttrData;
        TV_Attrlist.Cell0 cell = this.GetSubCellView(tableView, tableCell, SubGroupname) as TV_Attrlist.Cell0;
        cell.AttrIcon.SetSprite(info.GetAtlas(), info.GetIcon());
        cell.AttrNum.text = string.Format("+{0}", info.GetChangeNum());
    }

    //override protected void OnSubGroupCellClick(UISubGroup tableView, UITableViewCell tableCell, GameObject target, object data, string SubGroupname)
    //{
    //    base.OnSubGroupCellClick(tableView, tableCell, target, data, SubGroupname);
    //}

    override protected void OnTableViewCellClick(UITableView tableView, UITableViewCell tableCell, GameObject target, object data)
    {
        base.OnTableViewCellClick(tableView, tableCell, target, data);
        if (tableView == m_List)
        {
            //升级处理
            UserSceneBuildData info = data as UserSceneBuildData;
            if (info.IsFullLv())
            {
                return;
            }
            if (info.IsAdUp())
            {
                ModuleMgr.AdMgr.ClickAd(AdEnum.AdType.Reward_Buildingup, (adtype) =>
                {
                    LevelupAction(info);
                });
            }
            else
            {
                UserCategoryData costInfo = info.GetInfo().GetLvupCostInfo();
                if (ModuleMgr.CategoryMgr.CheckOwn(costInfo, costInfo.GetNum(), Const.CheckTipType.Fly, true))
                {
                    ModuleMgr.AwardMgr.AwardListone(costInfo, false);
                    ModuleMgr.AwardMgr.AwardList(info.GetInfo().GetLvupAwardInfo(), true);
                    LevelupAction(info);
                }
            }
        }

    }

    BuildController foucusBuild = null;
    float preClickTime = 0;
    private void LevelupAction(UserSceneBuildData info)
    {
        UIMgr.Open<UIMask>();
        UserChallengeShowData curChallenge = ModuleMgr.ChallengeMgr.GetCurChallege();
        List<UserSceneBuildDataBase> builds = curChallenge.GetUserInfo().builds;
        UserSceneBuildDataBase newInfo = null;
        for (int m = 0; m < builds.Count; m++)
        {
            if (builds[m].id == info.id)
            {
                builds[m].level = info.level + 1;
                newInfo = builds[m];
            }
        }
        for (int i = 0; i < RushManager.Instance.builds.Count; i++)
        {
            BuildController tar = RushManager.Instance.builds[i];
            if (tar.GetCtrlData().GetBuildID() == newInfo.id)
            {
                foucusBuild = tar;
                tar.GetCtrlData().level = newInfo.level;
                tar.LevelUp();
                EffectMgr.Instance.PlayEffect("HealOnce", tar.transform);
                EffectMgr.Instance.PlayEffect("LevelupCylinderYellow", tar.transform);
            }
        }
        ModuleMgr.AchivementMgr.UpdateAchivement(Const.AchivementType.UnlockupBuild, new int[] { newInfo.id, newInfo.level });
        AudioMgr.Instance.Play(AudioEnum.lvup);
        Refresh();
        CameraMgr.Instance.SetFollowCam(CameraController.CameraMode.FOUCUS, foucusBuild.transform, true);
        UserChallengeShowData curInfo = ModuleMgr.ChallengeMgr.GetCurChallege();
        EventMgr.DispatchEvent(EventEnum.ChallengeInfo_REFRESH);
        if (curInfo.IsFinishBar() && curInfo.GetNextChallengeInfo() != null)
        {
            //锁定第二关，jie........
            UIMgr.Open<UIChallengeFin>();
            //UIMgr.Open<UIChallengeInfo>(uiview => uiview.SetData(curInfo.GetNextChallengeInfo(), true));
            UIMgr.Close<UIBuilding>();
        }
        UIMgr.Close<UIMask>();
    }

    public Transform GetNoviceBuildUp(int inx = 0)
    {
        if (isInit && m_List.GetCellAt(inx))
        {
            TV_List.Cell0 cell = this.GetCellView(m_List, m_List.GetCellAt(inx)) as TV_List.Cell0;
            return cell.LevelupBN.transform;
            //return m_List.GetCellAt(inx).transform;
        }
        else
        {
            return null;
        }
    }

}
