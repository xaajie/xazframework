using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions
{

    [Name("是否完成订单")]
    [Category("条件节点")]
    public class CheckFinOrder : ConditionTask<CustomerCtrl>
    {

        protected override bool OnCheck() {
            return agent.IsFinishOrder();
        }

        //public override void OnDrawGizmosSelected() {
        //    if ( agent != null ) {
        //        Gizmos.DrawWireSphere(agent.position, distance.value);
        //    }
        //}
    }
}