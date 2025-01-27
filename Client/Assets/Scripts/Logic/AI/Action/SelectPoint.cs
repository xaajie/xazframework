using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
    [Name("选位置")]
    [Category("动作节点")]
    public class SelectPoint : ActionTask<ActorController>
    {
        public Const.ScenePoint pointType;
        protected override void OnExecute() 
        {
            Vector3 pt = Vector3.zero;
            if (pointType == Const.ScenePoint.Trash)
            {
                pt = RushManager.Instance.trashBin.GetStandPoint();
            }
            else if(pointType == Const.ScenePoint.GuardBorn)
            {               
                pt = RushManager.Instance.GetGuard().GetBindPoint();
            }
            else
            {
                pt = RushManager.Instance.scenePos.spawnPoint;
            }
            this.blackboard.SetVariableValue("preVector", pt);
            EndAction(true);
        }
    }
}