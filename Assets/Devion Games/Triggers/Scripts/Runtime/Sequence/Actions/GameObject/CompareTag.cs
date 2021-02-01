using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(GameObject))]
    [ComponentMenu("GameObject/Compare Tag")]
    public class CompareTag : Action, ICondition
    {
        [SerializeField]
        private TargetType m_Target = TargetType.Self;
        [SerializeField]
        private string m_Tag = "Player";

        public override ActionStatus OnUpdate()
        {
            GameObject target = GetTarget(this.m_Target);
            return target.CompareTag(this.m_Tag) ? ActionStatus.Success : ActionStatus.Failure;
        }
    }
}