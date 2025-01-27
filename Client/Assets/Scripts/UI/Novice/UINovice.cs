//----------------------------------------------------------------------------
//-- view
//-- <write your instructions here>
//-- @author xiejie
//----------------------------------------------------------------------------
using DG.Tweening;
using UnityEngine;
using Xaz;

public class UINovice : BaseUINovice
{
    private RectTransform boundsRect;
    private Camera uiCam;
    private Vector3 initTippos;
    private UserNoviceData noviceInfo;
    private int timetask;
    private bool needUpdate = false;
    private long startTime = 0;
    private Vector3 displayOffset = new Vector3(0f, 4f, 0f);
    //private HollowOutMask handmaskhollow;
    protected override void OnOpened()
    {
        base.OnOpened();
        //Profile.Instance.user.noviceId = 10004;
        
        initTippos = m_Tipbg.transform.localPosition;
        boundsRect = gameObject.GetComponent<RectTransform>();
        //handmaskhollow = m_HandMask.GetComponent<HollowOutMask>(); 
        scheduler.Update(() =>
        {
            UpdateInv();
        });
        uiCam = CameraMgr.Instance.GetUICam();
        ModuleMgr.NoviceMgr.InitCurrNoviceId();
        Utils.SetActive(m_Hand.gameObject, false);
        Utils.SetActive(m_HandMask.gameObject, false);
        Utils.SetActive(m_OkSpt.gameObject, false);
        Utils.SetActive(m_FirtTip.gameObject, false);
        Utils.SetActive(m_Arrows.gameObject, false);
        Utils.SetActive(m_Moveeff.gameObject, false);
        Utils.SetActive(m_Tipbg.gameObject, false);
        InitNovice();
        CheckNotice();
    }
    private void InitNovice()
    {
        noviceInfo = new UserNoviceData(Profile.Instance.user.noviceId);
        if (noviceInfo.GetPreDelay() > 0)
        {
            startTime = TimeUtil.GetNowTicks();
        }
        else
        {
            startTime = 0;
        }
        
    }
    Vector2 localPoint;
    private void UpdateInv()
    {
        if (needUpdate)
        {
            CheckNotice();
            return;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (m_Hand.gameObject.activeSelf && RectTransformUtility.ScreenPointToLocalPointInRectangle(m_HandBN.rectTransform, Input.mousePosition, uiCam, out localPoint))
            {
                Vector2 sizeDelta = m_HandBN.rectTransform.sizeDelta;
                Vector2 oneSize = new Vector2(sizeDelta.x / 2, sizeDelta.y / 2);

                if (localPoint.x >= -oneSize.x && localPoint.x <= oneSize.x &&
                    localPoint.y >= -oneSize.y && localPoint.y <= oneSize.y)
                {
                   // Logger.Print("---GetMouseButtonUp(0)---" + noviceInfo.GetID());
                    FinishNovice();
                }
            }
        }
    }

