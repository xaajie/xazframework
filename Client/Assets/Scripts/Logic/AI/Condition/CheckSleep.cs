using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Conditions
{

    [Name("是否需要睡觉")]
    [Category("条件节点")]
    public class CheckSleep : ConditionTask<WorkerCtrl>
    {

        protected override bool OnCheck() {
            agent.CheckNeedSleep();
            return agent.GetCtrlData().IsSleep();
        }
    }
}