//----------------------------------------------------------------------------
//-- UserShopInfoData数据封装
//-- @author xiejie
//----------------------------------------------------------------------------
using System.Collections.Generic;
using Table;
using UnityEngine;
using Xaz;
using static ShopMgr;

public class UserShopInfoData : Data, IPackageData

{
    private int id;
    public UserShopInfoData(int sid) : base()
    {
        id = sid;
    }

    public int GetID()
    {
        return id;
    }

    public int GetSlotId()
    {
        return id;
    }


    //public bool IsAdBuy()
    //{
    //    return GetShopInfo().buyType == (int)ShopMgr.ShopBuyType.ad;
    //}

    public bool CheckMatchBuyType(ShopMgr.ShopBuyType pt)
    {
        return GetShopInfo().buyType == (int)pt;
    }
    //用户购买记录
    public UserShopBuyDataBase GetUserShopBuyInfo()
    {
        return ModuleMgr.ShopMgr.GetShopBuyInfo(id);
    }
    public shop GetShopInfo()
    {
        return StaticDataMgr.Instance.shopInfo[id];
    }

    public int GetSort()
    {
        return GetShopInfo().sort;
    }

    public bool CheckCanBuy(Const.CheckTipType tipType)
    {
        if (GetLeftBuyNum() <= 0)
        {
            return false;
        }
        if (CheckMatchBuyType(ShopBuyType.no) || CheckMatchBuyType(ShopBuyType.ad))
        {
            return true;
        }
        else
        {
            UserCategoryData info = GetCostInfo();
            return ModuleMgr.CategoryMgr.CheckOwn(info, info.GetNum(), tipType);
        }
    }


    public int GetLimitMaxNum()
    {
        if (IsNoLimitNum())
        {
            return 99999;
        }
        return GetShopInfo().limitNum[1];
    }

    public int GetLimitType()
    {
        return GetShopInfo().limitNum[0];
    }
    public bool IsCheckLimitDay()
    {
        return GetLimitType() == (int)ShopMgr.ShopBuyLimit.day;
    }

    public bool IsNoLimitNum()
    {
        return GetLimitType() == (int)ShopMgr.ShopBuyLimit.no;
    }
    public int GetCurNum()
    {
        UserShopBuyDataBase buyrtinfo = GetUserShopBuyInfo();
        if (IsCheckLimitDay() && buyrtinfo != null)
        {
            if (!TimeUtil.IsSameDay(buyrtinfo.buyTime))
            {
                buyrtinfo.buynum = 0;
            }
        }
        if (buyrtinfo != null)
        {
            return buyrtinfo.buynum;
        }
        return 0;
    }

    public int GetCdLeftTime()
    {
        UserShopBuyDataBase buyrtinfo = GetUserShopBuyInfo();
        if (buyrtinfo != null)
        {
            return Mathf.Max(0,buyrtinfo.buyTime+GetShopInfo().adBuyCD - TimeUtil.GetNowInt());
        }
        else
        {
            return 0;
        }
    }
    public int GetLeftBuyNum()
    {
        int maxbuyNum = GetLimitMaxNum();
        return maxbuyNum - GetCurNum();
    }

    UserCategoryData costInfo;
    UserCategoryData buyInfo;
    public UserCategoryData GetCostInfo()
    {
        if (!string.IsNullOrEmpty(GetShopInfo().cost))
        {
            if (costInfo == null)
            {
                costInfo = ModuleMgr.CategoryMgr.CreateFrom(GetShopInfo().cost);
            }
            return costInfo;
        }
        return costInfo;
    }

    public UserCategoryData GetAwardInfo()
    {
        buyInfo = ModuleMgr.CategoryMgr.CreateFromParm(GetShopInfo().itemType, GetShopInfo().itemId, GetShopInfo().itemNum);
        return buyInfo;
    }

    public string GetDesc()
    {
        return string.Empty;
    }

    public string GetIcon()
    {
        if (GetShopInfo().icon != string.Empty)
        {
            return GetShopInfo().icon;
        }
        else
        {
            return GetAwardInfo().GetIcon();
        }
    }

    public int GetQuality()
    {
        return GetAwardInfo().GetQuality();
    }
    public string GetName()
    {
        if (GetShopInfo().name != string.Empty)
        {
            return GetShopInfo().name;
        }
        else
        {
            return GetAwardInfo().GetName();
        }
    }

    public int GetNum()
    {
        return GetAwardInfo().GetNum();
    }

    public int GetOwnNum()
    {
        return GetAwardInfo().GetOwnNum();
    }
    public int GetItemType()
    {
        return GetAwardInfo().GetItemType();
    }

    public int GetSType()
    {
        return GetAwardInfo().GetSType();
    }
    public string GetAtlas()
    {
        if (GetShopInfo().icon != string.Empty)
        {
            return GetShopInfo().atlas;
        }
        else
        {
            return GetAwardInfo().GetAtlas();
        }
    }

    public bool HasDiscount()
    {
        return false;
    }

    //public float DiscountVal()
    //{
    //    return (float)GetShopInfo().discount/10;
    //}
}
