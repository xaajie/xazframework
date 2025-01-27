//----------------------------------------------------------------------------
//-- UserBuildInfoData数据封装
//-- @author xiejie
//----------------------------------------------------------------------------
using System;
using Table;

public class UserAttrupShowData : Data
{
    private int attrGroupId;
    private UserCategoryData costInfo;
    private int changeId;
    private UserAttrupShowData nextInfo;
    private int curLevel=-1;
    public UserAttrupShowData(int tattrGroupId) : base()
    {
        attrGroupId = tattrGroupId;
    }

    public void SetLevel(int level)
    {
        if (curLevel != level)
        {
            curLevel = level;
            ChangeInfo();
        }
    }
    public void RefreshLevelData()
    {
        curLevel = Profile.Instance.GetAttrupLevel(GetAttrGroupId());
        ChangeInfo();
    }
    private void ChangeInfo()
    {
        changeId = curLevel==0?StaticDataMgr.Instance.GetAttrTableId(attrGroupId, 1) : StaticDataMgr.Instance.GetAttrTableId(attrGroupId, curLevel);
        nextInfo = null;
        int nexId = StaticDataMgr.Instance.GetAttrTableId(attrGroupId, curLevel + 1);
        if (nexId > 0)
        {
            nextInfo = new UserAttrupShowData(attrGroupId);
            nextInfo.SetLevel(curLevel + 1);
        }
    }

    public int GetAttrGroupId()
    {
        return attrGroupId;
    }
    public attrup GetInfo()
    {
        StaticDataMgr.Instance.attrupInfo.TryGetValue(changeId, out var info);
        return info;
    }

    public int GetBuffID()
    {
        return GetInfo().buffId;
    }
    public buff GetBuffInfo()
    {
        return StaticDataMgr.Instance.buffinfo[GetBuffID()];
    }

    public attr GetAttrInfo()
    {
        return StaticDataMgr.Instance.attrInfo[GetBuffInfo().attrId];
    }

    public int GetAttrId()
    {
        return GetBuffInfo().attrId;
    }
    public int GetSort() {
        return GetInfo().sort;
    }

    public int GetLevel()
    {
        return curLevel;
    }

    public bool IsFullLv()
    {
        return GetNextInfo() == null;
    }

    public string GetIcon()
    {
        if (string.IsNullOrEmpty(GetInfo().icon))
        {
            return GetAttrInfo().icon;
        }
        else
        {
            return GetInfo().icon;
        }
    }
    public string GetAtlas()
    {
        if (string.IsNullOrEmpty(GetInfo().atlas))
        {
            return GetAttrInfo().atlas;
        }
        else
        {
            return GetInfo().atlas;
        }
    }

    public string GetShowAttrval(int val)
    {
        return ModuleMgr.AttrMgr.GetShowAttrval(val,(int)GetAttrInfo().countType);
    }
    public string GetAttrNumStr()
    {
        int val = 0;
        if (GetLevel() > 0)
        {
            val = GetBuffInfo().attrval;
        }
        return GetShowAttrval(val);
    }

    public float GetAttrVal()
    {
        if (GetLevel() > 0)
        {
            return GetBuffInfo().attrval;
        }
        else
        {
            return 0;
        }
    }
    public string GetDesc()
    {
        return GetInfo().desc;
    }

    public UserAttrupShowData GetNextInfo()
    {
        return nextInfo;
    }

    public bool IsAdUp()
    {
        return GetInfo().costAd;
    }
    public bool IsFree()
    {
        return string.IsNullOrEmpty(GetInfo().cost);
    }
    public UserCategoryData GetLvupCostInfo()
    {
        if (costInfo == null&&!IsFree())
        {
            costInfo = ModuleMgr.CategoryMgr.CreateFrom(GetInfo().cost);
        }
        return costInfo;
    }
    public bool CheckCanLvup()
    {
        if (IsAdUp())
        {
            return true;
        }
        if (GetNextInfo() == null)
        {
            return false;
        }
        UserCategoryData costInfo = GetLvupCostInfo();
        return ModuleMgr.CategoryMgr.CheckOwn(costInfo, costInfo.GetNum());
    }

    public  int GetActionTarget ()
    {
        return GetBuffInfo().buffparm;
    }

    public bool IsActor()
    {
        return GetActionTarget() > 100 && GetActionTarget() < 300;
    }
}
 