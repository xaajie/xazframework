//----------------------------------------------------------------------------
//-- …ÃµÍΩÁ√Ê
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using Xaz;

public class UIAchivement : BaseUIAchivement
{
    protected override void OnOpened()
    {
        base.OnOpened();
        AddEventListener(EventEnum.UIAchivement_REFRESH, Refresh);
        Refresh();
        //scheduler.Interval(delegate ()
        //{
        //    UpdateInv();
        //}, 0.2f);
    }

    private void UpdateInv()
    {
        RefreshDetail();
    }
    protected override void OnClosed()
    {
        base.OnClosed();
        EventMgr.DispatchEvent(EventEnum.UIMAIN_REFRESH);
    }

    public void Refresh()
    {
        UserChallengeShowData curchall = ModuleMgr.ChallengeMgr.GetCurChallege();
        List<UserAchivementData> resinfo= ModuleMgr.AchivementMgr.GetShowTopAchives(curchall.GetID());
        m_List.Clear(false);
        m_List.AddDataList(resinfo);
        //RefreshDetail();
    }

    public void RefreshDetail()
    {
        m_List.Refresh();
    }

    protected override void OnButtonClick(Component com)
    {
        base.OnButtonClick(com);
        if (com == m_CloseBN)
        {
            UIMgr.Close<UIAchivement>();
        }

    }

    override protected void OnTableViewCellInit(UITableView tableView, UITableViewCell tableCell, object data)
    {
        base.OnTableViewCellInit(tableView, tableCell, data);
        if (tableView == m_List)
        {
            UserAchivementData dat = data as UserAchivementData;
            TV_List.Cell0 cell = this.GetCellView(tableView, tableCell) as TV_List.Cell0;
            cell.Icon.SetSprite(dat.GetAtlas(),dat.GetIcon());
            cell.Desc.text = dat.GetDesc();
            cell.Title.text = dat.GetName();
            cell.SliderTxt.text = dat.GetSliderTxt();
            cell.Slider.value = dat.GetSliderVal();
            bool canawa = dat.CanAward();
            Utils.SetActive(cell.GetBN.gameObject, canawa);
            Utils.SetActive(cell.GoBN.gameObject, !canawa && dat.CanShowGoto());
            cell.Awalist.Clear(true);
            cell.Awalist.AddDataList(dat.GetAwas());
        }
    }

    override protected void OnSubGroupCellInit(UISubGroup tableView, UITableViewCell tableCell, object data, string SubGroupname)
    {
        base.OnSubGroupCellInit(tableView, tableCell, data, SubGroupname);
        TV_Awalist.Cell0 cell = this.GetSubCellView(tableView, tableCell, SubGroupname) as TV_Awalist.Cell0;
        cell.Box.SetBoxData(data as UserCategoryData);
    }
    override protected void OnTableViewCellClick(UITableView tableView, UITableViewCell tableCell, GameObject target, object data)
    {
        base.OnTableViewCellClick(tableView, tableCell, target, data);
        if (tableView == m_List)
        {
            TV_List.Cell0 cell = this.GetCellView(tableView, tableCell) as TV_List.Cell0;
            UserAchivementData dat = data as UserAchivementData;
            if (target == cell.GetBN.gameObject)
            {
                if (dat.HadAdAward())
                {
                    UIMgr.Open<UIAchivementGet>(uiview => uiview.SetAchivementInfo(dat));
                }
                else
                {
                    ModuleMgr.AchivementMgr.GetAchivementAward(dat,false);
                }
            }
            else if (target == cell.GoBN.gameObject)
            {
                dat.GotoAction();
            }
        }

    }
}
