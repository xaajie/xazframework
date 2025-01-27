using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using static PoolManager;

public class CashBuild : BuildController
{
    [SerializeField] private MoneyPile moneyPile;
    [SerializeField] private GameObject daiziObj;
    [SerializeField] private Transform throwPoint;
    private List<CustomerCtrl> cashCustomers = new List<CustomerCtrl>();
    private float checkInv = 0.5f;
    private float checkTimer = 0f;

    protected override void Awake()
    {
        base.Awake();
        daiziObj.SetActive(false);
    }
    protected override void UpdateAttr()
    {
        base.UpdateAttr();
        moneyPile.exchangeTableMoney = ModuleMgr.ChallengeMgr.GetCurChallege().GetPerMoney();
    }
    ActorController cashworker;
    private CustomerCtrl curCashCustomer=null;
    void Update()
    {
        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInv)
        {
            checkTimer = 0f;
            if (cashCustomers.Count > 0)
            {
                for (int i = 0; i < cashCustomers.Count; i++)
                {
                    Vector3 tPs = GetQueuePos(i);
                    cashCustomers[i].MoveTo(tPs);
                    if (Vector3.Distance(tPs, cashCustomers[i].transform.position) < 0.1f)
                    {
                        if (i == 0 && curCashCustomer==null)
                        {
                            curCashCustomer = cashCustomers[0];
                        }
                        cashCustomers[i].StopMove();
                    }
                }
            }
            if (curCashCustomer == null)
            {
                return;
            }
            if (cashworker == null)
            {
                IsHasCashWorker(out cashworker);
            }
            if (cashworker!=null)
            {
                if (curCashCustomer.Stack.Count > 0)
                {
                    ShowWorkAction(true);
                    var thrownObj = curCashCustomer.Stack.RemoveFromStack(-1);
                    AudioMgr.Instance.Play(AudioEnum.CashPackage);
                    thrownObj.DOJump(throwPoint.transform.position, 5f, 1, checkInv)
                        .OnComplete(() =>
                        {
                            DoTweenUtil.DoReplayComponent(daiziObj.transform);
                            PoolManager.Instance.ReturnObject(thrownObj.gameObject);
                        });
                }
                else
                {
                    int sellprice = curCashCustomer.GetCtrlData().GetOrderCashPrice();
                    CollectPayment(sellprice);
                    AudioMgr.Instance.Play(AudioEnum.Cash);
                    GameObject nt = PoolManager.Instance.SpawnObject(PoolEnum.Package);
                    curCashCustomer.AddHandStack(nt.transform, PoolEnumUid[(int)PoolEnum.Package],false);
                    cashCustomers.RemoveAt(0);
                    ModuleMgr.AchivementMgr.UpdateAchivement(Const.AchivementType.FinishOrder);
                    curCashCustomer = null;
                    if (cashCustomers.Count == 0)
                    {
                        ShowWorkAction(false);
                    }
                    //else
                    //{
                    //    curCashCustomer = cashCustomers[0];
                    //}
                }
            }
        }
    }

    public int GetqueueInx(int uid)
    {
        for (int i = 0; i < cashCustomers.Count; i++)
        {
            if(cashCustomers[i].Getuid() == uid)
            {
                return i;
            }
        }
        return -1;
    }
    public void AddCashCustomer(CustomerCtrl actor)
    {
        if (cashworker == null)
        {
            IsHasCashWorker(out cashworker);
        }
        cashCustomers.Add(actor);
        //if (curCashCustomer == null)
        //{
        //    curCashCustomer = cashCustomers[0];
        //}
    }

    public Vector3 GetQueuePos(int inx)
    {
        return GetRawStandPosOne() + Vector3.forward * inx * 0.5f;
    }
    public override Vector3 GetStandPoint()
    {
        return GetQueuePos(cashCustomers.Count - 1);
    }
    public bool IsHasCashWorker(out ActorController act)
    {
        act = null;
        for (int i = 0; i < RushManager.Instance.workers.Count; i++)
        {
            if (RushManager.Instance.workers[i].GetActorType() == Const.ActorType.CashWorker)
            {
                act = RushManager.Instance.workers[i];
                return true;
            }
        }
        return false;
    }

    public void ShowWorkAction(bool iswork)
    {
        if(iswork)
        {
            cashworker.SetTargetAnim(SpineAnimCtrl.SpineAnimState.cashwork);
            SetSpineAnimState(SpineAnimCtrl.SpineAnimState.build_work);
            if (!daiziObj.gameObject.activeSelf)
            {
                daiziObj.SetActive(true);
            }
        }
        else
        {
            cashworker.SetTargetAnim(SpineAnimCtrl.SpineAnimState.Idle);
            SetSpineAnimState(SpineAnimCtrl.SpineAnimState.build_idle);
            daiziObj.SetActive(false);
        }
    }

    public void CollectPayment(int sellprice)
    {
        moneyPile.AddChangeShowMoney(sellprice);
    }

    public bool HasMoney()
    {
        return moneyPile.Count > 0;
    }
}

