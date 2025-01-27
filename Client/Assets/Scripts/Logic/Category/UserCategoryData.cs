using System;
using Table;
using Xaz;

public class UserCategoryData : IPackageData
{
    public Const.Category itemType;
    public int itemId;
    public int itemNum;
    public UserCategoryData(int sitemType, int sitemId, int sitemNum) : base()
    {
        itemType = (Const.Category)Enum.Parse(typeof(Const.Category), sitemType.ToString());
        itemId = sitemId;
        itemNum = sitemNum;
    }

    public int GetID()
    {
        return itemId;
    }
    public int GetSType()
    {
        return 0;
    }
    public int GetItemType()
    {
        return (int)itemType;
    }

    public int GetNum()
    {
        return itemNum;
    }
    public currency GetCurrencyInfo()
    {
        return StaticDataMgr.Instance.currencyInfo[this.itemId];
    }

    public item GetItemInfo()
    {
        return StaticDataMgr.Instance.itemInfo[this.itemId];
    }
    public actor GetActorInfo()
    {
        return StaticDataMgr.Instance.actorInfo[this.itemId];
    }
    public product GetProductInfo()
    {
        return StaticDataMgr.Instance.productInfo[this.itemId];
    }

    public build GetBuildInfo()
    {
        return StaticDataMgr.Instance.buildInfo[this.itemId];
    }
    public string GetName()
    {
        string val = string.Empty;
        switch (itemType)
        {
            case Const.Category.CURRENCY:
                val = this.GetCurrencyInfo().name;
                break;
            case Const.Category.ITEM:
                val = this.GetItemInfo().name;
                break;
            case Const.Category.ACTOR:
                val = this.GetActorInfo().name;
                break;
            case Const.Category.BUILD:
                val = this.GetBuildInfo().name;
                break;
            case Const.Category.PRODUCT:
                val = this.GetProductInfo().name;
                break;
            default:
                break;
        }
        return val;
    }

    public string GetCurrecyRich()
    {
        string val = string.Empty;
        if (itemType == Const.Category.CURRENCY)
        {
            switch (itemId)
            {
                case (int)Const.CurrencyType.GOLD:
                    val = "[gold]";
                    break;
                default:
                    break;
            }
        }
        return val;
    }

    public string getColorName()
    {
        return "[" + Const.COLORS_STR[GetQuality() - 1] + "]" + GetName() + "[-]";
    }
    public string GetDesc()
    {
        string val = string.Empty;
        switch (itemType)
        {
            case Const.Category.CURRENCY:
                val = this.GetCurrencyInfo().desc;
                break;
            case Const.Category.ITEM:
                val = this.GetItemInfo().desc;
                break;
            case Const.Category.BUILD:
                val = this.GetBuildInfo().desc;
                break;
            case Const.Category.ACTOR:
                val = this.GetActorInfo().desc;
                break;
            default:
                break;
        }
        return val;
    }
    public string GetIcon()
    {
        string val = string.Empty;
        switch (itemType)
        {
            case Const.Category.CURRENCY:
                val = this.GetCurrencyInfo().icon;
                break;
            case Const.Category.ITEM:
                val = this.GetItemInfo().icon;
                break;
            case Const.Category.ACTOR:
                val = this.GetActorInfo().icon;
                break;
            case Const.Category.PRODUCT:
                val = this.GetProductInfo().icon;
                break;
            default:
                break;
        }
        return val;
    }

    public string GetAtlas()
    {
        string val = string.Empty;
        switch (itemType)
        {
            case Const.Category.CURRENCY:
                val = this.GetCurrencyInfo().atlas;
                break;
            case Const.Category.ITEM:
                val = this.GetItemInfo().atlas;
                break;
            case Const.Category.ACTOR:
                val = this.GetActorInfo().atlas;
                break;
            case Const.Category.PRODUCT:
                val = this.GetProductInfo().atlas;
                break;
            default:
                break;
        }
        return val;
    }
    public int GetQuality()
    {
        int val = 1;
        switch (itemType)
        {
            case Const.Category.CURRENCY:
                val = this.GetCurrencyInfo().quality;
                break;
            case Const.Category.ITEM:
                val = this.GetItemInfo().quality;
                break;
            default:
                break;
        }
        return val;
    }

    public bool IsNew()
    {
        return false;
    }

    public int GetOwnNum()
    {
        int val = 1;
        switch (itemType)
        {
            case Const.Category.CURRENCY:
                val = Profile.Instance.GetCurrencyNum(itemId) ;
                break;
            default:
                val = ModuleMgr.BagMgr.GetBagIdNum(this);
                break;
        }
        return val;
    }
}
