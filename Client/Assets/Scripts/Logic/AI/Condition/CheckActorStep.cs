using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Conditions
{

    [Name("检测角色状态")]
    [Category("条件节点")]
    public class CheckActorStep : ConditionTask<ActorController>
    {
        public Const.ActorStep atype;

        protected override string info
        {
            get { return atype.ToString(); }
        }
        protected override bool OnCheck() {

            return agent.step == atype;
        }
    }
}