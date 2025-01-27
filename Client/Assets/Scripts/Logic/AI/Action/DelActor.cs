using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{

    [Name("删除角色")]
    [Category("动作节点")]
    public class DelActor : ActionTask<ActorController>
    {

        protected override void OnExecute() {
            if(agent.GetActorType() == Const.ActorType.Customer)
            {
                RushManager.Instance.DelCustomer(agent as CustomerCtrl);
            }else
            {
                RushManager.Instance.DelWorker(agent as WorkerCtrl);
            }
            EndAction(true);
        }

        //protected override void OnUpdate() {

        //}

        //protected override void OnPause() { OnStop(); }
        //protected override void OnStop() {
        //    //if ( lastRequest != null && agent.gameObject.activeSelf ) {
        //    //    agent.ResetPath();
        //    //}
        //    //lastRequest = null;
        //}
    }
}