    Vector3 ClampToBounds(Vector3 localPos, Vector2 elementSize)
    {
        Vector3 position = boundsRect.InverseTransformPoint(localPos);
        Rect bounds = boundsRect.rect;
        float clampedX = Mathf.Clamp(position.x, bounds.xMin + elementSize.x / 2f, bounds.xMax - elementSize.x / 2f);
        float clampedY = Mathf.Clamp(position.y, bounds.yMin + elementSize.y / 2f, bounds.yMax - elementSize.y / 2f);
        return new Vector3(clampedX, clampedY, position.z);
    }
    private void HideNovice(bool isHide)
    {
        UIMgr.SetHideView<UINovice>(isHide);
    }
    private bool CheckNoviceHide()
    {
        if (noviceInfo.GetPreDelay() > 0)
        {
            if ((TimeUtil.GetNowTicks() - startTime) <= noviceInfo.GetPreDelay())
            {
                return true;
            }
        }
        if (noviceInfo.GetNoviceType() !=(int) NoviceConst.NoviceType.UI)
        {
            if (!UIMgr.IsFocused<UIMain>())
            {
                return true;
            }
        }
        return false;
    }
    private void CheckNotice()
    {
        if (CheckNoviceHide())
        {
            HideNovice(true);
            needUpdate = true;
            return;
        }
        else
        {
            HideNovice(false);
        }
        bool showNotice = false;
       // Logger.Print(" Profile.Instance.user.noviceId====", Profile.Instance.user.noviceId);
        Utils.SetActive(m_Hand.gameObject, false);
        Utils.SetActive(m_HandMask.gameObject, false);
        Utils.SetActive(m_OkSpt.gameObject, false);
        Utils.SetActive(m_Arrows.gameObject, false);
        Utils.SetActive(m_Moveeff.gameObject, false);
        if (noviceInfo.GetID() == (int)NoviceConst.NoviceID.OpenJoystick)
        {
            Utils.SetActive(m_Hand.gameObject, true);
            Utils.SetActive(m_Moveeff.gameObject, true);
            Utils.SetActive(m_Handeff.gameObject, false);
            Vector3 pos = m_Moveeff.transform.position;
            pos.x -= 15f;
            m_Moveeff.transform.position = pos;
            pos.x += 30f;
            showNotice = true;
            m_Moveeff.transform.DOMove(pos, 1).SetLoops(-1, LoopType.Yoyo);
            AddEventListener(EventEnum.NOVICE_JOYSTICK_BEGINDRAG, NoviceStartTimer);
            AddEventListener(EventEnum.NOVICE_JOYSTICK_ENDRAG, NoviceEndTimer);
        }
        else if (noviceInfo.GetNoviceType() == (int)NoviceConst.NoviceType.UnLock)
        {
            showNotice = CheckLandTarget(noviceInfo.GetUnlockID());
        }
        else if (noviceInfo.GetNoviceType() == (int)NoviceConst.NoviceType.Building)
        {
            showNotice = CheckBuildTarget(noviceInfo.GetPosID());
        }
        else if (noviceInfo.GetID() == (int)NoviceConst.NoviceID.UpAppleTree1) 
        {
            UIMain view = UIMgr.Get<UIMain>();
            if (view != null)
            {
                //界面异步必须要有
                needUpdate = false;
                showNotice = true;
                Utils.SetActive(m_Hand.gameObject, true);
                Utils.SetActive(m_HandMask.gameObject, true);
                CheckClickTarget(view.GetNoviceBuildUp(1));
            }
        }else if (noviceInfo.GetID() == (int)NoviceConst.NoviceID.UpAppleTree2) 
        {
            UIBuilding view = UIMgr.Get<UIBuilding>();
            if (view != null&& view.GetNoviceBuildUp(0) != null)
            {
                showNotice = true;
                //界面异步必须要有
                needUpdate = false;
                Utils.SetActive(m_Hand.gameObject, true);
                Utils.SetActive(m_HandMask.gameObject, true);
                CheckClickTarget(view.GetNoviceBuildUp(0));
            }
            else
            {
                needUpdate = true;
            }
        }
        if (showNotice && noviceInfo.GetDesc() != string.Empty)
        {
            Utils.SetActive(m_Tipbg.gameObject, true);
            m_NoviceTxt.text = Utils.GetLang(noviceInfo.GetDesc());
            if (noviceInfo.GetNoviceType() == (int)NoviceConst.NoviceType.UI)
            {
                TrackPositionFloating vt = m_Tipbg.transform.GetComponent<TrackPositionFloating>();
                if (vt != null)
                {
                    vt.SetTrackTarget(RushManager.Instance.mainplayer.transform, displayOffset);
                }
            }
            
        }
        else
        {
            Utils.SetActive(m_Tipbg.gameObject, false);
        }
    }
    private void ClearTimer()
    {
        if (timetask > 0)
        {
            scheduler.Remove(ref timetask);
        }
    }
    private void FinishJoy()
    {
        ClearTimer();
        RemoveEventListener(EventEnum.NOVICE_JOYSTICK_BEGINDRAG, NoviceStartTimer);
        RemoveEventListener(EventEnum.NOVICE_JOYSTICK_ENDRAG, NoviceEndTimer);
        FinishNovice();
    }
    private void NoviceStartTimer()
    {
        ClearTimer();
        timetask = scheduler.Timeout(() =>
        {
            FinishJoy();
        }, noviceInfo.GetFinishTime());
    }
    private void NoviceEndTimer()
    {
        FinishJoy();
    }
    
    private void FinishNovice()
    {
        needUpdate = false;
        spcState = false;
        if (noviceInfo.GetID() == (int)NoviceConst.NoviceID.OpenJoystick)
        {
            Utils.SetActive(m_Handeff.gameObject, true);
        }
        if (noviceInfo.IsLast())
        {
            Profile.Instance.user.noviceId = -1;
            UIMgr.Close<UINovice>();
        }
        else
        {
            Profile.Instance.user.noviceId = noviceInfo.GetNextId()>0? noviceInfo.GetNextId():-1;
            InitNovice();
            CheckNotice();
        }
    }

    private void CheckClickTarget(Transform target)
    {
        RectTransform tartt = target.GetComponent<RectTransform>();
        m_Hand.transform.position = target.position;
        m_HandBN.rectTransform.sizeDelta = tartt.sizeDelta;
        m_Tipbg.transform.localPosition = initTippos;
        Vector3 clampedPos = ClampToBounds(m_Tipbg.transform.position,m_Tipbg.rectTransform.sizeDelta);
        m_Tipbg.transform.position = boundsRect.TransformPoint(clampedPos);
        //if (m_Tipbg.gameObject.activeSelf)
        //{
        //    edgest.RefreshEdge(5, this.transform);
        //}
    }

