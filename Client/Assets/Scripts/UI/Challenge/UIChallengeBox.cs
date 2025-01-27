//----------------------------------------------------------------------------
//-- 
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------
using UnityEngine;

public class UIChallengeBox : BaseUIChallengeBox
{
    protected override void OnOpened()
    {
        base.OnOpened();
        scheduler.Update(delegate ()
        {
            UpdateInv();
        });
    }

    private void UpdateInv()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!Utils.CheckClickInTarget(m_Bg.gameObject))
            {
                UIMgr.Close<UIChallengeBox>();
            }
        }
    }
    protected override void OnClosed()
    {
        base.OnClosed();
    }
    public void SetData(UserChallengeShowData info,Transform target)
    {
       m_Content.transform.position = target.position;
        m_Txt.text = info.IsOpen() ? Utils.GetLang("challegnebox1") : string.Format(Utils.GetLang("challegnebox2"));
    }



    protected override void OnButtonClick(Component com)
    {
        base.OnButtonClick(com);

    }

}
