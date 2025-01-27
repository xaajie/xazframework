using System.Collections.Generic;
using Xaz;

public class UserSceneBuildData : UserSceneBuildDataBase
{
    private UserBuildvInfoData buildlvinfo;
    private Dictionary<int, UserCategoryData> cashTar = new Dictionary<int, UserCategoryData>();
    public void RefreshData()
    {
        cashTar.Clear();
        buildlvinfo = ModuleMgr.BuildMgr.GetBuildlvInfo(id, level);
    }

    public UserBuildvInfoData GetInfo()
    {
        return buildlvinfo;
    }

    public string GetIcon()
    {
        return GetInfo().GetIcon();
    }

    public bool IsAdUp()
    {
        return GetInfo().GetLvInfo().costAd;
    }

    public bool IsFullLv()
    {
        return GetInfo().GetNextInfo() == null;
    }
    public bool CheckCanLvup()
    {
        if (IsAdUp())
        {
            return true;
        }
        if (IsFullLv())
        {
            return false;
        }
        UserCategoryData costInfo = GetInfo().GetLvupCostInfo();
        return ModuleMgr.CategoryMgr.CheckOwn(costInfo, costInfo.GetNum());
    }

    public List<int> GetLvupChangeAttrs()
    {
        return GetInfo().GetLvupAttrs();
    }
    public List<UserAttrData> GetNextAttrInfos()
    {
        List<UserAttrData> userAttrs = null;
        List<int> attrTypes = GetLvupChangeAttrs();
        UserBuildvInfoData nextInfo = GetInfo().GetNextInfo();
        userAttrs = new List<UserAttrData>();
        if (nextInfo != null)
        {
            for (int i = 0; i < attrTypes.Count; i++)
            {
                switch (attrTypes[i])
                {
                    case (int)AttrEnum.sellprice:
                        UserAttrData attr = new UserAttrData((int)AttrEnum.sellprice, GetInfo().GetPrice(), nextInfo.GetPrice());
                        userAttrs.Add(attr);
                        break;
                    case (int)AttrEnum.product_stacknum:
                        UserAttrData attr2 = new UserAttrData((int)AttrEnum.product_stacknum, GetInfo().GetProductCapacity(), nextInfo.GetProductCapacity());
                        userAttrs.Add(attr2);
                        break;
                    default:
                        break;
                }

            }
        }
        return userAttrs;
    }

    public int GetBuildID()
    {
        return buildlvinfo.GetID();
    }

    public int GetProductCd()
    {
        if (IsBroken())
        {
            return buildlvinfo.GetLvInfo().productcd + 10000;
        }
        else
        {
            return buildlvinfo.GetLvInfo().productcd;
        }
    }

    public int GetBrokenCd()
    {
        return buildlvinfo.GetLvInfo().fixcd;
    }

    public bool FinishProduct()
    {
        if (this.stproductTime <= 0)
        {
            this.stproductTime = TimeUtil.GetNowTicks();
        }
        return (TimeUtil.GetNowTicks() - this.stproductTime) >= GetProductCd();
    }

    public void CheckSetBroken()
    {
        if (!this.IsneedFix && GetBrokenCd() > 0)
        {
            if (this.lastFixTime <= 0)
            {
                this.lastFixTime = TimeUtil.GetNowTicks();
                this.IsneedFix = false;
            }
            else
            {
                this.IsneedFix = (TimeUtil.GetNowTicks() - this.lastFixTime) >= GetBrokenCd();
            }
            if (this.IsneedFix)
            {
                EventMgr.DispatchEvent(EventEnum.NOTICE_REFRESH);
            }
        }
    }

    public bool IsBroken()
    {
        return this.IsneedFix && IsMatchBuildType(Const.BuildType.Machine);
    }

    //修理时的数据处理
    public void SetFixBuildData()
    {
        this.IsneedFix = false;
        this.lastFixTime = TimeUtil.GetNowTicks();
    }

    public void RefreshStartProducTime(bool canv)
    {
        if (canv)
        {
            this.stproductTime = TimeUtil.GetNowTicks();
        }
        else
        {
            this.stproductTime = 0;
        }
    }

    public float GetShowProductCd()
    {
        if (this.stproductTime <= 0)
        {
            return 0;
        }
        return (float)(TimeUtil.GetNowTicks() - this.stproductTime) / GetProductCd();
    }

    public int GetProductId()
    {
        return buildlvinfo.GetProductId();
    }
    public UserCategoryData GetCategory(int productId)
    {
        if (!cashTar.ContainsKey(productId))
        {
            UserCategoryData product = ModuleMgr.CategoryMgr.CreateFromParm((int)Const.Category.PRODUCT, productId, buildlvinfo.GetRawCapacityById(productId));
            cashTar.Add(productId, product);
        }
        return cashTar[productId];
    }

    public bool IsMatchBuildType(Const.BuildType vt)
    {
        return GetBuildType() == (int)vt;
    }
    public int GetBuildType()
    {
        return buildlvinfo.GetBuildType();
    }

    public bool IsShelf()
    {
        return GetBuildType() == (int)Const.BuildType.Shelf;
    }
    public bool IsGenerProductType()
    {
        return GetBuildType() == (int)Const.BuildType.Tree || GetBuildType() == (int)Const.BuildType.Machine;
    }
}
