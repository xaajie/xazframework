//----------------------------------------------------------------------------
//-- Íæ·¨½çÃæ
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using Xaz;

public class UIHappen : BaseUIHappen
{
    private UserHappeningData info;
    protected override void OnOpened()
    {
        base.OnOpened();
        scheduler.Interval(delegate ()
        {
            UpdateInv();
        }, 0.1f);
        //AudioMgr.Instance.Play(AudioEnum.lvup);
    }

    private void UpdateInv()
    {
        if (info.GetLeftTime() > 0)
        {
            m_TimeTxt.text = string.Format(Utils.GetLang("happen_left"), info.GetLeftTime());
        }
        else
        {
            UIMgr.Close<UIHappen>();
        }
    }

    public void SetData(UserHappeningData awa)
    {
        info = awa;
        m_Title.text = awa.GetName();
        m_Icon.SetSprite(awa.GetAtlas(),awa.GetIcon());
        m_Desc.text = awa.GetDesc();
    }

    protected override void OnButtonClick(Component com)
    {
        base.OnButtonClick(com);
        if (com == m_CloseBN)
        {
            UIMgr.Close<UIHappen>();
        }
        else if(com == m_AwardBN)
        {
            ModuleMgr.AdMgr.ClickAd(AdEnum.AdType.Reward_Adhappen, (adtype) =>
            {
                info.SetSeeAdTime();
                if (info.GetHappenType() == (int)Const.HappenType.Money)
                {
                    List<UserCategoryData> awalist = ModuleMgr.CategoryMgr.CreateFromList(info.GetInfo().param);
                    ModuleMgr.AwardMgr.AwardList(awalist, true);
                    UIMgr.Open<UIFlyAward>(uiview => uiview.SetFlyInfo(awalist, m_AwardBN.transform.position, Const.GoldAwaEffect.onlygold));
                    UIMgr.WaitCloseUICallBack<UIFlyAward>(() =>
                    {
                        UIMgr.Close<UIHappen>();
                    });
                }
                else if (info.GetHappenType() == (int)Const.HappenType.GetBuff)
                {
                    ModuleMgr.BuffMgr.AddBuff(info.GetBuffID());
                    UIMgr.Close<UIHappen>();
                }
               // UIMgr.Close<UIHappen>();
            });
        }
        else if (com == m_CancelBN)
        {
            UIMgr.Close<UIHappen>();
        }
    }


}
