//----------------------------------------------------------------------------
//-- view
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------
using Table;
using UnityEngine;
using Xaz;

public class UIGM : BaseUIGM
{

    protected override void OnOpened()
    {
        base.OnOpened();
        m_List.Clear(false);
        foreach (gmorder cha in StaticDataMgr.Instance.gmorderInfo.Values)
        {
            m_List.AddData(cha.id);
        }
        Refresh();
    }

    private void Refresh()
    {


    }

    override protected void OnTableViewCellInit(UITableView tableView, UITableViewCell tableCell, object data)
    {
        base.OnTableViewCellInit(tableView, tableCell, data);
        if (tableView == m_List)
        {
            gmorder info = StaticDataMgr.Instance.gmorderInfo[(int)data];
            TV_List.Cell0 cell = this.GetCellView(tableView, tableCell) as TV_List.Cell0;
            cell.DescTxt.text = info.desc;
            cell.OrderTxt.text = info.order;

        }
    }

    override protected void OnTableViewCellClick(UITableView tableView, UITableViewCell tableCell, GameObject target, object data)
    {
        base.OnTableViewCellClick(tableView, tableCell, target, data);
        gmorder info = StaticDataMgr.Instance.gmorderInfo[(int)data];
        string[] are = info.order.Split(',');
        UIMgr.Get<UIGMEnter>().SetInputGM(are[0]);
        UIMgr.Close<UIGM>();
    }

    override protected void OnSubGroupCellInit(UISubGroup tableView, UITableViewCell tableCell, object data, string SubGroupname)
    {
        base.OnSubGroupCellInit(tableView, tableCell, data, SubGroupname);
       
    }

    override protected void OnSubGroupCellClick(UISubGroup tableView, UITableViewCell tableCell, GameObject target, object data, string SubGroupname)
    {
        base.OnSubGroupCellClick(tableView, tableCell, target, data, SubGroupname);
    }
    protected override void OnButtonClick(Component com)
    {
        base.OnButtonClick(com);
        if (com== this.m_CloseBN)
        {
            UIMgr.Close<UIGM>();
        }
    }
}
