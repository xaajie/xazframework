//----------------------------------------------------------------------------
//--UserHappeningData数据封装
//-- @author xiejie
//----------------------------------------------------------------------------

using Table;
using Xaz;

public class UserHappeningData : Data
{

    private UserHappeningDataBase rawInfo;
    public UserHappeningData(UserHappeningDataBase rawsInfo) : base()
    {
        rawInfo = rawsInfo;
    }

    public void SetSeeAdTime()
    {
        rawInfo.adSeeTime = TimeUtil.GetNowInt();
    }
    public string GetName()
    {
        return GetInfo().name;
    }
    public bool IsNull()
    {
        return !CheckTimeValid();
    }

    public bool IsAfterSeeAd()
    {
        if(rawInfo.adSeeTime> GetCurCreateTime())
        {
            return true;
        }
        return false;
    }

    public  int GetCurCreateTime()
    {
        return TimeUtil.GetNowInt() - (TimeUtil.GetNowInt() - rawInfo.createTime) % GetInfo().refreshcd;
    }
    //public int GetNextRefreshTime()
    //{
    //    return createTime + GetInfo().refreshcd;
    //}

    public int GetID()
    {
        return GetInfo().id;
    }

    public int GetBuffLeftTime()
    {
        if (IsEffectNow())
        {
            UserBuffData vuff = ModuleMgr.BuffMgr.CheckGetBuff(GetBuffID());
            return vuff.GetLeftTime();
        }
        return 0;
    }
    public buff GetBuffInfo()
    {
        return StaticDataMgr.Instance.buffinfo[GetBuffID()];
    }

    public int GetBuffDuration()
    {
        return GetBuffInfo().effectTime;
    }
    public int GetLeftTime()
    {
        return (GetCurCreateTime() + GetInfo().duration) - TimeUtil.GetNowInt();
    }
    public bool CheckTimeValid()
    {
        if (IsEffectNow())
        {
            return true;
        }
       return !IsAfterSeeAd() && TimeUtil.GetNowInt() < (GetCurCreateTime() + GetInfo().duration);
    }
    public bool IsEffectNow()
    {
        if(GetHappenType() == (int)Const.HappenType.GetBuff)
        {
            return ModuleMgr.BuffMgr.CheckGetBuff(GetBuffID())!=null;
        }
        return false;
    }
    public happening GetInfo()
    {
        return StaticDataMgr.Instance.happeningInfo[rawInfo.infoid];
    }

    public int GetBuffID()
    {
        return int.Parse(GetInfo().param[0]);
    }
    public string GetIcon()
    {
        return GetInfo().icon;
    }

    public string GetDesc()
    {
        if (GetHappenType() == (int)Const.HappenType.GetBuff)
        {
            return GetInfo().desc+"\n"+string.Format(Utils.GetLang("happen_durtxt"), GetBuffDuration());
        }
        else
        {
            return GetInfo().desc;
        }
    }

    public int GetHappenType()
    {
        return GetInfo().hType;
    }
    public string GetAtlas()
    {
        return GetInfo().atlas;
    }


}
