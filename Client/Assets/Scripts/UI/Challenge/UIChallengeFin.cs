//----------------------------------------------------------------------------
//-- 挑战结束界面
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------
using UnityEngine;

public class UIChallengeFin : BaseUIChallengeFin
{
    protected override void OnOpened()
    {
        base.OnOpened();
    }

    public void SetData(UserChallengeShowData netInfo)
    {
        if (netInfo != null)
        {
           // m_ChanllengeName.text = netInfo.GetName();
        }
        //bool hasAwa = awa.Count > 0;
        //if (hasAwa)
        //{
        //    m_List.Clear(false);
        //    m_List.AddDataList(awa);
        //}
    }

    protected override void OnButtonClick(Component com)
    {
        base.OnButtonClick(com);
        if (com == m_CloseBN)
        {
            UIMgr.Close<UIChallengeFin>();
        }
    }


}
