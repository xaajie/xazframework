//----------------------------------------------------------------------------
//-- view
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Xaz;

public class UILogin : BaseUILogin
{

    protected override void OnOpened()
    {
        base.OnOpened();
        Refresh();
    }

    private void Refresh()
    {


    }

    override protected void OnTableViewCellInit(UITableView tableView, UITableViewCell tableCell, object data)
    {
        base.OnTableViewCellInit(tableView, tableCell, data);
      
    }

    override protected void OnTableViewCellClick(UITableView tableView, UITableViewCell tableCell, GameObject target, object data)
    {
        base.OnTableViewCellClick(tableView, tableCell, target, data);
       
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
        if (com== this.m_EnterBN)
        {
  
        }
    }
}
