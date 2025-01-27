//----------------------------------------------------------------------------
//-- view
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;

public class UIActorDesc : BaseUIActorDesc
{
    protected override void OnOpened()
    {
        base.OnOpened();
    }

    protected override void OnClosed()
    {
        base.OnClosed();
    }


    public void SetData(UserSceneWorkerData info)
    {
        m_Icon.SetSprite(info.GetAtlas(),info.GetIcon());
        m_NameTxt.text = info.GetName();
        m_Help.text = info.GetHelpDesc();
        m_Desc.text = info.GetDesc();
        List<string> vts = new List<string>();
        if (info.GetInfo().walkspeed > 0)
        {
            UserAttrData attr2 = new UserAttrData((int)AttrEnum.movespeed, info.GetInfo().walkspeed, info.GetInfo().walkspeed);
            vts.Add(string.Format("{0}:{1}", attr2.GetName(), attr2.GetNum()));
        }
        if(info.GetInfo().Capacity > 0)
        {
            UserAttrData attr1 = new UserAttrData((int)AttrEnum.handstacknum, info.GetInfo().Capacity, info.GetInfo().Capacity);
            vts.Add(string.Format("{0}:{1}", attr1.GetName(), attr1.GetNum()));
        }
        m_Attr.text = string.Join("   ", vts.ToArray());
    }



    protected override void OnButtonClick(Component com)
    {
        base.OnButtonClick(com);
        if (com == this.m_CloseBN || com == this.m_CloseBN2)
        {
            UIMgr.Close<UIActorDesc>();
        }
    }

}
