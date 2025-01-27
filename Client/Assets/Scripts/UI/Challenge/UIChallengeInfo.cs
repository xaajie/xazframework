//----------------------------------------------------------------------------
//-- 关卡详情界面
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using Xaz;

public class UIChallengeInfo : BaseUIChallengeInfo
{
    private UserChallengeShowData showInfo;
    private bool CanOpenNewChallenge;
    protected override void OnOpened()
    {
        base.OnOpened();

    }

    public void SetData(UserChallengeShowData netInfo,bool crossChallenge)
    {
        CanOpenNewChallenge = crossChallenge;
        if (netInfo != null)
        {
            showInfo = netInfo;
            m_ChanllengeName.text = netInfo.GetName();
            m_Icon.SetSprite(netInfo.GetAtlas(), netInfo.GetIcon());
            bool isOpen = netInfo.IsFinishClickOpen();
            UIGray.SetGray(m_Icon.gameObject, !isOpen, true);
            Utils.SetActive(m_Open.gameObject,isOpen);
            Utils.SetActive(m_NoOpen.gameObject, !isOpen);
            if (!isOpen)
            {
                UserChallengeShowData preInfo = netInfo.GetPreChallengeInfo();
                m_Desc.text = string.Format(Utils.GetLang("challengefilter"), preInfo.GetName());
                List<UserCategoryData> awas = preInfo.GetAward();
                if (awas.Count > 0)
                {
                    m_Cost.SetBoxData(awas[0]);
                }
                if (crossChallenge)
                {
                    AudioMgr.Instance.Play(AudioEnum.lvup);
                }
                else
                {
                    UIGray.SetGray(m_OpenBN.gameObject, !preInfo.IsFinishBar(), true);
                }
            }
        }
    }

    protected override void OnButtonClick(Component com)
    {
        base.OnButtonClick(com);
        if (com == m_EnterBN)
        {
            ModuleMgr.ChallengeMgr.CheckEnterChallengeScene(showInfo.GetID());
            UIMgr.Close<UIChallengeInfo>();
            UIMgr.Close<UIChallenge>();
        }
        else if (com == m_OpenBN)
        {
            if (CanOpenNewChallenge)
            {
                ModuleMgr.AchivementMgr.UpdateAchivement(Const.AchivementType.OpenChallenge, new int[] { showInfo.GetID() });
                showInfo.GetUserInfo().clickOpen = true;
                ModuleMgr.ChallengeMgr.CheckEnterChallengeScene(showInfo.GetID());
                UIMgr.Close<UIChallengeInfo>();
            }
            else
            {
                UIMgr.ShowFlyTip(m_Desc.text);
            }
        }
        else if(com == m_CloseBN)
        {
            UIMgr.Close<UIChallengeInfo>();
        }

    }

  

}
