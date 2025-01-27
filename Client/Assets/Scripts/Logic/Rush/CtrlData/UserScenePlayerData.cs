using Table;

public class UserScenePlayerData : UserScenePlayerDataBase
{

    public float moveSpeed;
    public int capacity;
    public UserScenePlayerData() : base()
    {
    }

    public actor GetInfo()
    {
        return StaticDataMgr.Instance.actorInfo[id];
    }

    public int GetChallengeOwnId()
    {
        return this.ownId > 0 ? this.ownId : 1;
    }

    public float GetRawSpeedVal()
    {
        return (float)GetInfo().walkspeed / 100;
    }
    public void RefreshData()
    {
        moveSpeed = GetRawSpeedVal() + ModuleMgr.AttrMgr.GetCountAddVal(GetRawSpeedVal(), (int)AttrEnum.movespeed, (int)Const.ActorType.Player);
        capacity = GetInfo().Capacity + (int)ModuleMgr.AttrMgr.GetCountAddVal(GetInfo().Capacity, (int)AttrEnum.handstacknum, (int)Const.ActorType.Player);
    }

    public float GetSpeedVal()
    {
        return moveSpeed;
    }
    public int GetCapacity()
    {
        return capacity;
    }
}
