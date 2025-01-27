//----------------------------------------------------------------------------
//-- UserBookCellData数据封装
//-- @author xiejie
//----------------------------------------------------------------------------
using UnityEditor;

public class UserBookCellData : Data
{
    private UserCategoryData info;
    private string cellname;
    public bool needTween = true;
    public UserBookCellData(int itemType, int itemId) : base()
    {
        info = new UserCategoryData(itemType, itemId, 1);
        cellname = itemType == (int)Const.Category.ACTOR ? UIBook.CELLNAME1 : UIBook.CELLNAME2;
    }

    public string GetCellName()
    {
        return cellname;
    }

    public UserCategoryData GetBox()
    {
        return info;
    }

    public bool CanAwa()
    {
        return !IsLock() && !IsFinAwa();
    }
    public bool IsLock()
    {
        UserBookDataBase dat = ModuleMgr.BookMgr.GetBookDat(this);
        return dat == null;
    }

    public bool IsFinAwa()
    {
        UserBookDataBase dat= ModuleMgr.BookMgr.GetBookDat(this);
        return dat!=null && dat.finAward;
    }
}
