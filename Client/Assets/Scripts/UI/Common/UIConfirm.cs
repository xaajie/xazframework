//----------------------------------------------------------------------------
//-- view
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------
using UnityEngine;

public class UIConfirm : BaseUIConfirm
{

    public delegate void ChooseDelegate();

    private ChooseDelegate okCallback;
    private ChooseDelegate cancelCallback;
    protected override void OnOpened()
    {
        base.OnOpened();
    }

    public void SetConfirmInfo(string desc,ChooseDelegate ok,ChooseDelegate cancel=null)
    {
        m_Desc.text = desc;
        okCallback = ok;
        cancelCallback = cancel;

    }
    public void SetConfirmInfo(string desc, ChooseDelegate ok, ChooseDelegate cancel, string bluetxt, string redtxt)
    {
        m_Desc.text = desc;
        okCallback = ok;
        cancelCallback = cancel;
        //m_BlueTxt.text = bluetxt;
        //m_RedTxt.text = redtxt;

    }
    override protected void OnValueChanged(Component com, object value)
    {
        base.OnValueChanged(com, value);
    }

    protected override void OnButtonClick(Component com)
    {
        base.OnButtonClick(com);
        if (com == this.m_OkBN)
        {
            if(okCallback!=null)
            {
                okCallback();
            }
        }
        else if(com == this.m_CancelBN)
        {
            if (cancelCallback != null)
            {
                cancelCallback();
            }
        }
        UIMgr.Close<UIConfirm>();
    }
}
