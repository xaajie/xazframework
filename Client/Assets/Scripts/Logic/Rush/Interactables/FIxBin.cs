public class FixBin : Interactable
{
    public int ownBuildId;

    public void SetOwnID(int id)
    {
        ownBuildId = id;
    }
    protected override void OnPlayerEnter()
    {
        if (ownBuildId > 0)
        {
            ModuleMgr.FightMgr.FixBuild(ownBuildId,true);
        }
    }


}

