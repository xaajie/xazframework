//----------------------------------------------------------------------------
//-- view
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xaz;

public class UIMain : BaseUIMain
{
    Dictionary<Const.ActorStep, string> stateName = new Dictionary<Const.ActorStep, string>();
    public static string buildStateName = "machine";
    public static string buildShelfName = "order";
    private GameJoystick m_joystickctrl;
    private CameraController camera_ctrl;
    private bool joystickIng;
    //private int lastNoticeTime = 0;
    //private static int CheckNoticeINV = 2;
    protected override void OnOpened()
    {
        base.OnOpened();
        SetCamera();
        m_joystickctrl = m_Joystick.transform.GetComponent<GameJoystick>();
        m_joystickctrl.OnJoystickDrag += (dir,ps) => OnJoystickDrag(dir,ps);
        m_joystickctrl.OnJoystickEndDrag += OnJoystickEndDrag;
        m_joystickctrl.OnJoystickBeginDrag += OnJoystickBeginDrag;
        stateName.Add(Const.ActorStep.Cash,"cash");
        stateName.Add(Const.ActorStep.Get, "order");
        stateName.Add(Const.ActorStep.Exit, "face");
        stateName.Add(Const.ActorStep.None, "none");
        stateName.Add(Const.ActorStep.Sleep, "ad");
        stateName.Add(Const.ActorStep.CashNow, "cash");
        AddEventListener(EventEnum.UIMAIN_REFRESH, Refresh);
       // AddEventListener(EventEnum.CHANGE_CATEGORY, Refresh);
        AddEventListener(EventEnum.Cam_FINISH, SetCamera);
        AddEventListener(EventEnum.NOTICE_REFRESH, RefreshNotice);
        AddEventListener(EventEnum.UIAchivement_REFRESH, RefreshAchivement);
        AddEventListener(EventEnum.ChallengeInfo_REFRESH, RefreshChallenge);
        UIMgr.Open<UIMainBottom>();
#if UNITY_EDITOR     
        UIMgr.Open<UIGMEnter>();
#endif
        UserMainBtnData challen = ModuleMgr.MainMgr.GetMainBtnDataById((int)Const.MainViewID.map);
        Utils.SetActive(m_MapBN.gameObject,challen.CanShow());
        UserMainBtnData Achivementinfo = ModuleMgr.MainMgr.GetMainBtnDataById((int)Const.MainViewID.Achivement);
        Utils.SetActive(m_AchivementBN.gameObject, Achivementinfo.CanShow());
        List<UserMainBtnData> res = ModuleMgr.MainMgr.GetListByType((int)Const.MainShow.Bottom);
        m_BottomList.Clear(false);
        this.m_BottomList.AddDataList(res);
        Refresh();
        ModuleMgr.MainMgr.isInit = true;
        //lastNoticeTime = TimeUtil.GetNowInt();
        m_NoticeList.Clear(true);
        for (int i = 0; i < ModuleMgr.FightMgr.maxNoticeNum; i++)
        {
            m_NoticeList.AddData(i);
        }
        m_HappenList.Clear(true);
        for (int i = 0; i < ModuleMgr.HappeningMgr.maxNum; i++)
        {
            m_HappenList.AddData(i);
        }
        if (ModuleMgr.MainMgr.CheckShowOffLine())
        {
            UIMgr.Open<UIOfflineReward>();
        }
        scheduler.Interval(delegate ()
        {
            UpdateInv();
        }, 0.1f);
    }

    protected override void OnClosed()
    {
        base.OnClosed();
    }
    public void SetCamera()
    {
        camera_ctrl = CameraMgr.Instance.GetCamCtrl();
        m_Slider.SetValueWithoutNotify(camera_ctrl.GetInitFovSlider());
    }

