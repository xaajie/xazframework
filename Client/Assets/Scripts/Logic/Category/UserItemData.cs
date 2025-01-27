//----------------------------------------------------------------------------
//-- 技能数据
//-- @author xiejie
//----------------------------------------------------------------------------
using System.Collections.Generic;
using Table;

public class UserItemData : Data,IPackageData

{
    private int id;

    public UserItemData(int sid) : base()
    {
        this.id = sid;
    }

    public int GetID()
    {
        return id;
    }

    public item GetInfo()
    {
        return StaticDataMgr.Instance.itemInfo[id];
    }

    public string GetName()
    {
        return GetInfo().name;
    }

    public int GetNum()
    {
        return 1;
    }
    public int GetItemType()
    {
        return (int)Const.Category.ITEM;
    }
    public int GetSType()
    {
        return 1;
    }
    public int GetQuality()
    {
        return GetInfo().quality;
    }

    public string GetIcon()
    {
        return GetInfo().icon;
    }

    public string GetAtlas()
    {
        return GetInfo().atlas;
    }


    public int GetOwnNum()
    {
        return 1;
    }
    public string GetDesc()
    {
        return string.Empty;
    }

    //public bool IsOpen()
    //{
    //    return true;
    //}

}
