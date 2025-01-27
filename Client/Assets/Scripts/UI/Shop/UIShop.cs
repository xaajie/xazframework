//----------------------------------------------------------------------------
//-- …ÃµÍΩÁ√Ê
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using Xaz;

public class UIShop : BaseUIShop
{
    private List<UserShopInfoData> shopList;
    protected override void OnOpened()
    {
        base.OnOpened();
        AddEventListener(EventEnum.UIShop_REFRESH, RefreshDetail);
        UIMgr.SetTop<UIMainBottom>();
        Refresh();
        scheduler.Interval(delegate ()
        {
            UpdateInv();
        }, 0.2f);
    }

    private void UpdateInv()
    {
        RefreshDetail();
    }
    protected override void OnClosed()
    {
        base.OnClosed();
        UIMgr.Get<UIMainBottom>().Refresh();
        EventMgr.DispatchEvent(EventEnum.UIMAIN_REFRESH);
    }

    public void Refresh()
    {
        shopList = ModuleMgr.ShopMgr.GetShopShowInfoByType(ShopMgr.ShopType.COMMON);
        m_List.Clear(false);
        m_List.AddDataList(shopList);
        RefreshDetail();
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
            UIMgr.Close<UIShop>();
        }

    }

    string state_sellout = "3";
    string state_ad = "1";
    string state_get = "0";
    string state_cost = "2";
    override protected void OnTableViewCellInit(UITableView tableView, UITableViewCell tableCell, object data)
    {
        base.OnTableViewCellInit(tableView, tableCell, data);
        if (tableView == m_List)
        {
            TV_List.Cell0 cell = this.GetCellView(tableView, tableCell) as TV_List.Cell0;
            UserShopInfoData shopDat = data as UserShopInfoData;
            cell.Box.SetBoxData(shopDat);
            if(shopDat.GetAwardInfo().itemType == Const.Category.CURRENCY)
            {
                cell.Icon.transform.localScale = Vector3.one;
                Utils.SetActive(cell.Awanum.gameObject,true);
            }
            else
            {
                cell.Icon.transform.localScale = new Vector3(1,1.2f,1f);
                Utils.SetActive(cell.Awanum.gameObject, false);
            }

            if (shopDat.IsNoLimitNum())
            {
                cell.InfoTxt.text = string.Empty;
            }
            else
            {
                cell.InfoTxt.text = string.Format(Utils.GetLang("shop_adnum"), shopDat.GetCurNum(), shopDat.GetLimitMaxNum());
            }

            if (shopDat.CheckMatchBuyType(ShopMgr.ShopBuyType.ad))
            {
                cell.Uistate.SetState(shopDat.GetLeftBuyNum() <= 0 ? state_sellout : state_ad);
                int lefttime = shopDat.GetCdLeftTime();
                if (lefttime > 0)
                {
                    cell.Adcd.text = string.Format(Utils.GetLang("shopad_cd2"), TimeUtil.FormatTime(lefttime));
                }
                else
                {
                    cell.Adcd.text = Utils.GetLang("shopad_cd1");
                }
            }
            else if (shopDat.CheckMatchBuyType(ShopMgr.ShopBuyType.no))
            {
                cell.Uistate.SetState(shopDat.GetLeftBuyNum() <= 0 ? state_sellout : state_get);
            }
            else
            {
                cell.Uistate.SetState(shopDat.GetLeftBuyNum() <= 0 ? state_sellout : state_cost);
                cell.Costbox.SetBoxData(shopDat.GetCostInfo());
            }
        }
    }

    override protected void OnTableViewCellClick(UITableView tableView, UITableViewCell tableCell, GameObject target, object data)
    {
        base.OnTableViewCellClick(tableView, tableCell, target, data);
        if (tableView == m_List)
        {
            TV_List.Cell0 cell = this.GetCellView(tableView, tableCell) as TV_List.Cell0;
            UserShopInfoData shopDat = data as UserShopInfoData;
            if (shopDat.GetLeftBuyNum() > 0)
            {
                if (shopDat.CheckMatchBuyType(ShopMgr.ShopBuyType.ad))
                {
                    if (shopDat.GetCdLeftTime()>0)
                    {
                        UIMgr.ShowFlyTip(string.Format(Utils.GetLang("shopad_cdtip"), shopDat.GetCdLeftTime()));
                        return;
                    }
                    ModuleMgr.AdMgr.ClickAd(AdEnum.AdType.Reward_Shop, (adtype) =>
                    {
                        NetMgr.NetShop.SendBuy(shopDat, target.transform);
                    });
                }
                else
                {
                    NetMgr.NetShop.SendBuy(shopDat, target.transform);
                }
            }
        }

    }
}
