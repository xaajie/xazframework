using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{

    [Name("选定角色")]
    [Category("动作节点")]
    public class SelectActor : ActionTask<ActorController>
    {
        public Const.ActorSumType actorSumType;
        public Const.ActorStep actorState;
        protected override string info
        {
            get { return "选定 " + actorState.ToString() + "角色"; }
        }

        protected override void OnExecute()
        {
            ActorController info=null;

            if (actorSumType == Const.ActorSumType.Worker)
            {
                foreach (WorkerCtrl actor in RushManager.Instance.workers)
                {
                    if (actor.step == actorState)
                    {
                        info = actor;
                        this.blackboard.SetVariableValue("actorId", actor.Getuid());
                        break;
                    }
                }
            }
            else if(actorSumType == Const.ActorSumType.Customer)
            {
                foreach (CustomerCtrl actor in RushManager.Instance.customers)
                {
                    if (actor.step == actorState)
                    {
                        info = actor;
                        this.blackboard.SetVariableValue("actorId", actor.Getuid());
                        break;
                    }
                }
            }
            if (info != null)
            {
                this.blackboard.SetVariableValue("preVector", info.transform.position);
                EndAction(true);
            }
            else
            {
                EndAction(false);
            }
        }

    }
}