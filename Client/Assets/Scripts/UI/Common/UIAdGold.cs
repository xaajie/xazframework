//----------------------------------------------------------------------------
//-- 广告补充界面弹框
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIAdGold : BaseUIAdGold
{
    private UserCategoryData awa;
    public delegate void AdGoldDelegate();
    private AdGoldDelegate adBack;
    protected override void OnOpened()
    {
        base.OnOpened();
        awa = ModuleMgr.CategoryMgr.CreateCurrency(Const.CurrencyType.GOLD, 100);
    }

  
   protected override void OnClosed()
    {
        base.OnClosed();
    }

    public void SetData(int lackGold, AdGoldDelegate adGetGoldBack= null)
    {
        UserChallengeShowData curChall = ModuleMgr.ChallengeMgr.GetCurChallege();
        adBack = adGetGoldBack;
        awa.itemNum = curChall.GetGoldSupplyNum(lackGold);
        m_Cost.SetBoxData(awa);
    }

    protected override void OnButtonClick(Component com)
    {
        base.OnButtonClick(com);
        if (com == m_CancelBN || com == m_CloseBN)
        {
            UIMgr.Close<UIAdGold>();
        }
        else
        {
            ModuleMgr.AdMgr.ClickAd(AdEnum.AdType.Reward_Adgold, (adtype) => {
                ModuleMgr.AwardMgr.AwardListone(awa, true);
                if (adBack != null)
                {
                    adBack();
                }
                UIMgr.Close<UIAdGold>();
            });
        }
    }
}
