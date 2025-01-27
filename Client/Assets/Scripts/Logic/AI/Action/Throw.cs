using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{

    [Name("扔垃圾")]
    [Category("动作节点")]
    public class Throw : ActionTask<ActorController>
    {
        private float throwInterval = 0.2f;
        private Vector3 throwOffset = new Vector3(0f, 0.1f, 0.5f);
        private float throwCooldown;
        protected override void OnExecute()
        {

        }

        protected override void OnUpdate()
        {
            throwCooldown -= Time.deltaTime;

            if (throwCooldown <= 0)
            {
                throwCooldown = throwInterval;
                if (agent.Stack.Count <= 0)
                {
                    EndAction(true);
                    return;
                }
                RushManager.Instance.trashBin.ThrowToBin(agent.Stack);
            }
        }

        //protected override void OnPause() { OnStop(); }
        //protected override void OnStop() {
        //    //if ( lastRequest != null && agent.gameObject.activeSelf ) {
        //    //    agent.ResetPath();
        //    //}
        //    //lastRequest = null;
        //}
    }
}