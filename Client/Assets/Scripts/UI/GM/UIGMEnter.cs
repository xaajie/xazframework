//----------------------------------------------------------------------------
//-- view
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xaz;

public class UIGMEnter : BaseUIGMEnter
{

    private bool showDebug = false;
    protected override void OnOpened()
    {
        base.OnOpened();
        m_InputField.onSubmit.AddListener(OnSubmitGM);
        m_BigInputField.onSubmit.AddListener(OnSubmitMutiGM);
        Refresh();
    }

    public void SetInputGM(string cot)
    {
        m_InputField.text = cot;
    }
    Coroutine co;
    private void OnSubmitMutiGM(string strCMD)
    {
        m_BigInputField.text = string.Empty;
        co = XazHelper.StartCoroutine(HandlerCMD(strCMD));
    }

    IEnumerator HandlerCMD(string strCMD)
    {
        string[] table = strCMD.Split('\n');
        for (int i = 0; i < table.Length; i++)
        {
            SendGM(table[i]);
            UIMgr.ShowFlyTip("正在执行:"+i+ table[i]);
            yield return new WaitForSeconds(0.2f);
        }
        UIMgr.ShowFlyTip("执行结束");
        yield return XazHelper.waitFrame;
        co = null;
    }
    private void OnSubmitGM(string strCMD)
    {
        m_InputField.text = string.Empty;
        SendGM(strCMD);
        UIMgr.ShowFlyTip("执行结束");
    }
    private void Refresh()
    {
        Utils.SetActive(m_DebugSpt.gameObject, showDebug);
        Utils.SetActive(m_DebugBN.gameObject, !showDebug);
    }

    override protected void OnTableViewCellInit(UITableView tableView, UITableViewCell tableCell, object data)
    {
        base.OnTableViewCellInit(tableView, tableCell, data);

    }

    override protected void OnTableViewCellClick(UITableView tableView, UITableViewCell tableCell, GameObject target, object data)
    {
        base.OnTableViewCellClick(tableView, tableCell, target, data);

    }

    override protected void OnSubGroupCellInit(UISubGroup tableView, UITableViewCell tableCell, object data, string SubGroupname)
    {
        base.OnSubGroupCellInit(tableView, tableCell, data, SubGroupname);

    }

    override protected void OnSubGroupCellClick(UISubGroup tableView, UITableViewCell tableCell, GameObject target, object data, string SubGroupname)
    {
        base.OnSubGroupCellClick(tableView, tableCell, target, data, SubGroupname);
    }
    protected override void OnButtonClick(Component com)
    {
        base.OnButtonClick(com);
        if (com == this.m_DebugBN || com == this.m_ClosegmBN)
        {
            showDebug = !showDebug;
            Refresh();
        }
        else if (com == m_GmBN)
        {
            UIMgr.Open<UIGM>();
        }
        else if (com == this.m_ResetBN)
        {
            ClientServerCenter.Instance.ClearRecord();
            ModuleMgr.LoginMgr.OutLogin();
        }
    }

    private void SendGM(string cmd)
    {
        string[] arr = cmd.Split(',');
        switch (arr[0])
        {
            case "setturn":
                Profile.Instance.testID = arr[1];
                break;
            case "setturnitem":
                Profile.Instance.testItemID = arr[1];
                break;
            default:
                NetMgr.NetLogin.SendGMCallBack(cmd);
                break;
        }
    }
}
