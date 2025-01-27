//----------------------------------------------------------------------------
//-- view
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using Xaz;

public class UIBook : BaseUIBook
{
    public const string CELLNAME1 = "big";
    public const string CELLNAME2 = "small";
    private Dictionary<int, int> perPage = new Dictionary<int, int>();
    int curPage = 1;
    private int selectId = (int)Const.Category.ITEM;
    private Dictionary<int,List<UserBookCellData>> dataList =  new Dictionary<int, List<UserBookCellData>>();
    private Dictionary<int, string> protxt= new Dictionary<int, string>();
    UserBookCellData curSlect;
    protected override void OnOpened()
    {
        base.OnOpened();
        m_TabList.Clear(false);
        m_TabList.AddData(Const.Category.ITEM);
        m_TabList.AddData(Const.Category.ACTOR);
        Refresh();
    }

    protected override void OnClosed()
    {
        base.OnClosed();
        NetMgr.NetLogin.SendSynUser();
    }

    private void Refresh()
    {
        List<UserBookCellData> showList = null;
        if (dataList.ContainsKey(selectId))
        {
            showList = dataList[selectId];
        }
        else
        {
            showList = new List<UserBookCellData>();
            int opennum = 0;
            UserBookCellData newInfo;
            if (selectId == (int)Const.Category.ITEM)
            {
                foreach (item cha in StaticDataMgr.Instance.itemInfo.Values)
                {
                    newInfo = new UserBookCellData(selectId,cha.id);
                    showList.Add(newInfo);
                    if (!newInfo.IsLock())
                    {
                        opennum++;
                    }
                }
                perPage.Add(selectId,9);
            }
            else if (selectId == (int)Const.Category.ACTOR)
            {
                foreach (actor cha in StaticDataMgr.Instance.actorInfo.Values)
                {
                    newInfo  = new UserBookCellData(selectId, cha.id);
                    showList.Add(newInfo);
                    if (!newInfo.IsLock())
                    {
                        opennum++;
                    }
                }
                perPage.Add(selectId, 6);
            }
            dataList.Add(selectId,showList);
            protxt.Add(selectId, string.Format("{0}/{1}", opennum,showList.Count));
        }
        m_List.Clear(true);
        for (int i = 0; i < showList.Count; i++)
        {
            if (Math.Ceiling((double)(i + 1) / perPage[selectId]) == curPage)
            {
                m_List.AddData(showList[i], showList[i].GetCellName());
            }
        }
        m_PageNum.text = string.Format("{0}/{1}",curPage, GetMaxPage(selectId));
        m_ProTxt.text = protxt[selectId];
    }



    protected override void OnButtonClick(Component com)
    {
        base.OnButtonClick(com);
        if (com == this.m_CloseBN)
        {
            UIMgr.Close<UIBook>();
        }
        else if (com == this.m_RightBN)
        {
            if (curPage < GetMaxPage(selectId))
            {
                curPage++;
                Refresh();
            }
        }
        else if (com == this.m_LeftBN)
        {
            if (curPage > 1)
            {
                curPage--;
                Refresh();
            }
        }
    }

    public int GetMaxPage(int type)
    {
        return (int)Math.Ceiling((double)dataList[type].Count /(perPage[selectId]));
    }

    override protected void OnTableViewCellInit(UITableView tableView, UITableViewCell tableCell, object data)
    {
        base.OnTableViewCellInit(tableView, tableCell, data);
        if (tableView == m_TabList)
        {
            int itemType = (int)data;
            TV_TabList.Cell0 cell = this.GetCellView(tableView, tableCell) as TV_TabList.Cell0;
            cell.Txt.text = Utils.GetLang("category" + itemType);
            cell.Txt2.text = cell.Txt.text;
            Utils.SetActive(cell.Select.gameObject, itemType == selectId);
        }
        else if (tableView == m_List)
        {
            UserBookCellData info = data as UserBookCellData;
            if (info.needTween)
            {
                FadeInOut.FadeFrom(tableCell.gameObject, 0, 1, 0.5f, null);
                info.needTween = false;
            }
            if (tableCell.identifier == UIBook.CELLNAME2)
            {
                TV_List.Cell0 cell = this.GetCellView(tableView, tableCell)  as TV_List.Cell0;
                cell.Icon.SetSprite(info.GetBox().GetAtlas(), info.GetBox().GetIcon());
                cell.Stt.SetState(info.IsLock()?"lock":"open");
                cell.Txt.text = info.GetBox().GetName();
                Utils.SetActive(cell.SelectEffect.gameObject,curSlect == info);
                Utils.SetActive(cell.Red.gameObject,info.CanAwa());
            }
            else if (tableCell.identifier == UIBook.CELLNAME1)
            {
                TV_List.Cell1 cell = this.GetCellView(tableView, tableCell)  as TV_List.Cell1;
                cell.Icon.SetSprite(info.GetBox().GetAtlas(), info.GetBox().GetIcon());
                cell.Stt.SetState(info.IsLock() ? "lock" : "open");
                cell.Txt.text = info.GetBox().GetName();
                Utils.SetActive(cell.SelectEffect.gameObject, curSlect == info);
                Utils.SetActive(cell.Red.gameObject, info.CanAwa());
            }
        }
    }

    override protected void OnTableViewCellClick(UITableView tableView, UITableViewCell tableCell, GameObject target, object data)
    {
        base.OnTableViewCellClick(tableView, tableCell, target, data);
        if (tableView == m_TabList)
        {
            selectId = (int)data;
            curPage = 1;
            m_TabList.Refresh();
            Refresh();
        }
        else
        {
            UserBookCellData info = data as UserBookCellData;
            if (!info.IsLock())
            {
                if (info.CanAwa())
                {
                    ModuleMgr.BookMgr.SaveFinBookAwa(info, target.transform);
                    Refresh();
                }
                curSlect = info;
                UIMgr.Open<UIBookSee>(uiview => uiview.SetData(info));
                UIMgr.WaitCloseUICallBack<UIBookSee>(() =>
                {
                    curSlect = null;
                    m_List.Refresh();
                });
                m_List.Refresh();
            }
        }
    }
}
