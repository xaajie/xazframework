//----------------------------------------------------------------------------
//-- 
//-- @author xiejie
//----------------------------------------------------------------------------
using System;

[Serializable]
public class UserSceneBuildDataBase : Data
{
    public int id;
    public int level;
    public int uid;
    public long lastFixTime;
    public long stproductTime;
    public bool IsneedFix;
    public int bindposId;
    public int ownId;
}