//----------------------------------------------------------------------------
//-- 挑战关卡页面
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------
using System.Collections.Generic;
using Table;
using UnityEngine;
using Xaz;

public class UIChallenge : BaseUIChallenge
{
    private List<UserChallengeShowData> challengeList;
    private List<RectTransform> nodeList;
    protected override void OnOpened()
    {
        base.OnOpened();
        nodeList = new List<RectTransform>() { m_Node0, m_Node1, m_Node2, m_Node3, m_Node4, m_Node5, m_Node6, m_Node7, m_Node8, m_Node9 };
        AddEventListener(EventEnum.ChallengeInfo_REFRESH, Refresh);
        Refresh();
    }

    protected override void OnClosed()
    {
        base.OnClosed();
    }

    private void Refresh()
    {
        //章节那层已废止
        m_List.Clear(false);
        challengeList = ModuleMgr.ChallengeMgr.GetShowlist(Profile.Instance.fixChapterId);
        for (int i = 0; i < challengeList.Count; i++)
        {
            m_List.AddData(challengeList[i]);
        }
    }

    static string statelock = "lock";
    static string stateunlock = "unlock";
    override protected void OnFixTableViewCellInit(UIFixTableView tableView, UIFixTableViewCell tablecell, object data)
    {
        base.OnFixTableViewCellInit(tableView, tablecell, data);
        if (tableView == m_List)
        {
            UserChallengeShowData dat = data as UserChallengeShowData;
            TV_List.Cell0 cell = this.GetCellView(tableView, tablecell) as TV_List.Cell0;
            cell.Txt.text = dat.GetName();
            cell.UIState.SetState(dat.IsOpen() ? stateunlock : statelock);
            cell.Img.SetSprite(dat.GetAtlas(), dat.GetIcon());
            cell.LockImg.SetSprite(dat.GetAtlas(), dat.GetIcon());
            int nt = challengeList.IndexOf(dat);
            if (nt < nodeList.Count)
            {
                tablecell.transform.position = nodeList[nt].position;
            }
            else
            {
                tablecell.transform.position = nodeList[nodeList.Count - 1].position;
            }
            if (ModuleMgr.ChallengeMgr.GetCurChallege().id == dat.id)
            {
                m_PrefabLoader.transform.position = tablecell.transform.position;
            }
        }
    }

    override protected void OnFixTableViewCellClick(UIFixTableView tableView, UIFixTableViewCell tablecell, GameObject target, object data)
    {
        base.OnFixTableViewCellClick(tableView, tablecell, target, data);
        if (tableView == m_List)
        {
            UserChallengeShowData dat = data as UserChallengeShowData;
            //tablecell.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
            //if (dat.IsOpen())
            //{
            //    if (dat.id == ModuleMgr.ChallengeMgr.GetCurChallege().id)
            //    {
            //        ModuleMgr.ChallengeMgr.EnterChallenge();
            //        return;
            //    }
            //    DoTweenUtil.ClickZoomEffect(tablecell.transform, null);
            //}
            UIMgr.Open<UIChallengeInfo>(uiview => uiview.SetData(dat,false));
        }
    }

    protected override void OnButtonClick(Component com)
    {
        base.OnButtonClick(com);
        if(com == m_CloseBN)
        {
            UIMgr.Close<UIChallenge>();
        }

    }

}
