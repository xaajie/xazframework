using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions
{

    [Name("结算节点")]
    [Category("动作节点")]
    public class OrderCash : ActionTask<CustomerCtrl>
    {

        CashBuild cashBuild;
        protected override void OnExecute()
        {
            cashBuild = RushManager.Instance.GetCashBuild();
            cashBuild.AddCashCustomer(agent);
            //int index = cashBuild.GetqueueInx(agent.Getuid());
            //Vector3 tPs = cashBuild.GetQueuePos(index);
            //agent.MoveTo(tPs);
        }

        float dropTimer = 0f;
        float dropInterval = 1f;
        protected override void OnUpdate()
        {

            dropTimer += Time.deltaTime;
            if (dropTimer >= dropInterval)
            {
                dropTimer = 0f;
                if (agent.Stack.IsHandHasPackage())
                {
                    EndAction(true);
                }
            }
        }
    }
}