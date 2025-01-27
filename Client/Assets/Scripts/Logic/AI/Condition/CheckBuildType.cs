using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Conditions
{

    [Name("检测建筑类型")]
    [Category("条件节点")]
    public class CheckBuildType : ConditionTask<WorkerCtrl>
    {
        [BlackboardOnly]
        public BBParameter<int> checkBuild;

        public Const.BuildType chectvt;
        protected override bool OnCheck() {
            BuildController target = ModuleMgr.FightMgr.GetBuildByUid(checkBuild.value);
            return (int)chectvt == target.GetCtrlData().GetBuildType();
        }
    }
}