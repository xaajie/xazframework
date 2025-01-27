using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{

    [Name("开门")]
    [Category("动作节点")]
    public class OpenDoor : ActionTask<ActorController>
    {

        protected override string info
        {
            get { return "检测开门"; }
        }

        Door[] doors;
        bool checkdoor = true;
        float sttime = 0f;
        protected override void OnExecute() {
            OnUpdate();
        }

        protected override void OnUpdate() {
            if (checkdoor)
            {
                RaycastHit hit;
                if (Physics.Raycast(agent.transform.position + Vector3.up, agent.transform.forward, out hit, 0.5f, agent.entranceLayer, QueryTriggerInteraction.Collide))
                {
                    doors = hit.transform.GetComponentsInChildren<Door>();
                    foreach (var door in doors)
                    {
                        door.OpenDoor(agent.transform);
                    }
                    sttime = elapsedTime;
                    checkdoor = false;
                }
            }
            else
            {
                if (doors != null && (elapsedTime - sttime) >= 1)
                {
                    foreach (var door in doors)
                    {
                        door.CloseDoor();
                    }
                    EndAction(true);
                }
            }
        }

        protected override void OnPause() { OnStop(); }
        protected override void OnStop() {
            //if ( lastRequest != null && agent.gameObject.activeSelf ) {
            //    agent.ResetPath();
            //}
            //lastRequest = null;
        }
    }
}