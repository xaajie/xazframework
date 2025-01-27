//----------------------------------------------------------------------------
//-- Æ¯¸¡ÎÄ±¾
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------

public class UIFlyTip : BaseUIFlyTip
{
    protected override void OnOpened()
    {
        base.OnOpened();
        scheduler.Timeout(delegate ()
        {
            UIMgr.Close<UIFlyTip>();
        }, 1);
    }

    public void SetFlyInfo(string desc)
    {
        m_Desc.text = desc;
    }
   
}
