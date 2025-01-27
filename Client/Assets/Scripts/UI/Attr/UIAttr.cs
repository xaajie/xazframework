//----------------------------------------------------------------------------
//-- view
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using Xaz;

public class UIAttr : BaseUIAttr
{
    protected override void OnOpened()
    {
        base.OnOpened();
        m_Title.text = Utils.GetLang(ModuleMgr.MainMgr.GetMainBtnDataById((int)Const.MainViewID.attrup).GetName());
        List<UserAttrupShowData> listdat = ModuleMgr.AttrMgr.CheckGetShowAttrupList();
        m_List.Clear(false);
        m_List.AddDataList(listdat);
    }

    protected override void OnClosed()
    {
        base.OnClosed();
        NetMgr.NetLogin.SendSynUser();
    }

    private void Refresh()
    {
        m_List.Refresh();
    }



    protected override void OnButtonClick(Component com)
    {
        base.OnButtonClick(com);
        if (com == this.m_CloseBN)
        {
            UIMgr.Close<UIAttr>();
        }
    }

    string cellstate_full = "full";
    string cellstate_lvup = "up";
    string cellstate_canlvup = "canup";
    override protected void OnTableViewCellInit(UITableView tableView, UITableViewCell tableCell, object data)
    {
        base.OnTableViewCellInit(tableView, tableCell, data);
        if (tableView == m_List)
        {
            UserAttrupShowData info = data as UserAttrupShowData;
            TV_List.Cell0 cell = this.GetCellView(tableView, tableCell) as TV_List.Cell0;
            cell.Lvtxt.text = string.Format("Lv.{0}", info.GetLevel());
            cell.Buildicon.SetSprite(info.GetAtlas(), info.GetIcon());
            cell.AttrName.text = info.GetDesc();
            cell.AttrCurNum.text = info.GetAttrNumStr();
            bool isfull = info.IsFullLv();
            if (isfull)
            {
                cell.Cellstate.SetState(cellstate_full);
            }
            else
            {
                cell.AttrNum.text = info.GetNextInfo().GetAttrNumStr();
                bool canLvup = info.CheckCanLvup();
                cell.Cellstate.SetState(canLvup ? cellstate_canlvup : cellstate_lvup);
                if (!info.IsAdUp())
                {
                    cell.Cost.SetBoxData(info.GetLvupCostInfo());
                }
                Utils.SetActive(cell.Cost.gameObject, !info.IsAdUp());
                Utils.SetActive(cell.Adcost.gameObject, info.IsAdUp());
            }
        }
    }

    override protected void OnTableViewCellClick(UITableView tableView, UITableViewCell tableCell, GameObject target, object data)
    {
        base.OnTableViewCellClick(tableView, tableCell, target, data);
        if (tableView == m_List)
        {
            //Éý¼¶´¦Àí
            UserAttrupShowData info = data as UserAttrupShowData;
            if (info.IsAdUp())
            {
                ModuleMgr.AdMgr.ClickAd(AdEnum.AdType.Reward_Attrup, (adtype) => {
                    LevelupAction(info);
                });
            }
            else
            {
                UserCategoryData costInfo = info.GetLvupCostInfo();
                if (ModuleMgr.CategoryMgr.CheckOwn(costInfo, costInfo.GetNum(), Const.CheckTipType.Fly))
                {
                    ModuleMgr.AwardMgr.AwardListone(costInfo, false);
                    LevelupAction(info);
                }
            }

        }

    }

    private void LevelupAction(UserAttrupShowData info)
    {
        UserAttrupDataBase attrdata = Profile.Instance.GetAttrupRecord(info.GetAttrGroupId());
        ModuleMgr.BuffMgr.DelBuff(info.GetBuffID());
        if (attrdata == null)
        {
            attrdata = new UserAttrupDataBase();
            attrdata.attrGroupId = info.GetAttrGroupId();
            attrdata.level = 1;
            Profile.Instance.attrIdLv.Add(attrdata);
        }
        else
        {
            attrdata.level = attrdata.level + 1;
        }
        if (info.IsActor())
        {
            UserAttrupShowData netInfo = info.GetNextInfo();
            ModuleMgr.AchivementMgr.UpdateAchivement(Const.AchivementType.Attrup, new int[] { info.GetActionTarget(),info.GetAttrId(), netInfo.GetLevel() });
    }
        info.RefreshLevelData();
        ModuleMgr.BuffMgr.AddBuff(info.GetBuffID());
        Refresh();
    }
    

}
