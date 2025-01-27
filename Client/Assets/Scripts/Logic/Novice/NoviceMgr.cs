public class NoviceMgr
{
    public NoviceMgr()
    {
        
    }
    public int InitCurrNoviceId()
    {
        if (Profile.Instance.user.noviceId == 0)
        {
            Profile.Instance.user.noviceId = NoviceConst.Novice_StartId;
        }
        return Profile.Instance.user.noviceId;
    }
    public bool NeedStartNovice()
    {
        if (ModuleMgr.MainMgr.isInit && Profile.Instance.user.noviceId >= 0)
        {
            return true;
        }
        return false;
    }

    public bool IsInNovice()
    {
        return Profile.Instance.user.noviceId > 0;
    }

    public void CheckStartNovice()
    {
        if(NoviceConst.OpenNovice && NeedStartNovice())
        {
            UIMgr.Open<UINovice>();
        }
    }
    public void CheckEndNovice()
    {
        if (UIMgr.Get<UINovice>() != null)
        {
            UIMgr.Close<UINovice>();
        }
    }
    // Update is called once per frame
    public void Release()
    {
        
    }
}