    private void RefreshAchivement()
    {
        bool isRed = ModuleMgr.AchivementMgr.HasAchivementRed();
        Utils.SetActive(m_AchivementRed.gameObject, isRed);
    }
    override protected void OnValueChanged(Component com, object value)
    {
        base.OnValueChanged(com, value);
        if(com == m_Slider)
        {
            float val = float.Parse(value.ToString());
            camera_ctrl.SetFovSlider(val);
        }
    }

    private float lastSetTime = 0;
   private void UpdateInv()
    {
        m_Fixlist.Refresh();

        if (Time.time - lastSetTime>1 && RushManager.Instance!=null && RushManager.Instance.canUpdate)
        {
            lastSetTime = Time.time;
            ModuleMgr.HappeningMgr.CheckHappen();
            ModuleMgr.BuffMgr.CheckBuff();
            m_HappenList.Refresh();
        }
    }

    private void RefreshNotice()
    {
        //lastNoticeTime = TimeUtil.GetNowInt();
        ModuleMgr.FightMgr.CheckNotice();
        m_NoticeList.Refresh();
    }
    private void OnJoystickBeginDrag()
    {
        joystickIng = true;
        EventMgr.DispatchEvent(EventEnum.NOVICE_JOYSTICK_BEGINDRAG);
    }

    private void OnJoystickDrag(Vector3 dir,float dis)
    {
        //specCamera_ctrl.OnPanMove(-dirx, -diry);
        //SetHideContent(true);
        if (joystickIng)
        {
            RushManager.Instance.mainplayer.MoveByDir(dir);
        }
    }

    private void OnJoystickEndDrag()
    {
        joystickIng = false;
        RushManager.Instance.mainplayer.StopMove();
        EventMgr.DispatchEvent(EventEnum.NOVICE_JOYSTICK_ENDRAG);
    }
    public void Refresh()
    {
        RefreshChallenge();
        this.m_BottomList.Refresh();
        RefreshAchivement();
        // this.m_Toplist.Refresh();
        //UserChallengeShowData info = Profile.Instance.GetCanChallengeMaxInfo();
        //this.m_Info.text = string.Format(Utils.GetLang("challgenInfo"), info.GetChapterId(), info.GetID());

        //m_GoonBN.gameObject.SetActive(Const.needTurnResult && Profile.Instance.HasTurnRecord());
    }

    private void RefreshChallenge()
    {
        float prevt = m_Lvbar.value;
        UserChallengeShowData curChallengeInfo = ModuleMgr.ChallengeMgr.GetCurChallege();
        UserChallengeShowData nextInfo = curChallengeInfo.GetNextChallengeInfo();
        Utils.SetActive(m_ChallengeIcon.gameObject, nextInfo != null);
        if (nextInfo != null)
        {
            m_Lvbar.value = curChallengeInfo.GetBuildBarVal();
            m_ChallengeName.text = curChallengeInfo.GetName();
            m_ChallengeIcon.SetSprite(nextInfo.GetAtlas(), nextInfo.GetIcon());
        }
        else
        {
            m_Lvbar.value = 1;
        }
        if(prevt< m_Lvbar.value)
        {
            Utils.SetActive(m_Bareff.gameObject, false);
            Utils.SetActive(m_Bareff.gameObject, true);
        }
        m_LvTxt.text = string.Format("Lv.{0}", curChallengeInfo.GetLevel());
    }


    public void AddFloating(object tardata)
    {
        m_Fixlist.AddData(tardata);
    }

    public void RemoveFloating(object tardata)
    {
        m_Fixlist.RemoveData(tardata);
    }

    public void RefreshFloating(object tardata)
    {
        m_Fixlist.RefreshCell(tardata);
    }

