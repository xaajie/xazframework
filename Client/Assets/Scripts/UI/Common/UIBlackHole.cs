//----------------------------------------------------------------------------
//-- view
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------
public class UIBlackHole : BaseUIBlackHole
{
    protected override void OnOpened()
    {
        base.OnOpened();
        scheduler.Timeout(delegate ()
        {
            UIMgr.Close<UIBlackHole>();
        },1.3f);
    }

}
