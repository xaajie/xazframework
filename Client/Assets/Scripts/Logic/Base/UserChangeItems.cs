//----------------------------------------------------------------------------
//-- UserChangeItems
//-- @author xiejie
//----------------------------------------------------------------------------

using System.Collections.Generic;

public class UserChangeItems : Data
{
    public List<UserChangeItem> addItems;
    public List<UserChangeItem> delItems;
    public UserChangeItems():base()
    {
        delItems = new List<UserChangeItem>() { };
        addItems = new List<UserChangeItem>();
    }
}

public class UserChangeItem : Data
{
    public int itemType;
    public int itemId;
    public int num;
    public UserChangeItem(int iType,int iId,int count) : base()
    {
        this.itemType = iType;
        this.itemId = iId;
        this.num = count;
    }
}
