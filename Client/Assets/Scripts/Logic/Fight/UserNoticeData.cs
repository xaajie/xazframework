//----------------------------------------------------------------------------
//--UserNoticeData数据封装
//-- @author xiejie
//----------------------------------------------------------------------------

using Table;
using UnityEngine;
using Xaz;

public class UserNoticeData : Data

{
    public int uid;
    private Const.NoticeType noticeType;
    private WorkerCtrl worker;
    private BuildController build;
    private int createTime;
    public UserNoticeData(Const.NoticeType sid, int suid) : base()
    {
        UpdateNoticeData(sid,suid);
    }

    public void UpdateNoticeData(Const.NoticeType sid, int suid)
    {
        noticeType = sid;
        uid = suid;
        createTime = TimeUtil.GetNowInt();
        if (suid > 0)
        {
            worker = ModuleMgr.FightMgr.GetActorByUid(uid);
            build = ModuleMgr.FightMgr.GetBuildByUid(uid);
        }
    }

    public bool IsNull()
    {
        return noticeType == Const.NoticeType.None;
    }
    public int GetCreateTime()
    {
        return createTime;
    }
    public string GetIcon()
    {
        if(noticeType == Const.NoticeType.Sleep)
        {
            return worker.GetCtrlData().GetSmallIcon();

        }
        else if(noticeType == Const.NoticeType.Broken)
        {
            return build.GetCtrlData().GetIcon();
        }
        return string.Empty;
    }

    public Transform GetClickActionTarget()
    {
        if (noticeType == Const.NoticeType.Sleep)
        {
            return worker.transform;
        }
        else if (noticeType == Const.NoticeType.Broken)
        {
            return build.transform;
        }
        return null;
    }

    public Const.NoticeType GetNoticeType()
    {
        return noticeType;
    }
    public string GetAtlas()
    {
        return Const.AtlasIcon;
    }

    public bool CheckTimeValid()
    {
        if (noticeType == Const.NoticeType.Sleep)
        {
            if (worker != null && worker.step == Const.ActorStep.Sleep)
            {
                return true;
            }
            else
            {
                noticeType = Const.NoticeType.None;
            }
        }
        if (noticeType == Const.NoticeType.Broken)
        {
            if (build != null && build.GetCtrlData().IsBroken())
            {
                return true;
            }
            else
            {
                noticeType = Const.NoticeType.None;
            }
        }
        return false;
    }
}
