using UnityEngine;

public class CustomerCtrl : ActorController
{
    private UserCustomerOrderData ctrldata;
    private float checkInterval = 0.3f;
    private float checkTimer = 0f;
    protected override void Awake()
    {
        base.Awake();
        actorType = Const.ActorType.Customer;
    }

    public void SetCtrlData(UserCustomerOrderData info)
    {
        ctrldata = info;
        this.name = "customer" + Getuid();
        agent.SetAgentSpeed(ctrldata.GetSpeedVal());
    }

    public int Getuid()
    {
        return ctrldata.uid;
    }
    protected override void Update()
    {
        base.Update();
        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInterval)
        {
            checkTimer = 0f;
            RushManager.Instance.RefreshFloating(this);
        }
    }
    public int GetFirstGetProduct()
    {
        //jietodo 区分优先级？
        foreach (int productId in GetCtrlData().needProductNum.Keys)
        {
            if (GetProductLeftNum(productId) > 0)
            {
                return productId;
            }
        }
        return -1;
    }

    public int GetProductLeftNum(int productId)
    {
        return GetCtrlData().GetNeedProductNum(productId) - Stack.GetNumById(productId);
    }

    public string GetFace()
    {
        //jietodo，根据结算情况设定表情
        return GetCtrlData().GetInfo().face[0];
    }
    public bool IsFinishOrder()
    {
        return GetFirstGetProduct() < 0;
    }

    public UserCustomerOrderData GetCtrlData()
    {
        return ctrldata;
    }

    public override void UpdateAttr()
    {
        base.UpdateAttr();
    }

}

