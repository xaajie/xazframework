//----------------------------------------------------------------------------
//-- view
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------
using UnityEngine;
using Xaz;

public class UILoading : BaseUILoading
{

    public float curNum = 30;
    private int maxVal = 150;
    protected override void OnOpened()
    {
        base.OnOpened();
        Utils.SetActive(m_ChallengeBN.gameObject, false);
        m_Bar.value = 0f;
        scheduler.Update(delegate ()
        {
            UpdateInv();
        });
    }
    private void UpdateInv()
    {
        if (Time.frameCount % 3 == 0)
        { 
            curNum = curNum + 3f;
            m_Bar.value = curNum / maxVal;
            m_Txt.text = string.Format("{0}/{1}", Mathf.Min(100, Mathf.RoundToInt(m_Bar.value * 100)), 100);
            if (ModuleMgr.MainMgr != null && ModuleMgr.MainMgr.isInit)
            {
                curNum = maxVal;
            }
            if (curNum >= maxVal)
            {
                if (ModuleMgr.MainMgr != null && ModuleMgr.MainMgr.isInit)
                {
                    WaitCloseLoading();
                }
                else if (SDKMgr.Instance.NoHasAuth())
                {
                    Utils.SetActive(m_ChallengeBN.gameObject, true);
                    Utils.SetActive(m_Bar.gameObject, false);
                }

            }
        }
    }

    private bool isShow = false;
    private void WaitCloseLoading()
    {
        if (ModuleMgr.NoviceMgr.NeedStartNovice())
        {
            UIMgr.Close<UILoading>();
        }
        else
        {
            if (UIMgr.Get<UIBlackHole>() == null && !isShow)
            {
                isShow = true;
                UIMgr.Open<UIBlackHole>();
                scheduler.Timeout(delegate () {
                    UIMgr.Close<UILoading>();
                }, 0.3f);
            }
        }
    }
    protected override void OnButtonClick(Component com)
    {
        base.OnButtonClick(com);
        if (com == this.m_ChallengeBN)
        {
            WaitCloseLoading();
        }
    }
}
