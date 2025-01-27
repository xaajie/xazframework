//----------------------------------------------------------------------------
//-- Æ¯¸¡ÎÄ±¾
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Xaz;

public class UITip : BaseUITip
{
    private List<RectTransform> curShowList;
    protected override void OnOpened()
    {
        base.OnOpened();
        curShowList = new List<RectTransform>();
        scheduler.Interval(delegate ()
        {
            UpdateInv();
        }, 0.3f);
    }

    PointerEventData eventData = new PointerEventData(EventSystem.current);
    private void UpdateInv()
    {
        eventData.position = Input.mousePosition;
        List<RaycastResult> list = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, list);
        bool isSelfRange = false;
        foreach (var item in list)
        {
            if (item.gameObject != null)
            {
                for (int i = 0; i < curShowList.Count; i++)
                {
                    if (item.gameObject == curShowList[i].gameObject)
                    {
                        isSelfRange = true;
                        break;
                    }
                }
            }
            if (isSelfRange)
            {
                break;
            }
        }
        if (!isSelfRange)
        {
            UIMgr.Close<UITip>();
        }
    }
    public void OpenUITip(IPackageData data, Transform target)
    {
        m_Fixlist.Clear(false);
        GenerTip(data, target);
    }

    public void AddUITip(IPackageData data, Transform target)
    {
        GenerTip(data, target);
    }

    private void GenerTip(IPackageData data, Transform target)
    {
        TipCellData nt = new TipCellData(data, target == null ? Vector3.zero : target.transform.position);
        m_Fixlist.AddData(nt);
    }

    private Vector2 initBgSigleHeight;
    override protected void OnFixTableViewCellInit(UIFixTableView tableView, UIFixTableViewCell tablecell, object data)
    {
        base.OnFixTableViewCellInit(tableView, tablecell, data);
        if (tableView == m_Fixlist)
        {
            TipCellData dat = data as TipCellData;
            TV_Fixlist.Cell0 cell = this.GetCellView(tableView, tablecell) as TV_Fixlist.Cell0;
            cell.Name.text = dat.info.GetName();
            cell.Icon.SetSprite(dat.info.GetAtlas(), dat.info.GetIcon());
            cell.Desc.text = dat.info.GetDesc();
            cell.UIState.SetState(dat.info.GetItemType() == (int)Const.Category.ITEM ? "sign" : "item");
            RectTransform bg = cell.Bg.GetComponent<RectTransform>();
            if (initBgSigleHeight == null)
            {
                initBgSigleHeight = bg.sizeDelta;
            }
            bg.sizeDelta = new Vector2(bg.sizeDelta.x, bg.sizeDelta.y + cell.Desc2.GetComponent<LayoutElement>().preferredHeight);
            tablecell.transform.position = dat.pos;
            //tablecell.GetComponent<UIEdgeSet>().RefreshEdge(0f);
            curShowList.Add(bg);
        }
    }

    override protected void OnFixTableViewCellClick(UIFixTableView tableView, UIFixTableViewCell tablecell, GameObject target, object data)
    {
        base.OnFixTableViewCellClick(tableView, tablecell, target, data);

    }
    protected override void OnButtonClick(Component com)
    {
        base.OnButtonClick(com);

    }
}

public class TipCellData
{
    public IPackageData info;
    public Vector3 pos;
    public TipCellData(IPackageData sinfo, Vector3 spos) : base()
    {
        info = sinfo;
        pos = spos;
    }
}
