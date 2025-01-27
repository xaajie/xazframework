using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions
{

    [Name("移动到")]
    [Category("动作节点")]
    public class MoveTo : ActionTask<ActorController>
    {

        [BlackboardOnly]
        public BBParameter<Vector3> targetPosition;
        public float randomRadius=0;
        public bool needOpenDoor;
        private BBParameter<float> keepDistance = 1f;
        private Vector3? lastRequest;
        int entranceLayer;
        Door[] doors;
        bool checkdoor = true;
        float sttime = 0f;
        protected override string info
        {
            get { return "移动到 " + targetPosition; }
        }

        protected override void OnExecute() {
            checkdoor = needOpenDoor;
            entranceLayer = 1 << LayerMask.NameToLayer(XazConfig.LayerDefine.SceneDoor);
            if (randomRadius > 0)
            {
                var randomCircle = Random.insideUnitCircle * randomRadius;
                targetPosition.value = targetPosition.value + new Vector3(randomCircle.x, 0, randomCircle.y);
            }
            if ( Vector3.Distance(agent.transform.position, targetPosition.value) < keepDistance.value ) {
                EndAction(true);
                return;
            }
        }

        protected override void OnUpdate() {
            if ( lastRequest != targetPosition.value ) {
                if ( !agent.MoveTo(targetPosition.value) ) {
                    EndAction(false);
                    return;
                }
            }

            lastRequest = targetPosition.value;
            if (needOpenDoor)
            {
                if (checkdoor)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(agent.transform.position + Vector3.up, agent.transform.forward, out hit, 0.5f, entranceLayer, QueryTriggerInteraction.Collide))
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
                        doors = null;
                    }
                }
            }
            if ( agent.agent.HasArrived(keepDistance.value) ) {
                agent.StopMove();
                EndAction(true);
            }
        }

        protected override void OnPause() { OnStop(); }
        protected override void OnStop() {
            if ( lastRequest != null && agent.gameObject.activeSelf ) {
                agent.StopMove();
            }
            lastRequest = null;
        }

        public override void OnDrawGizmosSelected()
        {
            if (targetPosition.value != null)
            {
                Gizmos.DrawWireSphere(targetPosition.value, keepDistance.value);
            }
        }
    }
}