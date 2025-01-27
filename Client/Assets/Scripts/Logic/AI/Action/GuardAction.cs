using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{

    [Name("守卫活动")]
    [Category("动作节点")]
    public class GuardAction : ActionTask<ActorController>
    {
        [BlackboardOnly]
        public BBParameter<int> nearBuild;
        [BlackboardOnly]
        public BBParameter<int> actorId;
        protected override void OnExecute()
        {
            agent.SetTargetAnim(SpineAnimCtrl.SpineAnimState.wakeup);
            bool canFIx = false;
            if (nearBuild != null && nearBuild.value > 0)
            {
                canFIx = ModuleMgr.FightMgr.FixBuild(nearBuild.value,false);
            }
            else if (actorId != null && actorId.value > 0)
            {
                canFIx = ModuleMgr.FightMgr.WakeupActor(actorId.value);
            }
            if (!canFIx)
            {
                EndAction(true);
            }
        }

        float dropTimer = 0f;
        protected override void OnUpdate()
        {
            dropTimer += Time.deltaTime;
            if (dropTimer >= Const.FixWakeupDuration)
            {
                dropTimer = 0f;
                EndAction(true);
            }
        }
    }
}