    override protected void OnFixTableViewCellInit(UIFixTableView tableView, UIFixTableViewCell tablecell, object data)
    {
        base.OnFixTableViewCellInit(tableView, tablecell, data);
        if (tableView == m_Fixlist)
        {
            TV_Fixlist.Cell0 cell = this.GetCellView(tableView, tablecell) as TV_Fixlist.Cell0;
            TrackPositionFloating vt = tablecell.GetComponent<TrackPositionFloating>();
            if (data is ActorController)
            {
                ActorController dat = data as ActorController;
                if (vt != null && vt.displayer == null)
                {
                    vt.SetTrackTarget(dat.transform);
                }
                cell.State.SetState(stateName[dat.step]);
                if (dat.GetActorType() == Const.ActorType.Customer)
                {
                    CustomerCtrl tar = dat as CustomerCtrl;
                    if (dat.step == Const.ActorStep.Get)
                    {
                        if (dat.taskproductId> 0){
                            UserCategoryData product = tar.GetCtrlData().GetCategory(dat.taskproductId);
                            cell.Icon.SetSprite(product.GetAtlas(), product.GetIcon());
                            cell.Iconnum.text = string.Format("{0}/{1}", tar.Stack.GetNumById(dat.taskproductId), product.GetNum());
                        }
                    }
                    else if(dat.step== Const.ActorStep.Exit)
                    {
                        cell.FaceIcon.SetSprite(Const.AtlasFace, tar.GetFace());
                    }
                }
                else if (dat.GetActorType() == Const.ActorType.Guard)
                {
                    WorkerCtrl tar2 = dat as WorkerCtrl;
                    cell.Adcd.fillAmount = tar2.GetWakeupBarVal();
                }
            }else if(data is BuildController)
            {
                BuildController  build = data as BuildController;
                if(build.GetCtrlData().GetBuildType() == (int)Const.BuildType.Shelf)
                {
                    if (vt != null && vt.displayer == null)
                    {
                        vt.SetTrackTarget(build.transform);
                    }
                    bool isFull = build.productStack.Count >= build.productStack.MaxStack;
                    if (isFull)
                    {
                        cell.State.SetState("none");
                    }
                    else
                    {
                        cell.State.SetState(buildShelfName);
                        UserCategoryData product = build.GetCtrlData().GetCategory(build.productStack.productId);
                        cell.Icon.SetSprite(product.GetAtlas(), product.GetIcon());
                        cell.Iconnum.text = string.Format("{0}/{1}", build.productStack.Count, build.productStack.MaxStack);
                    }
                }
                else
                {
                    MachineBuild dat2 = data as MachineBuild;
                    if (dat2.rawPiles[0].productId <= 0)
                    {
                        return;
                    }
                    if (vt != null && vt.displayer == null)
                    {
                        UIImage[] rawicon = new UIImage[] { cell.Rawicon1, cell.Rawicon2 };
                        Transform[] nodes = new Transform[] { cell.Raw1, cell.Raw2 };
                        vt.SetTrackTarget(dat2.transform);
                        cell.State.SetState(buildStateName);
                        for (int i = 0; i < nodes.Length; i++)
                        {
                            nodes[i].gameObject.SetActive(i < dat2.rawPiles.Count);
                            if (i < dat2.rawPiles.Count && dat2.rawPiles[i].productId > 0)
                            {
                                //nodes[i].transform.localPosition = vt.GetRectPos(dat2.rawPiles[i].transform, nodes[i].transform.parent.GetComponent<RectTransform>());
                                UserCategoryData product = dat2.GetCtrlData().GetCategory(dat2.rawPiles[i].productId);
                                rawicon[i].SetSprite(product.GetAtlas(), product.GetIcon());
                            }
                        }
                    }
                    Text[] rawtxt = new Text[] { cell.Rawnum1, cell.Rawnum2 };
                    for (int i = 0; i < 2; i++)
                    {
                        if (i < dat2.rawPiles.Count)
                        {
                            rawtxt[i].text = string.Format("{0}/{1}", dat2.rawPiles[i].Count, dat2.rawPiles[i].MaxStack);
                        }
                        cell.Cd1.fillAmount = dat2.GetCtrlData().GetShowProductCd();
                        cell.Cd2.fillAmount = dat2.GetCtrlData().GetShowProductCd();
                    }
                }
            }
        }
    }

