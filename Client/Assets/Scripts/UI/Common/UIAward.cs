//----------------------------------------------------------------------------
//-- Íæ·¨½çÃæ
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using Xaz;

public class UIAward : BaseUIAward
{
    protected override void OnOpened()
    {
        base.OnOpened();
        AudioMgr.Instance.Play(AudioEnum.lvup);
    }

    public void SetData(List<UserCategoryData> awa)
    {
        bool hasAwa = awa.Count > 0;
        if (hasAwa)
        {
            m_List.Clear(false);
            m_List.AddDataList(awa);
        }
    }

    protected override void OnButtonClick(Component com)
    {
        base.OnButtonClick(com);
        if (com == m_OkBN)
        {
            UIMgr.Close<UIAward>();
        }
    }

    override protected void OnTableViewCellInit(UITableView tableView, UITableViewCell tableCell, object data)
    {
        base.OnTableViewCellInit(tableView, tableCell, data);
        if (tableView == m_List)
        {
            //DoTweenUtil.BounceTarget(tableCell.transform, null);
            UserCategoryData dat = data as UserCategoryData;
            TV_List.Cell0 cell = this.GetCellView(tableView, tableCell) as TV_List.Cell0;
            cell.Box.SetBoxData(dat);
        }
    }

}
