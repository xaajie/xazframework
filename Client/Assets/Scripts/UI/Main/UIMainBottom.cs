//----------------------------------------------------------------------------
//-- view
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using Xaz;
using static DoTweenUtil;

public class UIMainBottom : BaseUIMainBottom
{
    List<UserCategoryData> ownListData;
    protected override void OnOpened()
    {
        base.OnOpened();
        AddEventListener(EventEnum.CHANGE_CATEGORY, DelayRefresh);
        AddEventListener(EventEnum.UIFIHGT_REFRESHTop, RefreshTop);
        List<Const.CurrencyType> ownShowIds = new List<Const.CurrencyType>() { Const.CurrencyType.GOLD, Const.CurrencyType.FISH };
        SetData(ownShowIds);

    }

    public void SetData(List<Const.CurrencyType> ownShowIds)
    {
        ownListData = new List<UserCategoryData>();
        m_Toplist.Clear(false);
        for (int i = 0; i < ownShowIds.Count; i++)
        {
            UserCategoryData info = ModuleMgr.CategoryMgr.CreateCurrency(ownShowIds[i], 1);
            ownListData.Add(info);
            m_Toplist.AddData(info);
        }
        Refresh();
    }
    private void DelayRefresh()
    {
        scheduler.Timeout(delegate ()
        {
            Refresh();
        }, 0.6f);
    }
    public Vector3 GetCurrencyIconPos(int cid)
    {
        return GetCurrencyNode(cid).position;
    }

    public void ShowCurrencyIconEffect(int cid)
    {
        Transform vt = GetCurrencyNode(cid);
        Utils.SetActive(vt.gameObject, false);
        Utils.SetActive(vt.gameObject, true);
    }
    private Transform GetCurrencyNode(int cid)
    {
        for (int i = 0; i < ownListData.Count; i++)
        {
            if (ownListData[i].GetID() == (int)cid)
            {
                UITableViewCell crell = m_Toplist.GetCell(ownListData[i]);
                //TV_Toplist.Cell0 cell = this.GetCellView(m_Toplist, crell) as TV_Toplist.Cell0;
                return crell.transform;
            }
        }
        return null;
    }
    protected override void OnButtonClick(Component com)
    {
        base.OnButtonClick(com);
        if (com == this.m_SettingBN)
        {
            UIMgr.Open<UISettings>();
        }
    }
    public void Refresh()
    {
        RefreshTop();
    }
    public void RefreshTop()
    {
        this.m_Toplist.Refresh();
    }
    override protected void OnTableViewCellInit(UITableView tableView, UITableViewCell tableCell, object data)
    {
        base.OnTableViewCellInit(tableView, tableCell, data);
         if (tableView == m_Toplist)
        {
            UserCategoryData dat = data as UserCategoryData;
            TV_Toplist.Cell0 cell = this.GetCellView(tableView, tableCell) as TV_Toplist.Cell0;
            if(dat.itemId == (int)Const.CurrencyType.LVEXP)
            {

                SetFloat vt = delegate (float x)
                {
                    cell.Lvbar.value = x;
                };
                DoTweenUtil.DOToFloat(cell.Lvbar.value, Profile.Instance.user.GetlvexpVal(), 0.3f, vt, null);
                cell.Num.text = Profile.Instance.user.GetlvexpStr();
                //UserCategoryData showbox = Profile.Instance.user.GetLvOpenStock(Profile.Instance.user.GetLevel()+1);
                //cell.Box.SetBoxData(dat, false);
            }
            else
            {
                cell.Lvbar.value = 0;
                cell.Box.SetBoxData(dat, true);
                cell.Num.SetNum(dat.GetOwnNum());
            }
        }

    }

    override protected void OnTableViewCellClick(UITableView tableView, UITableViewCell tableCell, GameObject target, object data)
    {
        base.OnTableViewCellClick(tableView, tableCell, target, data);
        UIMgr.Open<UIShop>();
    }
}
