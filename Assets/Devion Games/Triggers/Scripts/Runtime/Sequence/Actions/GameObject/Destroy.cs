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
        [SerializeField]
        private float m_Delay = 0f;
        public override ActionStatus OnUpdate()
        {
            GameObject target = GetTarget(this.m_Target);
            GameObject.Destroy(target,this.m_Delay);
            return ActionStatus.Success;
        }
    }
}