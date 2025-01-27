using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PoolManager;

public class MachineBuild : BuildController
{
    private float checkInv = 0.7f;
    private float checkTimer = 0f;
    [SerializeField] public List<BaseStack> rawPiles;
    [SerializeField] private FixBin fixcollider;
    protected override void UpdateAttr()
    {
        base.UpdateAttr();
        int inx = 0;
        foreach (int pid in GetCtrlData().GetInfo().rawProductInfo.Keys)
        {
            rawPiles[inx].productId = pid;
            rawPiles[inx].MaxStack = GetCtrlData().GetInfo().GetRawCapacityById(pid);
            inx++;
        }
        fixcollider.SetOwnID(this.GetCtrlData().uid);
    }

    public bool CheckCanProduct()
    {
        for (int i = 0; i < rawPiles.Count; i++)
        {
            if (rawPiles[i].productId <= 0 || rawPiles[i].Count < GetCtrlData().GetInfo().GetRawNeedById(rawPiles[i].productId))
            {
                return false;
            }
        }
        return true;
    }

    public int GetRawMaxNum(int rawId)
    {
        for (int i = 0; i < rawPiles.Count; i++)
        {
            if (rawPiles[i].productId == rawId)
            {
                return rawPiles[i].MaxStack;
            }
        }
        return 0;
    }

    /// <summary>
    /// 原产物个数
    /// </summary>
    /// <param name="parm"></param>
    /// <returns></returns>
    public int GetRawLeftNum(int rawId)
    {
        for (int i = 0; i < rawPiles.Count; i++)
        {
            if (rawPiles[i].productId == rawId)
            {
                return rawPiles[i].MaxStack - rawPiles[i].Count;
            }
        }
        return 0;
    }

    //不具备生产条件
    public bool IsRawEmpty()
    {
        bool vt = false;
        for (int i = 0; i < rawPiles.Count; i++)
        {
            if (rawPiles[i].Count <= 0)
            {
                vt = true;
            }
        }
        return vt;
    }

    public void AddRawStack(ActorController toactor, int rawId)
    {
        for (int i = 0; i < rawPiles.Count; i++)
        {
            if (rawPiles[i].productId == rawId)
            {
                if (rawPiles[i].Count < rawPiles[i].MaxStack)
                {
                    var objToStack = toactor.Stack.RemoveFromStack(rawId);
                    if (objToStack == null) return;

                    rawPiles[i].AddToStack(objToStack.gameObject, true);
                }
                break;
            }
        }
    }

    public void RemoveRawStack(ActorController toactor, int rawId)
    {
        for (int i = 0; i < rawPiles.Count; i++)
        {
            if (rawPiles[i].productId == rawId)
            {
                if (rawPiles[i].Count > 0)
                {
                    var removedObj = rawPiles[i].RemoveFromStack();
                    toactor.AddHandStack(removedObj.transform, rawPiles[i].productId);
                }
                break;
            }
        }
    }
    private bool isFIxing = false;
    public void FixBuildAction(bool frommaplyer)
    {
        if (GetCtrlData().IsBroken() && !isFIxing)
        {
            isFIxing = true;
            StartCoroutine(FixStepAction(frommaplyer));
        }
    }
    private IEnumerator FixStepAction(bool frommaplyer)
    {
        if (frommaplyer)
        {
            AudioMgr.Instance.Play(AudioEnum.Fixmachine);
        }
        SetSpineAnimState(SpineAnimCtrl.SpineAnimState.build_fix);
        yield return new WaitForSeconds(Const.FixWakeupDuration);
        if (frommaplyer)
        {
            AudioMgr.Instance.Play(AudioEnum.Fixmachine);
            yield return new WaitForSeconds(0.2f);
            AudioMgr.Instance.Play(AudioEnum.Wakeup);
        }
        GetCtrlData().SetFixBuildData();
        SetWorkSpine();
        isFIxing = false;
        EventMgr.DispatchEvent(EventEnum.NOTICE_REFRESH);
    }

    private void SetWorkSpine()
    {
        if (CheckCanProduct())
        {
            SetSpineAnimState(SpineAnimCtrl.SpineAnimState.build_work);
        }
        else
        {
            SetSpineAnimState(SpineAnimCtrl.SpineAnimState.build_idle);
        }
    }

    void Update()
    {
        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInv)
        {
            checkTimer = 0f;
            if (productStack.Count >= productStack.MaxStack) return;
            GetCtrlData().CheckSetBroken();
            if (CheckCanProduct() && GetCtrlData().FinishProduct())
            {
                for (int i = 0; i < rawPiles.Count; i++)
                {
                    for (int j = 0; j < GetCtrlData().GetInfo().GetRawNeedById(rawPiles[i].productId); j++)
                    {
                        var objToStack = rawPiles[i].RemoveFromStack();
                        if (objToStack != null)
                        {
                            PoolManager.Instance.ReturnObject(objToStack.gameObject);
                        }
                    }
                }
                GameObject nt = PoolManager.Instance.SpawnObject(PoolEnum.Product, GetCtrlData().GetProductId());
                productStack.AddToStack(nt.gameObject, false);
                GetCtrlData().RefreshStartProducTime(CheckCanProduct());
            }

            if (!isFIxing)
            {
                if (GetCtrlData().IsBroken())
                {
                    SetSpineAnimState(SpineAnimCtrl.SpineAnimState.build_broken);
                }
                else
                {
                    SetWorkSpine();
                }
            }

        }
    }
}

