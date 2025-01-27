//----------------------------------------------------------------------------
//-- 技能数据
//-- @author xiejie
//----------------------------------------------------------------------------
using Table;

public class UserAttrData : Data

{
    private int id;
    private int curnum;
    private int tonum;

    public UserAttrData(int sid,int curnumt,int tonumt) : base()
    {
        this.id = sid;
        curnum = curnumt;
        tonum = tonumt;
    }

    public int GetAttrID()
    {
        return id;
    }

    public int GetChangeNum()
    {
        return tonum - curnum;
    }

    public attr GetInfo()
    {
        return StaticDataMgr.Instance.attrInfo[id];
    }

    public string GetName()
    {
        return GetInfo().name;
    }

    public string GetIcon()
    {
        return GetInfo().icon;
    }


    public string GetAtlas()
    {
        return GetInfo().atlas;
    }

    public int GetNum()
    {
        return curnum;
    }

}
