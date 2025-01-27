using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions
{

    [Name("是否手里有东西")]
    [Category("条件节点")]
    public class CheckThrow : ConditionTask<ActorController>
    {

        protected override bool OnCheck() {
            return agent.Stack.Count > 0;
        }

    }
}