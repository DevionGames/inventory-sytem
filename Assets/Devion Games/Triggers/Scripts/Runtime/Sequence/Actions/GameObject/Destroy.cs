using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(GameObject))]
    [ComponentMenu("GameObject/Destroy")]
    public class Destroy : Action
    {
        [SerializeField]
        private TargetType m_Target = TargetType.Self;

        public override ActionStatus OnUpdate()
        {
            GameObject target = GetTarget(this.m_Target);
            GameObject.Destroy(target);
            return ActionStatus.Success;
        }
    }
}