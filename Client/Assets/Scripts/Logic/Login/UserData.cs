//----------------------------------------------------------------------------
//-- UserData数据封装
//-- @author xiejie
//----------------------------------------------------------------------------
using Table;

public class UserData: UserDataBase

{
    public UserData():base()
    {
            
    }
    public int GetGold()
    {
        return this.gold;
    }
    public string GetName()
    {
        return this.name;
    }

    public void Addlvexp(int addnum)
    {
        level v = StaticDataMgr.Instance.levelInfo[GetLevel()];
        lvexp = lvexp + addnum;
        while (v!=null && v.exp > 0 && v.exp <= lvexp)
        {
            level++;
            lvexp = lvexp - v.exp;
            v = StaticDataMgr.Instance.levelInfo[level];
        }
    }
    public int GetLevel()
    {
        return this.level>0?level:0;
    }

    public string GetlvexpStr()
    {
        if (GetLevelInfo().exp > 0)
        {
            return string.Format("{0}/{1}", this.lvexp, GetLevelInfo().exp);
        }
        else
        {
            return string.Empty;
        }
    }

    public float GetlvexpVal()
    {
        if (GetLevelInfo().exp > 0)
        {
            return (float)lvexp / (float)GetLvupNeedExp();
        }
        else
        {
            return 1;
        }
    }
    public level GetLevelInfo()
    {
        return StaticDataMgr.Instance.levelInfo[GetLevel()];
    }

    public int GetLvupNeedExp()
    {
        return GetLevelInfo().exp;
    }

    public bool CanShake()
    {
        return !closeshake;
    }


}
