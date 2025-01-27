using System.Collections.Generic;
using UnityEngine;
using Xaz;

public class UIGameView : UIView
{
    private int defaultClickAudioId = AudioEnum.click;
    private Dictionary<string,int> specAudioDic = new Dictionary<string, int>();
    //private UIAnim anim;

    protected override void OnCreated()
    {
        if (setting.openAuido)
        {
            AudioMgr.Instance.Play(AudioEnum.UIShow);
        }
        base.OnCreated();
    }

    //protected override void OnOpened()
    //{
    //    Logger.Print(gameObject.name);
    //    if (anim != null)
    //    {
    //        Logger.Print("ssssssssssss",gameObject.name);
    //        anim.ResetAni();
    //    }
    //    base.OnOpened();
    //}
    protected override void OnClosed()
    {
        specAudioDic.Clear(); 
        base.OnClosed();
    }

    protected override void OnButtonClick(Component com)
    {
        PlayAudio(com.name);
        base.OnButtonClick(com);
    }

    private void PlayAudio(string com)
    {
        if (specAudioDic.ContainsKey(com))
        {
            AudioMgr.Instance.Play(specAudioDic[com]);
        }
        else
        {
            AudioMgr.Instance.Play(defaultClickAudioId);
        }
    }
    override protected void OnTableViewCellPress(UITableView tableView, UITableViewCell tableCell, GameObject target, object data)
    {
        PlayAudio(tableView.name);
        base.OnTableViewCellPress(tableView, tableCell, target, data);

    }

    override protected void OnFixTableViewCellPress(UIFixTableView tableView, UIFixTableViewCell tableCell, GameObject target, object data)
    {
        PlayAudio(tableView.name);
        base.OnFixTableViewCellPress(tableView, tableCell, target, data);

    }
    public void RegisterViewAudio(string com,int id)
    {
        if (specAudioDic.ContainsKey(com))
        {
            specAudioDic[com] = id;
        }
        else
        {
            specAudioDic.Add(com, id);
        }
    }
}
