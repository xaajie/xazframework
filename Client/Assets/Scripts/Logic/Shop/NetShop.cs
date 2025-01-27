//----------------------------------------------------------------------------
//-- NetShop模块协议
//-- @author xiejie
//----------------------------------------------------------------------------
using System.Collections.Generic;
using Table;
using UnityEngine;
using Xaz;
public class NetShop : NetModuleBase
{
    public NetShop()
    {
        this.Register(ProtocolEnum.SHOP_BUY_BACK, OnBuy);

    }

    public void SendBuy(UserShopInfoData shopInfo,Transform clicktar)
    {
        if (shopInfo.CheckCanBuy(Const.CheckTipType.Fly))
        {
            UserShopBuyDataBase vt = shopInfo.GetUserShopBuyInfo();
            if (vt == null)
            {
                vt = new UserShopBuyDataBase();
                vt.id = shopInfo.GetID();
                vt.buyTime = 0;
                vt.buynum = 0;
            }
            vt.buynum++;
            vt.buyTime = TimeUtil.GetNowInt();
            if (shopInfo.CheckMatchBuyType(ShopMgr.ShopBuyType.item))
            {
                ModuleMgr.AwardMgr.AwardListone(shopInfo.GetCostInfo(), false);
            }
            ModuleMgr.ShopMgr.CheckChangeAddBuyInfo(new List<UserShopBuyDataBase>() { vt });
            List<UserCategoryData> awalist = new List<UserCategoryData>() { shopInfo.GetAwardInfo() };
            //if(shopInfo.GetAwardInfo().itemType == Const.Category.CURRENCY)
            //{
            //    //ModuleMgr.AwardMgr.FlyAwardShow(awalist, clicktar.position, Const.GoldAwaEffect.flytipgold);
            //}
            //else
            //{
            //    UIMgr.Open<UIAward>(uiView => uiView.SetData(awalist));
            //}
            UIMgr.Open<UIAward>(uiView => uiView.SetData(awalist));
            ModuleMgr.AwardMgr.AwardList(awalist, true);
            NetMgr.NetLogin.SendSynUser();
            EventMgr.DispatchEvent(EventEnum.UIShop_REFRESH);
        }
        //ShopBuySend info = new ShopBuySend();
        //info.shopid = shopId;
        //info.IsAdFin = IsAdBuy;
        //this.Request(ProtocolEnum.SHOP_BUY, info);
    }

    private void OnBuy(INetData vt)
    {
        //ShopBuyBack res = vt as ShopBuyBack;
        //ModuleMgr.ShopMgr.CheckChangeAddBuyInfo(new List<UserShopBuyDataBase>() { res.buyInfo });
        //ModuleMgr.AwardMgr.ChangeItems(res.changeItems);
        //UIMgr.Open<UIAward>(uiView => uiView.SetData(ModuleMgr.AwardMgr.GetCategorys(res.changeItems.addItems)));
        //EventMgr.DispatchEvent(EventEnum.UIShop_REFRESH);
    }

    override public void Release()
    {
        base.Release();
    }
}

