//----------------------------------------------------------------------------
//-- view
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------
using UnityEngine;

public class UIBubble : BaseUIBubble
{

    protected override void OnOpened()
    {
        base.OnOpened();
        Refresh();
    }

    protected override void OnClosed()
    {
        base.OnClosed();
    }

    private void Refresh()
    {

    }


    protected override void OnButtonClick(Component com)
    {
        base.OnButtonClick(com);
        if (com == m_CancelBN)
        {
            UIMgr.Close<UIBubble>();
        }
        else if(com == m_OkBN)
        {
            UIMgr.Close<UIBubble>();
            ModuleMgr.AdMgr.ClickAd(AdEnum.AdType.Reward_Adgold, (adtype) => {
               // UIMgr.Get<UIFight>().StartBubbleEffect();
            });
        }
    }
}
