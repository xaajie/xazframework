//----------------------------------------------------------------------------
//-- ¿ÎœﬂΩ±¿¯ΩÁ√Ê
//-- <write your instructions here>
//-- @author xz
//----------------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using Xaz;

public class UIOfflineReward : BaseUIOfflineReward
{
    List<UserCategoryData> awardList;
    List<UserCategoryData> adAwardList = new List<UserCategoryData>();
    protected override void OnOpened()
    {
        base.OnOpened();

        awardList = ModuleMgr.MainMgr.GetShowOffLineAward();
        int awardTime = Profile.Instance.user.onLineAwardTime;
        foreach (UserCategoryData info in awardList)
        {
            info.itemNum *= Mathf.CeilToInt(awardTime/60);
            UserCategoryData data = new UserCategoryData(awardList[0].GetItemType(), awardList[0].GetID(), awardList[0].GetNum());
            data.itemNum = Mathf.FloorToInt(data.itemNum * Constant.Offline_AdRate / 100);
            adAwardList.Add(data);
        }
        m_Box.SetBoxData(awardList[0]);
        m_Adbox.SetBoxData(adAwardList[0]);
        m_TimeTxt.text = string.Format(Utils.GetLang("offline_time"), TimeUtil.FormatTime(awardTime));
        m_AddNum.text = string.Format("+ {0}%",(Constant.Offline_AdRate - 100));
    }

    private void GetAward(bool isAd)
    {
        if (isAd)
        {
            UIMgr.Open<UIAward>(uiView => uiView.SetData(adAwardList));
            ModuleMgr.AwardMgr.AwardList(adAwardList, true);
        }
        else
        {
            UIMgr.Open<UIAward>(uiView => uiView.SetData(awardList));
            ModuleMgr.AwardMgr.AwardList(awardList, true);
        }
        Profile.Instance.user.onLineAwardTime = 0;
        NetMgr.NetLogin.SendSynUser();
        UIMgr.Close<UIOfflineReward>();
    }

    protected override void OnButtonClick(Component com)
    {
        base.OnButtonClick(com);
        if (com == m_CloseBN)
        {
            UIMgr.Close<UIOfflineReward>();
        }
        else if(com == m_AdGetBN)
        {
            ModuleMgr.AdMgr.ClickAd(AdEnum.AdType.Reward_Offline, (adtype) =>
            {
                GetAward(true);
            });
        }
        else if (com == m_GetBN)
        {
            GetAward(false);
        }
    }


}
