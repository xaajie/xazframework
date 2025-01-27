//----------------------------------------------------------------------------
//-- UserBuildInfoData数据封装
//-- @author xiejie
//----------------------------------------------------------------------------
using System.Collections.Generic;
using Table;

public class UserBuildvInfoData : Data
{
    private int id;
    private int level;
    private buildlv info;
    public Dictionary<int, int[]> rawProductInfo = new Dictionary<int, int[]>() { };
    private UserCategoryData costInfo;
    private UserBuildvInfoData buildnextInfo;
    public UserBuildvInfoData(int itemId,int lv, buildlv cha) : base()
    {
        id = itemId;
        level = lv;
        info = cha;
        if (GetLvInfo() != null)
        {
            for (int i = 0; i < info.rawitem.Count; i++)
            {
                string[] arr = info.rawitem[i].Split('_');
                rawProductInfo.Add(int.Parse(arr[0]), new int[] { int.Parse(arr[1]), GetLvInfo().rawlimitnum[i] });
            }
        }
    }

    public buildlv GetLvInfo()
    {
        return info;
    }

    public int GetBarVal(out int max)
    {
        int premaxlv = ModuleMgr.BuildMgr.GetStepmaxLv(GetID(),GetStep()-1);
        int maxlv = ModuleMgr.BuildMgr.GetStepmaxLv(GetID(), GetStep());
        int cur = GetLevel() - premaxlv;
        max = maxlv - premaxlv;
        return cur;
    }

    //当前是换新等级
    public bool IsChangeLv()
    {
        return GetNextInfo() != null  && GetNextInfo().GetStep()>GetStep();
    }

    public UserBuildvInfoData GetNextInfo()
    {
        return ModuleMgr.BuildMgr.GetBuildlvInfo(id, level + 1) ;
    }

    public int GetStep()
    {
        return GetLvInfo().step;
    }

    public string GetIcon()
    {
        return GetLvInfo().icon;
    }
    public string GetAtlas()
    {
        return GetLvInfo().atlas;
    }
    public string GetName()
    {
        return GetLvInfo().name;
    }

    public build GetBuildInfo()
    {
        return StaticDataMgr.Instance.buildInfo[id];
    }
    public int GetLevel()
    {
        return level;
    }

    public int GetPrice()
    {
        return GetLvInfo().productSellPrice;
    }
    public bool IsCanShowLvup()
    {
        return GetBuildInfo().showlvup;
    }

    public int GetSort()
    {
        return GetBuildInfo().sort;
    }

    public int GetBuildType()
    {
        return GetBuildInfo().buildType;
    }
    public int GetProductId()
    {
        return GetLvInfo().productItem;
    }
    public int GetModelId()
    {
        return GetBuildInfo().modelId;
    }
   
    public int GetID()
    {
        return id;
    }
    public int GetProductCapacity()
    {
        return GetLvInfo().limitnum;
    }

    public int GetRawCapacityById(int productId)
    {
        if (rawProductInfo.ContainsKey(productId))
        {
            return rawProductInfo[productId][1];
        }
        else
        {
            return 0;
        }
    }
    public int GetRawNeedById(int productId)
    {
        return rawProductInfo[productId][0];
    }

    public List<int> GetLvupAttrs()
    {
        return GetLvInfo().lvupAttr;
    }

    public UserCategoryData GetLvupCostInfo()
    {
        if (costInfo == null)
        {
            costInfo = ModuleMgr.CategoryMgr.CreateFrom(GetLvInfo().cost[0]);
        }
        return costInfo;
    }
    public List<UserCategoryData> GetLvupAwardInfo()
    {
        return ModuleMgr.CategoryMgr.CreateFromList(GetLvInfo().award);
    }

    public string GetModelImg()
    {
        return GetLvInfo().modelImg;
    }
}
