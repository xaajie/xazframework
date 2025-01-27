using System.Collections.Generic;
using Table;

public class NoticeSortData
{
    public Const.NoticeType nType;
    public int uid;
    public NoticeSortData(Const.NoticeType vt, int suid)
    {
        nType = vt;
        uid = suid;
    }
}
public class FightMgr
{
    private Dictionary<int, List<int>> orderPoolList = new Dictionary<int, List<int>>();
    private Dictionary<int, List<int>> unlockConfig = new Dictionary<int, List<int>>();
    private static int keyRatio = 100;
    private List<UserNoticeData> userNoticeDatas = new List<UserNoticeData>();
    public int maxNoticeNum = 3;
    public FightMgr()
    {
        for (int i = 0; i < maxNoticeNum; i++)
        {
            userNoticeDatas.Add(new UserNoticeData(Const.NoticeType.None, 0));
        }
    }

    public UserNoticeData GetShowNotice(int i)
    {
        return userNoticeDatas[i];
    }

    private static int SortNotice (UserNoticeData x, UserNoticeData y)
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
        return x.GetCreateTime().CompareTo(y.GetCreateTime());
    }

    private bool CheckMacthNotice(Const.NoticeType nType, int suid)
    {
        for (int i = 0; i < userNoticeDatas.Count; i++)
        {
            UserNoticeData selectNoticeData = userNoticeDatas[i];
            if (selectNoticeData.uid == suid && selectNoticeData.GetNoticeType()== nType)
            {
                return true;
            }
        }
        return false;
    }
    public bool CheckNotice()
    {
        bool isChange=false;
        for (int i = 0; i < userNoticeDatas.Count; i++)
        {
            userNoticeDatas[i].CheckTimeValid();
        }
        List<NoticeSortData> uids = new List<NoticeSortData>();
        for (int i = 0; i < RushManager.Instance.workers.Count; i++)
        {
            WorkerCtrl actort = RushManager.Instance.workers[i];
            if (actort.GetActorType()!=Const.ActorType.Guard && actort.step == Const.ActorStep.Sleep && !CheckMacthNotice(Const.NoticeType.Sleep, actort.Getuid()))
            {
                uids.Add(new NoticeSortData(Const.NoticeType.Sleep, actort.Getuid()));
            }
        }
        for (int i = 0; i < RushManager.Instance.builds.Count; i++)
        {
            BuildController build = RushManager.Instance.builds[i];
            if (build.GetCtrlData().IsBroken() && !CheckMacthNotice(Const.NoticeType.Broken, build.Getuid()))
            {
                uids.Add(new NoticeSortData(Const.NoticeType.Broken, build.Getuid()));
            }
        }
        if (uids.Count > 0)
        {
            ListExtensions.Shuffle(uids);
            for (int m = 0; m < userNoticeDatas.Count; m++)
            {
                UserNoticeData selectNoticeData = userNoticeDatas[m];
                if (selectNoticeData.GetNoticeType() == Const.NoticeType.None)
                {
                    NoticeSortData nvdata = uids[0];
                    selectNoticeData.UpdateNoticeData(nvdata.nType, nvdata.uid);
                    uids.RemoveAt(0);
                    isChange = true;
                    if (uids.Count <= 0)
                    {
                        break;
                    }
                }
            }
        }
        userNoticeDatas.Sort(SortNotice);
        return isChange;
    }
    public bool FixBuild(int buildId,bool frommaplyer)
    {
        BuildController tar = GetBuildByUid(buildId);
        if (tar != null)
        {
            MachineBuild bu = (tar as MachineBuild);
            if (bu.GetCtrlData().IsBroken())
            {
                bu.FixBuildAction(frommaplyer);
                return true;
            }
        }
        return false;
    }

    public void LevelUpActor(int actionTarget)
    {
        if (actionTarget == (int)Const.ActorType.Player)
        {
            RushManager.Instance.mainplayer.UpdateAttr();
            return;
        }
        for (int i = 0; i < RushManager.Instance.workers.Count; i++)
        {
            WorkerCtrl tar = RushManager.Instance.workers[i];
            if ((int)tar.GetActorType() == actionTarget)
            {
                tar.UpdateAttr();
            }
        }
    }

    public bool WakeupActor(int aId)
    {
        WorkerCtrl tar = GetActorByUid(aId);
        if (tar != null)
        {
            WorkerCtrl bu = (tar as WorkerCtrl);
            if (bu.GetCtrlData().IsSleep())
            {
                bu.WakeupAction();
                return true;
            }
        }
        return false;
    }
    public WorkerCtrl GetActorByUid(int bid)
    {
        for (int i = 0; i < RushManager.Instance.workers.Count; i++)
        {
            WorkerCtrl build = RushManager.Instance.workers[i];
            if (build.Getuid() == bid)
            {
                return build;
            }
        }
        return null;
    }

    public WorkerCtrl GetActorByType(Const.ActorType bid)
    {
        for (int i = 0; i < RushManager.Instance.workers.Count; i++)
        {
            WorkerCtrl worker = RushManager.Instance.workers[i];
            if (worker.GetActorType() == bid)
            {
                return worker;
            }
        }
        return null;
    }
    public BuildController GetBuildByStaticId(int bid)
    {
        for (int i = 0; i < RushManager.Instance.builds.Count; i++)
        {
            BuildController build = RushManager.Instance.builds[i];
            if (build.GetCtrlData().GetBuildID() == bid)
            {
                return build;
            }
        }
        return null;
    }

    public BuildController GetBuildByUid(int bid)
    {
        for (int i = 0; i < RushManager.Instance.builds.Count; i++)
        {
            BuildController build = RushManager.Instance.builds[i];
            if (build.Getuid() == bid)
            {
                return build;
            }
        }
        return null;
    }
    //jietodo 优化缓存不？

    public BuildController GetOrderGetProductBuild(int productId)
    {
        for (int i = 0; i < RushManager.Instance.builds.Count; i++)
        {
            BuildController build = RushManager.Instance.builds[i];
            if (build.GetCtrlData().IsShelf()&&build.GetCtrlData().GetInfo().GetProductId() == productId)
            {
                return build;
            }
        }
        return null;
    }

    public int GetPricebyProductId(int productId)
    {
        for (int i = 0; i < RushManager.Instance.builds.Count; i++)
        {
            BuildController build = RushManager.Instance.builds[i];
            if (build.GetCtrlData().IsGenerProductType() && build.GetCtrlData().GetInfo().GetProductId() == productId)
            {
                return build.GetCtrlData().GetInfo().GetPrice();
            }
        }
        return 0;
    }

    public BuildController GetBuildByBuildType(Const.BuildType buildType, int productId = -1)
    {
        for (int i = 0; i < RushManager.Instance.builds.Count; i++)
        {
            BuildController build = RushManager.Instance.builds[i];
            if (productId < 0)
            {
                if (build.GetCtrlData().GetInfo().GetBuildType() == (int)buildType)
                {
                    return build;
                }
            }
            else
            {
                if (build.GetCtrlData().GetInfo().GetBuildType() == (int)buildType && productId == build.GetCtrlData().GetInfo().GetProductId())
                {
                    return build;
                }
            }
        }
        return null;
    }

    public int GetKey(int id, int lv)
    {
        return id * keyRatio + lv;
    }

    public List<int> GetUnlockIdList(int unlockGroupId, int sortId)
    {
        int key = GetKey(unlockGroupId, sortId);
        if (!unlockConfig.ContainsKey(key))
        {
            List<int> datList = new List<int>() { };
            foreach (sceneunlock cha in StaticDataMgr.Instance.sceneunlockInfo.Values)
            {
                if (cha.unlockId == unlockGroupId && (cha.sort == sortId || sortId==-1))
                {
                    datList.Add(cha.id);
                }
            }
            unlockConfig[key] = datList;
        }
        return unlockConfig[key];
    }


    public int GetNewOrderId(int poolId)
    {
        List<int> re = GetOrderList(poolId);
        List<int> filtere = new List<int>();
        List<int> ratio = new List<int>() { };
        bool canadd = false;
        foreach (int id in re)
        {
            order vg = StaticDataMgr.Instance.orderInfo[id];
            canadd = false;
            for (int i = 0; i < vg.needIem.Count; i++)
            {
                string[] arr = vg.needIem[i].Split('_');
                //过滤无产物的情况
                if (GetOrderGetProductBuild(int.Parse(arr[0])) != null)
                {
                    canadd = true;
                    break;
                }
            }
            if (canadd)
            {
                filtere.Add(id);
                ratio.Add(vg.ratio * Const.Weight_Ex);
            }
        }
        if (filtere.Count > 0)
        {
            int Inx = MathUtil.GetRandomByWeight(ratio);
            return filtere[Inx];
        }
        else
        {
            return -1;
        }
    }

    public List<int> GetOrderProductIdAndRatio(List<string> needIem, out List<int> ratio)
    {
        List<int> filtere = new List<int>();
        ratio = new List<int> { };
        for (int i = 0; i < needIem.Count; i++)
        {
            string[] arr = needIem[i].Split('_');
            int productId = int.Parse(arr[0]);
            //过滤无产物的情况(废弃)
            //if (GetOrderGetProductBuild(productId) != null)
            {
                filtere.Add(productId);
                ratio.Add(int.Parse(arr[1]) * Const.Weight_Ex);
            }
        }
        return filtere;
    }

    private List<int> GetOrderList(int poolId)
    {
        if (!orderPoolList.ContainsKey(poolId))
        {
            List<int> datList = new List<int>() { };
            foreach (order cha in StaticDataMgr.Instance.orderInfo.Values)
            {
                if (cha.poolId == poolId)
                {
                    datList.Add(cha.id);
                }
            }
            orderPoolList[poolId] = datList;
        }
        return orderPoolList[poolId];
    }
    public void Release()
    {
   
    }
}
