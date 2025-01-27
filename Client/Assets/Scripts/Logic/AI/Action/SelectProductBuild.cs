using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{

    [Name("查找产品生产建筑")]
    [Category("动作节点")]
    public class SelectProductBuild : ActionTask<ActorController>
    {
        [BlackboardOnly]
        public BBParameter<int> productId;

        protected override string info
        {
            get { return "查找 " + productId.ToString() + "生产建筑"; }
        }

        protected override void OnExecute()
        {
            BuildController info = null;
            for (int i = 0; i < RushManager.Instance.builds.Count; i++)
            {
                BuildController build = RushManager.Instance.builds[i];
                if (build.GetCtrlData().GetProductId() == productId.value)
                {
                    if (build.GetCtrlData().GetBuildType() == (int)Const.BuildType.Tree)
                    {
                        if (build.productStack.Count > 0 && !(build as TreeBuild).IsFullPick())
                        {
                            info = build;
                            break;
                        }
                    }
                    else if (build.GetCtrlData().GetBuildType() == (int)Const.BuildType.Machine)
                    {
                        if (build.productStack.Count > 0)
                        {
                            info = build;
                            break;
                        }
                    }
                }
            }
            if (info != null)
            {
                this.blackboard.SetVariableValue("preVector", info.GetStandPoint());
                this.blackboard.SetVariableValue("frombuildId", info.Getuid());
                EndAction(true);
            }
            else
            {
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