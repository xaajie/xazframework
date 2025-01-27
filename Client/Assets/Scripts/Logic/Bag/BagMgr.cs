//----------------------------------------------------------------------------
//-- 宝箱模块管理
//-- @author xiejie
//----------------------------------------------------------------------------
using System.Collections.Generic;
using Table;
using UnityEngine;
using Xaz;

public class BagMgr
{
    List<UserBagData> bagList;
    public BagMgr()
    {
        
    }

    public List<UserBagData> GetBagList()
    {
        return bagList;
    }

    public int GetBagIdNum(UserCategoryData info)
    {
        for (int i = 0; i < bagList.Count; i++)
        {
            if((int)info.itemType == bagList[i].itemType && info.itemId == bagList[i].itemId)
            {
                return bagList[i].num;
            }
        }
        return 0;
    }
    public void SetBagList(List<UserBagData> res)
    {
         bagList = res;
    }
    public void SetBagInfo(List<UserBagDataBase> datalist)
    {
        bagList = new List<UserBagData>();
        if (datalist != null)
        {
            for (int i = 0; i < datalist.Count; i++)
            {
                UserBagData pt = new UserBagData();
                pt.SetData(datalist[i]);
                bagList.Add(pt);
            }
        }
    }

    public List<UserBagData> GetBagListByType(Const.Category info)
    {
        List < UserBagData > res = new List < UserBagData >();
        for (int i = 0; i < bagList.Count; i++)
        {
            if (bagList[i].itemType == (int)info)
            {
                res.Add(bagList[i]);
            }
        }
        return res;
    }


    public bool CheckHasGetAwardBox()
    {
        return false;
    }
    public void Release()
    {
        
    }
}
