//----------------------------------------------------------------------------
//-- Íæ·¨½çÃæ
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------
using System.Collections.Generic;
using Table;
using UnityEngine;

public class UILvup : BaseUILvup
{
    private UserCategoryData awa1;
    private UserCategoryData awa2;
    protected override void OnOpened()
    {
        base.OnOpened();
        AudioMgr.Instance.Play(AudioEnum.lvup);
    }

    protected override void OnClosed()
    {
        base.OnClosed();
        NetMgr.NetLogin.SendSynUser();
    }
    public void SetData(int level)
    {
        level info = StaticDataMgr.Instance.levelInfo[level];
        awa1 = ModuleMgr.CategoryMgr.CreateCurrency(Const.CurrencyType.GOLD, info.gold);
        m_Awabox1.SetBoxData(awa1);
        awa2 = ModuleMgr.CategoryMgr.CreateCurrency(Const.CurrencyType.GOLD, info.adGold);
        m_Awabox2.SetBoxData(awa2);
        //UserCategoryData showbox = Profile.Instance.user.GetLvOpenStock(level);
        //m_Box.SetBoxData(showbox);
        //Utils.SetActive(m_Box.gameObject, showbox.itemType != Const.Category.ITEM);
        //m_Box2.SetBoxData(showbox);
        //Utils.SetActive(m_Box2.gameObject,showbox.itemType == Const.Category.ITEM);
    }

    protected override void OnButtonClick(Component com)
    {
        base.OnButtonClick(com);
        if (com == m_AwaBN1)
        {
            ModuleMgr.AwardMgr.AwardListone(awa1, true);
            UIMgr.Close<UILvup>();
            EventMgr.DispatchEvent(EventEnum.UIFIHGT_REFRESHTop);
        }
        else if(com == m_AwaBN2)
        {
            ModuleMgr.AdMgr.ClickAd(AdEnum.AdType.Reward_Lvup, (adtype) => {
                ModuleMgr.AwardMgr.AwardListone(awa2, true);
                UIMgr.Close<UILvup>();
                EventMgr.DispatchEvent(EventEnum.UIFIHGT_REFRESHTop);
            });
        }
    }

}
