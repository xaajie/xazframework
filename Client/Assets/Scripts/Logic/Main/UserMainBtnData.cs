//----------------------------------------------------------------------------
//-- 主界面按钮入口封装
//-- @author xiejie
//----------------------------------------------------------------------------
using System;
using Table;
public class UserMainBtnData : Data

{
    public int id;

    public UserMainBtnData() : base()
    {

    }

    public int GetSort()
    {
        return GetInfo().sortIndex;
    }
    public Const.MainViewID GetIdType()
    {
        return (Const.MainViewID)Enum.Parse(typeof(Const.MainViewID), id.ToString());
    }
    public mainshow GetInfo()
    {
        return StaticDataMgr.Instance.mainshowInfo[id];
    }

    public string GetName()
    {
        return Utils.GetLang(GetInfo().name);
    }

    public bool CanShow()
    {
        return !GetInfo().debugClose;
    }

    public string GetAtlas()
    {
        return GetInfo().atlas;
    }
    public string GetIcon()
    {
        return GetInfo().icon;
    }

    public void OpenView()
    {
        if (id == (int)Const.MainViewID.map)
        {
            UIMgr.Open<UIChallenge>();
        }
        else if (id == (int)Const.MainViewID.buildup)
        {
            UIMgr.Open<UIBuilding>();
        }
        else if (id == (int)Const.MainViewID.attrup)
        {
            UIMgr.Open<UIAttr>();
        }
        else if (id == (int)Const.MainViewID.attrup)
        {
            UIMgr.Open<UIAttr>();
        }
        else if (id == (int)Const.MainViewID.shop)
        {
            UIMgr.Open<UIShop>();
        }
        else
        {
            UIMgr.ShowFlyTipKey("funcnotopen");
        }
        UIMgr.Get<UIMainBottom>().Refresh();
    }

}
