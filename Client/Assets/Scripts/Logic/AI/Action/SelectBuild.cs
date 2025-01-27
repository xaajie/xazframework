using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections.Generic;

namespace NodeCanvas.Tasks.Actions
{

    [Name("选定建筑")]
    [Category("动作节点")]
    public class SelectBuild : ActionTask<ActorController>
    {
        public Const.BuildType buildType;
        public Const.BuildState buildState;
        public bool isCheckFix;
        protected override string info
        {
            get { return "选定 " + buildType.ToString() + buildState.ToString() + "建筑"; }
        }

        private bool CheckMatchBuildType(UserSceneBuildData data)
        {
            return buildType == Const.BuildType.All || data.GetBuildType() == (int)buildType;
        }

        private bool CheckBuildState(BuildController target, Const.BuildState vt, out int pId)
        {
            bool st = false;
            pId = -1;
            switch (vt)
            {
                case Const.BuildState.Every:
                    st = true;
                    break;
                case Const.BuildState.RawNotFull:
                    MachineBuild cvt = target as MachineBuild;
                    if (cvt != null)
                    {
                        for (int i = 0; i < cvt.rawPiles.Count; i++)
                        {
                            if (cvt.rawPiles[i].Count <= cvt.rawPiles[i].MaxStack)
                            {
                                pId = cvt.rawPiles[i].productId;
                                st = true;
                                break;
                            }
                        }
                    }
                    break;
                case Const.BuildState.ProductNotFull:
                    st = target.productStack.Count < target.GetCtrlData().GetInfo().GetProductCapacity();
                    pId = target.productStack.productId;
                    break;
                case Const.BuildState.ProductFull:
                    st = target.productStack.Count >= target.GetCtrlData().GetInfo().GetProductCapacity();
                    pId = target.productStack.productId;
                    break;
                case Const.BuildState.ProductNotEmpty:
                    st = target.productStack.Count > 0;
                    pId = target.productStack.productId;
                    break;
                case Const.BuildState.Broken:
                    st = target.GetCtrlData().IsBroken();
                    break;
            }
            return st;
        }
        private bool IsWorkForShelf()
        {
            return buildType == Const.BuildType.Shelf;
        }
        protected override void OnExecute()
        {
            BuildController info = null;
            if (isCheckFix)
            {
                if (buildType == Const.BuildType.CashDesk)
                {
                    info = RushManager.Instance.GetCashBuild();
                }
                else
                {
                    for (int i = 0; i < RushManager.Instance.builds.Count; i++)
                    {
                        BuildController build = RushManager.Instance.builds[i];
                        int sid;
                        if (CheckMatchBuildType(build.GetCtrlData()) && CheckBuildState(build, buildState, out sid))
                        {
                            info = build;
                            break;
                        }
                    }
                }
            }
            else
            {
                if (agent.GetActorType() == Const.ActorType.Customer)
                {
                    int productId = (agent as CustomerCtrl).GetFirstGetProduct();
                    for (int i = 0; i < RushManager.Instance.builds.Count; i++)
                    {
                        BuildController build = RushManager.Instance.builds[i];
                        if (CheckMatchBuildType(build.GetCtrlData()) && build.GetCtrlData().GetProductId() == productId)
                        {
                            this.blackboard.SetVariableValue("productId", productId);
                            agent.SetCurTaskId(productId);
                            info = build;
                            break;
                        }
                    }
                }
                else if (agent.IsProductWorker())
                {
                    List<int> workbuild = (agent as WorkerCtrl).GetCtrlData().GetWorkForBuild(IsWorkForShelf());
                    List<int> buildguid = new List<int> { };
                    int taskid = -1;
                    int tasknum = 0;
                    for (int i = 0; i < RushManager.Instance.builds.Count; i++)
                    {
                        BuildController build = RushManager.Instance.builds[i];
                        if (workbuild.IndexOf(build.GetCtrlData().GetBuildID()) != -1)
                        {
                            buildguid.Add(build.Getuid());
                        }
                    }
                    ListExtensions.Shuffle(buildguid);
                    for (int i = 0; i < buildguid.Count; i++)
                    {
                        BuildController build = ModuleMgr.FightMgr.GetBuildByUid(buildguid[i]);
                        int prid;
                        if (CheckBuildState(build, buildState, out prid))
                        {
                            taskid = prid;
                            if (buildState == Const.BuildState.RawNotFull)
                            {
                                MachineBuild macht = build as MachineBuild;
                                if (macht != null)
                                {
                                    tasknum =macht.GetRawMaxNum(taskid);
                                }
                            }
                            else
                            {
                                tasknum = build.productStack.MaxStack - build.productStack.Count;
                            }
                        }
                        if (taskid > 0 && tasknum>0)
                        {
                            this.blackboard.SetVariableValue("productId", taskid);
                            agent.SetCurTaskId(taskid);
                            agent.SetCurTaskNum(tasknum);
                            info = build;
                            break;
                        }
                    }

                }
            }

            if (info != null)
            {
                this.blackboard.SetVariableValue("preVector", info.GetStandPoint());
                this.blackboard.SetVariableValue("buildId", info.Getuid());
                EndAction(true);
            }
            else
            {
                agent.SetCurTaskId(-1);
                EndAction(false);
            }
        }

        //protected override void OnUpdate()
        //{

        //}

        //protected override void OnPause() { OnStop(); }
        //protected override void OnStop()
        //{
        //    //if ( lastRequest != null && agent.gameObject.activeSelf ) {
        //    //    agent.ResetPath();
        //    //}
        //    //lastRequest = null;
        //}
    }
}