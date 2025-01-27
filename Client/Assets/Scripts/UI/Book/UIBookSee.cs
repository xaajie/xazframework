//----------------------------------------------------------------------------
//-- view
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------
using UnityEngine;

public class UIBookSee : BaseUIBookSee
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
                UIMgr.Close<UIBookSee>();
            }
        }
    }
    protected override void OnClosed()
    {
        base.OnClosed();

    }

    public void SetData(UserBookCellData data)
    {
        m_Box.SetBoxData(data.GetBox());
    }
    protected override void OnButtonClick(Component com)
    {
        base.OnButtonClick(com);
        if (com == this.m_CloseBN)
        {
            UIMgr.Close<UIBookSee>();
        }
    }

}
