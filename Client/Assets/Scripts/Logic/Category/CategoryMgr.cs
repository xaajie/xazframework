//----------------------------------------------------------------------------
//-- CategoryMgr模块管理
//-- @author xiejie
//----------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xaz;
using static UIAdGold;

public class CategoryMgr
{
    private Dictionary<int, UserItemData> signDataList = new Dictionary<int, UserItemData>();

    //private Dictionary<int, UserTurnItemData> turnItemDataList = new Dictionary<int, UserTurnItemData>();
    public CategoryMgr()
    {

    }

    public List<UserCategoryData> CreateFromList(List<string> arr)
    {
        List<UserCategoryData> list = new List<UserCategoryData>();
        if (arr != null)
        {
            for (int i = 0; i < arr.Count; i++)
            {
                list.Add(CreateFrom(arr[i]));
            }
        }
        return list;
    }

    public UserCategoryData CreateFrom(string val)
    {
        string[] dat = val.Split('_');
        if (dat.Length >= 3)
        {
            return CreateFromParm(int.Parse(dat[0]), int.Parse(dat[1]), int.Parse(dat[2]));
        }
        else
        {
            return CreateFromParm(int.Parse(dat[0]), int.Parse(dat[1]), 1);
        }
    }

    public UserCategoryData CreateCurrency(Const.CurrencyType id, int num)
    {
        return new UserCategoryData((int)Const.Category.CURRENCY, (int)id, num);
    }

    public UserCategoryData CreateFromParm(int itemType, int itemId, int itemNum)
    {
        UserCategoryData data = new UserCategoryData(itemType, itemId, itemNum);
        return data;
    }

    public UserItemData GetItemData(int signId)
    {
        if (!signDataList.ContainsKey(signId))
        {
            UserItemData signt = new UserItemData(signId);
            signDataList[signId] = signt;
        }
        return signDataList[signId];
    }

    public void SetTip(IPackageData data, GameObject obj)
    {
        XazEventListener.VoidDelegate click = delegate (GameObject obj)
        {
            //ModuleMgr.TipMgr.ShowTip(data.GetItemType(), data.GetID(), data, null, obj.transform);
        };
        XazEventListener.Get(obj).onClick = click;
    }

    //    -- 检验某类道具数目是否充足
    //-- @parm needNum:所需数目
    //-- @parm needTip：对应Const.CheckTipType 传空不提示
    //-- return 布尔值 true充足 false不充足
    public bool CheckOwn(IPackageData data, int needNum, Const.CheckTipType tipType = Const.CheckTipType.No,bool checkgoldsupply=false)
    {
        int num = data.GetOwnNum();
        if (num < needNum)
        {
            if (checkgoldsupply)
            {
                if (data.GetItemType() == (int)Const.Category.CURRENCY && data.GetID() == (int)Const.CurrencyType.GOLD)
                {
                    int lackgold = data.GetNum() - Profile.Instance.user.gold;
                    UserChallengeShowData curChall = ModuleMgr.ChallengeMgr.GetCurChallege();
                    int canaddgold = curChall.GetGoldSupplyNum(lackgold);
                    if (canaddgold >= lackgold)
                    {
                        UIMgr.Open<UIAdGold>(uiview => uiview.SetData(lackgold));
                        return false;
                    }
                }
            }
            if (tipType != Const.CheckTipType.No)
            {
                string str = string.Format(Utils.GetLang("currencylack"), data.GetName());
                UIMgr.ShowFlyTip(str);
            }
        }
        return num >= needNum;
    }
    public void Release()
    {

    }
}
