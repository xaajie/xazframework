//----------------------------------------------------------------------------
//-- Æ¯¸¡ÎÄ±¾
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xaz;

public class FlyAwaData
{
    public UserCategoryData box;
    public Const.GoldAwaEffect goldstate;
    public Vector3 fromworldpos;
    public FlyAwaData(UserCategoryData content, Vector3 oj, Const.GoldAwaEffect gt) : base()
    {
        box = content;
        fromworldpos = oj;
        goldstate = gt;
    }
}

public class UIFlyAward : BaseUIFlyAward
{
    List<FlyAwaData> flyList = new List<FlyAwaData>();
    private bool cancheck = false;
    protected override void OnOpened()
    {
        base.OnOpened();
        cancheck = false;
        //scheduler.Interval(delegate ()
        //{
        //    UpdateInv();
        //}, 0.1f);
    }

    //private void UpdateInv()
    //{
    //    if (flyList.Count > 0)
    //    {
    //        m_Fixlist.AddData(flyList[flyList.Count - 1]);
    //        flyList.RemoveAt(flyList.Count - 1);
    //    }
    //    //if (cancheck)
    //    //{
    //    //    if (m_Fixlist.dataCount <= 0 && flyList.Count<=0)
    //    //    {
    //    //        UIMgr.Close<UIFlyAward>();
    //    //    }
    //    //}
    //}

    private void CheckShow()
    {
        if (flyList.Count > 0)
        {
            m_Fixlist.AddData(flyList[flyList.Count - 1]);
            flyList.RemoveAt(flyList.Count - 1);
        }
        if (m_Fixlist.dataCount <= 0 && flyList.Count <= 0)
        {
            UIMgr.Close<UIFlyAward>();
        }
    }
    public void SetFlyInfo(List<UserCategoryData> datlist, Vector3 fromwordpos, Const.GoldAwaEffect goldstate)
    {
        for (int i = 0; i < datlist.Count; i++)
        {
            flyList.Add(new FlyAwaData(datlist[i], fromwordpos, goldstate));
        }
        CheckShow();
        cancheck = true;
    }

    override protected void OnFixTableViewCellInit(UIFixTableView tableView, UIFixTableViewCell tablecell, object data)
    {
        base.OnFixTableViewCellInit(tableView, tablecell, data);
        if (tableView == m_Fixlist)
        {
            FlyAwaData dat = data as FlyAwaData;
            TV_Fixlist.Cell0 cell = this.GetCellView(tableView, tablecell) as TV_Fixlist.Cell0;
            cell.Box.SetBoxData(dat.box);
            cell.Num.text = string.Format("{0} x{1}", dat.box.GetName(), dat.box.GetNum());
            Utils.SetActive(cell.Gold.gameObject, false);
            Utils.SetActive(cell.Tip.gameObject, false);
            if (dat.fromworldpos == null)
            {
                tablecell.transform.localPosition = Vector3.zero;
            }
            else
            {
                tablecell.transform.position = dat.fromworldpos;
            }
            if (dat.goldstate == Const.GoldAwaEffect.flytipgold || dat.goldstate == Const.GoldAwaEffect.onlygold)
            {
                GoldTargetMove pmove = cell.Gold.GetComponent<GoldTargetMove>();
                if (dat.box.itemType == Const.Category.CURRENCY)
                {
                    pmove.transform.position = tablecell.transform.position;
                    pmove.Play(dat.box);
                    AudioMgr.Instance.Play(AudioEnum.lvup);
                }
                if (dat.goldstate == Const.GoldAwaEffect.onlygold)
                {
                    m_Ani.StartCoroutine(BeginShowGold(dat));
                }
            }
            if (dat.goldstate == Const.GoldAwaEffect.flytipgold || dat.goldstate == Const.GoldAwaEffect.onlytip)
            {
                Utils.SetActive(cell.Tip.gameObject, true);
                m_Ani.StartCoroutine(BeginShowBox(tablecell.gameObject, dat));
            }
        }
    }

    IEnumerator BeginShowGold(FlyAwaData dat)
    {
        yield return new WaitForSeconds(1f);
        m_Fixlist.RemoveData(dat);
        CheckShow();
    }
    IEnumerator BeginShowBox(GameObject obj, FlyAwaData dat)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        FadeInOut.FadeFrom(obj, 0.1f, 1, 0.3f, null);
        rect.DOMoveY(obj.transform.position.y + 1, 0.3f);
        yield return new WaitForSeconds(1f);
        FadeInOut.FadeFrom(obj, 1, 0f, 0.2f, null);
        yield return new WaitForSeconds(0.2f);
        m_Fixlist.RemoveData(dat);
        CheckShow();
    }
}
