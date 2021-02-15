using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(Animator))]
    [ComponentMenu("Animator/Is Name")]
    public class IsName : Action, ICondition
    {
        [SerializeField]
        private TargetType m_Target = TargetType.Player;
        [SerializeField]
        private int layer = 0;
        [SerializeField]
        private string name = string.Empty;

        private Animator m_Animator;

        public override void OnStart()
        {
            this.m_Animator = this.m_Target == TargetType.Self ? gameObject.GetComponentInChildren<Animator>() : playerInfo.animator;
        }

        public override ActionStatus OnUpdate()
        {
            if (m_Animator == null)
            {
                Debug.LogWarning("Missing Component of type Animator!");
                return ActionStatus.Failure;
            }

            AnimatorStateInfo stateInfo= this.m_Animator.GetCurrentAnimatorStateInfo(layer);

            return stateInfo.IsName(name)? ActionStatus.Success : ActionStatus.Failure;
        }
    }
}
