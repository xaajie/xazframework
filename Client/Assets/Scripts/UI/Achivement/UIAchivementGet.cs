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

public class UIAchivementGet : BaseUIAchivementGet
{
    private UserAchivementData curInfo;
    protected override void OnOpened()
    {
        base.OnOpened();
    }

    protected override void OnClosed()
    {
        base.OnClosed();
    }

    public void SetAchivementInfo(UserAchivementData info)
    {
        curInfo = info;
        m_Awalist1.Clear(false);
        m_Awalist1.AddDataList(info.GetAwas());
        m_Awalist2.Clear(false);
        m_Awalist2.AddDataList(info.GetAdAwas());
        //RefreshDetail();
    }

    protected override void OnButtonClick(Component com)
    {
        base.OnButtonClick(com);
        if (com == m_AwardBN)
        {
            ModuleMgr.AchivementMgr.GetAchivementAward(curInfo, false);
            UIMgr.Close<UIAchivementGet>();
        }
        else if(com == m_AdBN)
        {
            ModuleMgr.AdMgr.ClickAd(AdEnum.AdType.Reward_Achivement, (adtype) =>
            {
                ModuleMgr.AchivementMgr.GetAchivementAward(curInfo, true);
                UIMgr.Close<UIAchivementGet>();
            });
        }

    }

    override protected void OnTableViewCellInit(UITableView tableView, UITableViewCell tableCell, object data)
    {
        base.OnTableViewCellInit(tableView, tableCell, data);
        if (tableView == m_Awalist1)
        {
            UserCategoryData dat = data as UserCategoryData;
            TV_Awalist1.Cell0 cell = this.GetCellView(tableView, tableCell) as TV_Awalist1.Cell0;
            cell.Box.SetBoxData(dat);
        }
        else if (tableView == m_Awalist2)
        {
            UserCategoryData dat = data as UserCategoryData;
            TV_Awalist2.Cell0 cell = this.GetCellView(tableView, tableCell) as TV_Awalist2.Cell0;
            cell.Box.SetBoxData(dat);
        }
    }
}
