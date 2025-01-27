using System.Collections.Generic;
using Table;

public class HappeningMgr
{
    private List<UserHappeningData> userHappenDatas = new List<UserHappeningData>();
    public int maxNum = 2;
    public HappeningMgr()
    {
    }

    public void GenerHappens(List<UserHappeningDataBase> lists)
    {
        userHappenDatas.Clear();
        for (int i = 0; i < lists.Count; i++)
        {
            AddHappenInfo(lists[i]);
        }
    }
    public void AddHappenInfo(UserHappeningDataBase info)
    {
        UserHappeningData vt = new UserHappeningData(info);
        userHappenDatas.Add(vt);
    }
    public UserHappeningData GetShowHappens(int i)
    {
        if (i < userHappenDatas.Count)
        {
            return userHappenDatas[i];
        }
        else
        {
            return null;
        }
    }

    private static int SortNotice(UserHappeningData x, UserHappeningData y)
    {
        bool xImportant = x.IsNull();
        bool yImportant = y.IsNull();

        if (xImportant && !yImportant)
        {
            return 1;
        }
        else if (!xImportant && yImportant)
        {
            return -1;
        }
        return x.GetID().CompareTo(y.GetID());
    }

    public bool CheckHappen()
    {
        bool isChange = false;
        for (int i = 0; i < userHappenDatas.Count; i++)
        {
            userHappenDatas[i].CheckTimeValid();
        }

        //for (int m = 0; m < userHappenDatas.Count; m++)
        //{
        //    UserHappeningData selectNoticeData = userHappenDatas[m];
        //    if (selectNoticeData.GethType() == Const.HappenType.None)
        //    {
        //        NoticeSortData nvdata = uids[0];
        //        selectNoticeData.UpdateNoticeData(nvdata.nType, nvdata.uid);
        //        uids.RemoveAt(0);
        //        isChange = true;
        //        if (uids.Count <= 0)
        //        {
        //            break;
        //        }
        //    }
        //}
        userHappenDatas.Sort(SortNotice);
        return isChange;
    }


    public void Release()
    {

    }
}
