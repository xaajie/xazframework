//----------------------------------------------------------------------------
//-- UserNoviceData
//-- @author xz
//----------------------------------------------------------------------------

using System.Collections.Generic;
using Table;

public class UserNoviceData : Data
{
    private int id;

    public UserNoviceData(int sid) : base()
    {
        this.id = sid;
    }

    public int GetID()
    {
        return id;
    }

    public novice GetInfo()
    {
        novice info = StaticDataMgr.Instance.noviceInfo[id];
        if (info != null)
        {
            return info;
        }
        else
        {
            Logger.Print("新手任务数据错误，id = ", id);
            return null;
        }
    }

    public string GetDesc()
    {
        return GetInfo().desc;
    }

    public int GetPreDelay()
    {
        return GetInfo().preDelay;
    }

    public bool IsLast()
    {
        return GetInfo().nextId > 0 ? false : true;
    }

    public int GetNoviceType()
    {
        return GetInfo().noviceType;
    }

    public int GetNextId()
    {
        return GetInfo().nextId;
    }

    public List<string> GetFinishConAttr()
    {
        return GetInfo().finishConAttr;
    }

    public float GetFinishTime()
    {
        if(GetInfo().noviceType == (int)NoviceConst.NoviceType.Joystick)
        {
            return int.Parse(GetFinishConAttr()[0]) / 1000;
        }
        return 0;
    }

    public int GetUnlockID()
    {
        if (GetInfo().noviceType == (int)NoviceConst.NoviceType.UnLock)
        {
            return int.Parse(GetFinishConAttr()[0]);
        }
        return 0;
    }
    public int GetPosID()
    {
        if (GetInfo().noviceType == (int)NoviceConst.NoviceType.Building)
        {
            return int.Parse(GetFinishConAttr()[0]);
        }
        return 0;
    }
}
