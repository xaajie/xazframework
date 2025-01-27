using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections.Generic;
using Table;
using UnityEngine;
using static Const;

namespace NodeCanvas.Tasks.Actions
{

    [Name("放货")]
    [Category("动作节点")]
    public class PutProduct : ActionTask<ActorController>
    {
        [BlackboardOnly]
        public BBParameter<int> nearBuild;

        private bool IsRaw = false;

        BuildController target;
        int count = 0;
        protected override void OnExecute()
        {
            target = ModuleMgr.FightMgr.GetBuildByUid(nearBuild.value);
            if (agent.Stack.productIds.IndexOf(target.productStack.productId) != -1)
            {
                IsRaw = false;
            }
            else
            {
                IsRaw = true;
            }
            count = 0;
        }

        float dropTimer = 0f;
        float dropInterval = 0.2f;
        protected override void OnUpdate()
        {
            dropTimer += Time.deltaTime;
            if (dropTimer >= dropInterval)
            {
                dropTimer = 0f;
                count++;
                if (IsRaw)
                {
                    MachineBuild macht = target as MachineBuild;
                    if (macht != null)
                    {
                        int handid = agent.Stack.Count>0?agent.Stack.productIds[0]:-1;
                        if (agent.Stack.GetNumById(handid) <= 0 || macht.GetRawLeftNum(handid) <= 0)
                        {
                            EndAction(true);
                        }
                        else
                        {
                            if (target != null)
                            {
                                macht.AddRawStack(agent, handid);
                            }
                        }
                    }
                }
                else
                {
                    if (agent.Stack.GetNumById(target.productStack.productId) <= 0 || target.productStack.Count >= target.productStack.MaxStack)
                    {
                        EndAction(true);
                    }
                    else
                    {
                        if (target != null)
                        {
                            target.AddProductStack(agent);
                        }
                    }
                }
            }
        }

        protected override void OnPause() { OnStop(); }
        protected override void OnStop()
        {
            //if ( lastRequest != null && agent.gameObject.activeSelf ) {
            //    agent.ResetPath();
            //}
            //lastRequest = null;
        }
    }
}