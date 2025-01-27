using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Conditions
{

    [Name("播放动作")]
    [Category("动作节点")]
    public class PlayAction : ActionTask<ActorController>
    {

        public SpineAnimCtrl.SpineAnimState vt;

        protected override string info
        {
            get { return "播放 " + vt.ToString() + "动作"; }
        }

        protected override void OnExecute()
        {
            agent.SetTargetAnim(vt);
        }

        float dropInterval = 0.4f;
        protected override void OnUpdate()
        {

            if (elapsedTime >= dropInterval)
            {
                EndAction(true);
            }
        }

        protected override void OnPause() { OnStop(); }
        protected override void OnStop()
        {

        }
    }
}