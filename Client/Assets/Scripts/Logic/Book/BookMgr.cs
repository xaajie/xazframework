//----------------------------------------------------------------------------
//-- BookMgr模块管理
//-- @author xiejie
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using Xaz;

public class BookMgr
{
    public Dictionary<string, UserBookDataBase> books= new Dictionary<string, UserBookDataBase>();

    public BookMgr()
    {
        
    }

    public string GetKey(int itemType, int id)
    {
        return string.Format("{0}_{1}", itemType, id);
    }

    public void SetDataList(List<UserBookDataBase> info)
    {
        for (int i = 0; i < info.Count; i++)
        {
            string key = GetKey(info[i].itemType, info[i].itemId);
            books.Add(key, info[i]);
        }
    }
    public void CheckBookChange(int itemType, int id)
    {
        string key = GetKey(itemType, id);
        if (!books.ContainsKey(key))
        {
            UserBookDataBase info = new UserBookDataBase();
            info.itemType = itemType;
            info.itemId = id;
            info.finAward = false;
            books.Add(key, info);
        }
    }

    public UserBookDataBase GetBookDat(UserBookCellData info)
    {
        string key = GetKey((int)info.GetBox().itemType, info.GetBox().GetID());
        if (books.ContainsKey(key))
        {
            return books[key];
        }
        return null;
    }
    public void SaveFinBookAwa(UserBookCellData info, Transform target)
    {
        UserBookDataBase dat = GetBookDat(info);
        if (dat != null) { dat.finAward = true; }
        //UserCategoryData awa = ModuleMgr.CategoryMgr.CreateFrom(Constant.BOOK_AWA);
        //List<UserCategoryData> awalist = new List<UserCategoryData>() { awa };
        //ModuleMgr.AwardMgr.AwardList(awalist, true);
        //UIMgr.Open<UIFlyAward>(uiview => uiview.SetFlyInfo(awalist,target));
    }
    public void Release()
    {
        books.Clear();
    }

}
