using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{

    [Name("取货")]
    [Category("动作节点")]
    public class WaitProductId : ActionTask<ActorController>
    {
        [BlackboardOnly]
        public BBParameter<int> nearBuild;

        [BlackboardOnly]
        public BBParameter<int> nextBuild;

        float dropTimer = 0f;
        float dropInterval = Const.InvFlyTime;
        protected override string info
        {
            get { return "取建筑 " + nearBuild.ToString() + "产物到"+nextBuild.ToString(); }
        }

        BuildController target;
        int productId;
        float sttime;
        int outtimeLimit = 15;
        protected override void OnExecute() {
            target = ModuleMgr.FightMgr.GetBuildByUid(nearBuild.value);
            if (nextBuild.value > 0)
            {
                BuildController target2 = ModuleMgr.FightMgr.GetBuildByUid(nextBuild.value);
                this.blackboard.SetVariableValue("preVector", target2.GetStandPoint());
            }
            sttime = elapsedTime;
            productId = target.GetCtrlData().GetProductId();
        }

        protected override void OnUpdate()
        {
            dropTimer += Time.deltaTime;
            if (dropTimer >= dropInterval)
            {
                dropTimer = 0f;
                if(agent.GetActorType() == Const.ActorType.Customer)
                {
                    if ((agent as CustomerCtrl).GetProductLeftNum(productId)<=0)
                    {
                        EndAction(true);
                        return;
                    }
                }else if(agent.IsProductWorker())
                {
                    if (agent.Stack.Count >= agent.Stack.MaxStack
                        || (agent as WorkerCtrl).GetProductFin()
                        || (agent.Stack.Count>0 && IsMachinGetFin()))
                    {
                        EndAction(true);
                        return;
                    }
                }
                if (target != null)
                {
                    target.RemoveProductStack(agent);
                    if (agent.IsProductWorker())
                    {
                        if (!(agent as WorkerCtrl).GetProductFin() && IsTreeEmpty())
                        {
                            EndAction(false);
                        }
                        else
                        {
                            SetTreeActorFlag(true);
                        }
                    }
                }
                if ((elapsedTime - sttime) > outtimeLimit)
                {
                    EndAction(agent.Stack.Count > 0);
                }
            }
        }

        public bool IsTreeEmpty()
        {
            if (target != null && target.GetCtrlData().GetBuildType() == (int)Const.BuildType.Tree)
            {
                return target.productStack.Count <= 0;
            }
            return false;
        }

        public bool IsMachinGetFin()
        {
            if (target != null && target.GetCtrlData().GetBuildType() == (int)Const.BuildType.Machine)
            {
               return  (target as MachineBuild).IsRawEmpty() && target.productStack.Count<=0;
            }
            return false;
        }
        //public bool IsTreeGetFin()
        //{
        //    if (target != null && target.GetCtrlData().GetBuildType() == (int)Const.BuildType.Tree)
        //    {
        //        return  target.productStack.Count <= 0;
        //    }
        //    return false;
        //}
        private void SetTreeActorFlag(bool ispick)
        {
            if (target!=null && target.GetCtrlData().GetBuildType() == (int)Const.BuildType.Tree)
            {
                (target as TreeBuild).SetIsPickingActor((agent as WorkerCtrl).Getuid(), ispick);
            }
        }
        protected override void OnStop()
        {
            base.OnStop();
            SetTreeActorFlag(false);
        }
    }
}