    private bool CheckLandTarget(int unlockid)
    {
        needUpdate = true;
        bool showNotice = false;
        BuyLandController unlock = RushManager.Instance.GetUnlockById(unlockid);
        if (unlock != null)
        {
            showNotice = ShowLandArrow(unlock.transform);
        }
        else
        {
            if (needUpdate)
            {
                //解锁成功
                FinishNovice();
            }
        }
        return showNotice;
    }
    private bool spcState = false;
    private bool CheckBuildTarget(int buildId)
    {
        needUpdate = true;
        bool showNotice = false;
        BuildController build = RushManager.Instance.GetBuildById(buildId);
        if (build != null)
        {
            showNotice = true;
            if ((noviceInfo.GetID() == (int)NoviceConst.NoviceID.AddAppleTree || noviceInfo.GetID() == (int)NoviceConst.NoviceID.PdAppJiang1 
                || noviceInfo.GetID() == (int)NoviceConst.NoviceID.AddAppJiang1) && 
                (RushManager.Instance.mainplayer.Stack.GetNumById(build.GetCtrlData().GetProductId()) > 0))
            {
                FinishNovice();
            }
            else if ((noviceInfo.GetID() == (int)NoviceConst.NoviceID.AddAppleGui || noviceInfo.GetID() == (int)NoviceConst.NoviceID.AddAppJiang2) &&
                (RushManager.Instance.mainplayer.Stack.GetNumById(build.GetCtrlData().GetProductId()) <= 0))
            {
                FinishNovice();
            }
            else if (noviceInfo.GetID() == (int)NoviceConst.NoviceID.GetGash && spcState && (!(build as CashBuild).HasMoney()))
            {
                FinishNovice();
            }
            else if (noviceInfo.GetID() == (int)NoviceConst.NoviceID.PdAppJiang2 && spcState)
            {
                FinishNovice();
            }
            else if (noviceInfo.GetID() == (int)NoviceConst.NoviceID.RepairAppJiang && spcState && !build.GetCtrlData().IsBroken())
            {
                FinishNovice();
            }
            else
            {
                if (noviceInfo.GetID() == (int)NoviceConst.NoviceID.GetGash)
                {
                    if((build as CashBuild).HasMoney())
                    {
                        spcState = true;
                    }
                }
                else if(build.GetCtrlData().GetBuildType() == (int)Const.BuildType.Machine)
                {
                    MachineBuild mbuild = build as MachineBuild;
                    if (noviceInfo.GetID() == (int)NoviceConst.NoviceID.PdAppJiang2)
                    {
                        for (int i = 0; i < mbuild.rawPiles.Count; i++)
                        {
                            if (mbuild.rawPiles[i].Count > 0)
                            {
                                spcState = true;
                                break;
                            }
                        }
                    }
                    else if (noviceInfo.GetID() == (int)NoviceConst.NoviceID.RepairAppJiang)
                    {
                        if(mbuild.GetCtrlData().IsBroken())
                        {
                            spcState = true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                showNotice = ShowLandArrow(build.transform);
            }
        }
        return showNotice;
    }
    private bool ShowLandArrow(Transform target)
    {
        Utils.SetActive(m_Arrows.gameObject, true);
        if (Vector3.Distance(target.position, RushManager.Instance.mainplayer.transform.position) >= (Constant.Novice_Arrow_Distance)/100)
        {
            Utils.SetActive(RushManager.Instance.mainplayer.landArrow.gameObject, true);
            Utils.SetActive(m_PosArrow.gameObject, false);
            // 计算从箭头到目标点的方向
            Vector3 direction = target.position - RushManager.Instance.mainplayer.transform.position;

            // 使箭头朝向目标点，并保持与地面平行（忽略y轴的方向）
            direction.y = 0;  // 设置y为0，避免箭头上下倾斜

            // 如果目标点和箭头不在同一位置，进行旋转
            if (direction.magnitude > 0.1f)
            {
                // 计算箭头需要旋转的角度
                float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                RushManager.Instance.mainplayer.landArrow.rotation = Quaternion.Euler(90f, 0f, -angle + 180);
            }
            return false;
        }
        else
        {
            Utils.SetActive(m_PosArrow.gameObject, true);
            Utils.SetActive(RushManager.Instance.mainplayer.landArrow.gameObject, false);
            TrackPositionFloating vt = m_Arrows.transform.GetComponent<TrackPositionFloating>();
            if (vt != null)
            {
                vt.SetTrackTarget(target);
            }
            TrackPositionFloating vt2 = m_Tipbg.transform.GetComponent<TrackPositionFloating>();
            if (vt2 != null)
            {
                vt2.SetTrackTarget(target, displayOffset);
            }
            return true;
        }
    }
    protected override void OnButtonClick(Component com)
    {
        base.OnButtonClick(com);
        if (com == this.m_OkBN)
        {
            CheckNotice();
        }
    }
}
