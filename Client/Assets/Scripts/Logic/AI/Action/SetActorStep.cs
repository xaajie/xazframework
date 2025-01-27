using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{

    [Name("设置行为标识")]
    [Category("动作节点")]
    public class SetActorStep : ActionTask<ActorController>
    {
        public Const.ActorStep actorsetp;

        protected override void OnExecute() {
            agent.SetActorStep(actorsetp);
            EndAction(true);
        }

        protected override string info
        {
            get { return "设置 " + actorsetp.ToString() + "标识"; }
        }

    }
}