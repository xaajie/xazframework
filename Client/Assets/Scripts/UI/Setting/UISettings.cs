//----------------------------------------------------------------------------
//-- view
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------
using System;
using UnityEngine;
using Xaz;

public class UISettings : BaseUISettings
{

    protected override void OnOpened()
    {
        base.OnOpened();
        ModuleMgr.AdMgr.PlayBannerAD();
        Refresh();
    }

    protected override void OnClosed()
    {
        base.OnClosed();
        AudioMgr.Instance.Save();
        ModuleMgr.AdMgr.CloseBannerAD();
    }

    private void Refresh()
    {
        m_MusicTo.SetIsOnWithoutNotify(AudioMgr.Instance.musicVal>0);
        m_SoundTo.SetIsOnWithoutNotify(AudioMgr.Instance.soundVal > 0);
        m_ShakeTo.SetIsOnWithoutNotify(Profile.Instance.user.CanShake());
    }


    override protected void OnValueChanged(Component com, object value)
    { 
        base.OnValueChanged(com, value);
        if (com.name == m_MusicTo.name)
        {
            AudioMgr.Instance.SetMusicVolume((bool)value==true?1:0);
        }
        else if (com.name == m_SoundTo.name)
        {
            AudioMgr.Instance.SetSoundVolume((bool)value == true ? 1 : 0);
        }
        else if (com.name == m_ShakeTo.name)
        {
            Profile.Instance.user.closeshake = !(bool)value;
        }
    }

    protected override void OnButtonClick(Component com)
    {
        base.OnButtonClick(com);
        if (com == this.m_CloseBN)
        {
            UIMgr.Close<UISettings>();
        }else if (com == m_ServicerBN)
        {
            Debug.Log("------OpenService--------");
            SDKMgr.Instance.OpenService();
        }
        else if (com == this.m_ClearBN)
        {
            UIMgr.ShowConfirm(Utils.GetLang("gamclearttips"), () =>
            {
                ClientServerCenter.Instance.ClearRecord();
                ModuleMgr.LoginMgr.OutLogin();
                SDKMgr.Instance.ReLogin();
            });
        }
    }
}