    override protected void OnTableViewCellInit(UITableView tableView, UITableViewCell tableCell, object data)
    {
        base.OnTableViewCellInit(tableView, tableCell, data);
        if (tableView == m_NoticeList)
        {
            UserNoticeData dat = ModuleMgr.FightMgr.GetShowNotice((int)data);
            TV_NoticeList.Cell0 cell = this.GetCellView(tableView, tableCell) as TV_NoticeList.Cell0;
            cell.Icon.SetSprite(dat.GetAtlas(), dat.GetIcon());
            Utils.SetActive(cell.Content.gameObject, dat.GetNoticeType() != Const.NoticeType.None);
        }
        else if (tableView == m_HappenList)
        {
            UserHappeningData dat = ModuleMgr.HappeningMgr.GetShowHappens((int)data);
            TV_HappenList.Cell0 cell = this.GetCellView(tableView, tableCell) as TV_HappenList.Cell0;
            if (dat != null)
            {
                cell.Icon.SetSprite(dat.GetAtlas(), dat.GetIcon());
                Utils.SetActive(cell.Content.gameObject, dat.CheckTimeValid());
                if (dat.IsEffectNow())
                {
                    cell.Time.text = TimeUtil.GetTimeFormatStr(dat.GetBuffLeftTime(), "mm:ss");
                    cell.UIState.SetState("1");
                }
                else
                {
                    cell.Time.text = TimeUtil.GetTimeFormatStr(dat.GetLeftTime(), "mm:ss");
                    cell.UIState.SetState("0");
                }
            }
            else
            {
                Utils.SetActive(cell.Content.gameObject, false);
            }
        }
        else if (tableView == m_BottomList)
        {
            UserMainBtnData dat = data as UserMainBtnData;
            TV_BottomList.Cell0 cell = this.GetCellView(tableView, tableCell) as TV_BottomList.Cell0;
            cell.Icon.SetSprite(dat.GetAtlas(), dat.GetIcon());
            cell.Name.text = dat.GetName();
        }
    }

    override protected void OnTableViewCellClick(UITableView tableView, UITableViewCell tableCell, GameObject target, object data)
    {
        base.OnTableViewCellClick(tableView, tableCell, target, data);
        if (tableView == m_BottomList)
        {
            UserMainBtnData dat = data as UserMainBtnData;
            dat.OpenView();
        }
        else if (tableView == m_NoticeList)
        {
            UserNoticeData dat = ModuleMgr.FightMgr.GetShowNotice((int)data);
            m_Ani.StartCoroutine(NoticeClick(dat));
        }
        else if (tableView == m_HappenList)
        {
            UserHappeningData dat = ModuleMgr.HappeningMgr.GetShowHappens((int)data);
            if (dat != null)
            {
                if (dat.IsEffectNow())
                {
                    UIMgr.ShowFlyTip(string.Format(Utils.GetLang("happen_tipbuff"),dat.GetName()));
                    return;
                }
                UIMgr.Open<UIHappen>(uiview => uiview.SetData(dat));
            }
        }
    }

    IEnumerator NoticeClick(UserNoticeData info)
    {
        UIMgr.Open<UIMask>();
        Transform tar = info.GetClickActionTarget();
        CameraMgr.Instance.SetFollowCam(CameraController.CameraMode.FOUCUS, tar.transform,false);
        yield return new WaitForSeconds(Const.CamerFocusTime);
        UIMgr.Close<UIMask>();
        CameraMgr.Instance.ResetCam();
    }
    protected override void OnButtonClick(Component com)
    {
        base.OnButtonClick(com);
        if(com == m_MapBN)
        {
            UIMgr.Open<UIChallenge>();
        }
        else if(com == m_AchivementBN)
        {
            UIMgr.Open<UIAchivement>();
        }
    }
    public Transform GetJoystickBN()
    {
        return this.m_Joystick.transform;
    }
    public Transform GetNoviceBuildUp(int inx = 0)
    {
        return m_BottomList.GetCellAt(inx).transform;
    }
}
