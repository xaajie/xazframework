//----------------------------------------------------------------------------
//-- 宝箱模块管理
//-- @author xiejie
//----------------------------------------------------------------------------
using System.Collections.Generic;
using Table;
using UnityEngine;
using Xaz;
public class ShopMgr
{
    public enum ShopType
    {
        COMMON=1
    }

    public enum ShopBuyLimit
    {
        forever=1,
        day=2,
        no=3,
    }

    public enum ShopBuyType
    {
        ad=1,
        item=2,
        no=0,
    }
    public Dictionary<int, UserShopBuyDataBase> userShopBuyInfos;
    public ShopMgr()
    {
        
    }

    public void SetShopBuyList(List<UserShopBuyDataBase> datalist)
    {
        userShopBuyInfos = new Dictionary<int, UserShopBuyDataBase>();
        CheckChangeAddBuyInfo(datalist);
    }

    public void CheckChangeAddBuyInfo(List<UserShopBuyDataBase> datalist)
    {
        if (datalist != null)
        {
            for (int i = 0; i < datalist.Count; i++)
            {
                UserShopBuyDataBase pt = GetShopBuyInfo(datalist[i].id);
                if (pt != null)
                {
                    pt.SetData(datalist[i]);
                }
                else
                {
                    userShopBuyInfos.Add(datalist[i].id, datalist[i]);
                }
            }
        }
    }

    public UserShopBuyDataBase GetShopBuyInfo(int shopId)
    {
        if (userShopBuyInfos != null && userShopBuyInfos.ContainsKey(shopId))
        {
            return userShopBuyInfos[shopId];
        }
        return null;
    }

    public shopset GetShopSetInfo(ShopType vt)
    {
        foreach (shopset cha in StaticDataMgr.Instance.shopsetInfo.Values)
        {
            if (cha.shopType == (int)vt)
            {
                return cha;
            }
        }
        return null;
    }

    /// <summary>
    /// 后端接入后，这里转后端逻辑
    /// </summary>
    /// <param name="vt"></param>
    /// <returns></returns>
    public List<UserShopInfoData> GetShopShowInfoByType(ShopType vt)
    {
        List < UserShopInfoData > showInfo = new List < UserShopInfoData >();
        shopset sInfo = GetShopSetInfo(vt);
        foreach (shop cha in StaticDataMgr.Instance.shopInfo.Values)
        {
            if (sInfo.shopGroupId.IndexOf(cha.shopGroup)!=-1)
            {
                UserShopInfoData shopInfo = new UserShopInfoData(cha.id);
                if (shopInfo.GetAwardInfo()!=null)
                {
                    showInfo.Add(shopInfo);
                }
            }
        }
        showInfo.Sort((x, y) => { return x.GetSort().CompareTo(y.GetSort()); });
        return showInfo;
    }
    public void Release()
    {
        
    }
}
