using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Conditions
{

    [Name("是否需要睡觉")]
    [Category("条件节点")]
    public class CheckSetSleep : ConditionTask<WorkerCtrl>
    {

        protected override bool OnCheck() {
            bool preflag = agent.GetCtrlData().IsSleep();
            agent.CheckNeedSleep();
            bool needsleep = agent.GetCtrlData().IsSleep();
            //需要变化为睡觉
            if(needsleep && !preflag)
            {
                //工人
                if (agent.IsProductWorker())
                {
                    this.blackboard.SetVariableValue("preVector", (agent as WorkerCtrl).GetSleepPoint());
                }
                else
                {
                    //保安
                    this.blackboard.SetVariableValue("preVector", RushManager.Instance.GetGuard().GetBindPoint());
                }
            }
            return needsleep;
        }
    }
}