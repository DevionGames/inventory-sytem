using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(Animator))]
    [ComponentMenu("Animator/CrossFade")]
    public class CrossFade : Action
    {
        [SerializeField]
        private TargetType m_Target= TargetType.Player;
        [SerializeField]
        private string m_AnimatorState = "Pickup";
        [SerializeField]
        private float m_TransitionDuration = 0.2f;

        private Animator m_Animator;
        private int m_ShortNameHash;

        public override void OnStart()
        {
            this.m_ShortNameHash = Animator.StringToHash(this.m_AnimatorState);
            
            this.m_Animator = this.m_Target == TargetType.Self ? gameObject.GetComponentInChildren<Animator>() : playerInfo.animator; 
        }

        public override ActionStatus OnUpdate()
        {
            if (m_Animator == null)
            {
                Debug.LogWarning("Missing Component of type Animator!");
                return ActionStatus.Failure;
            }
            this.m_Animator.CrossFadeInFixedTime(this.m_ShortNameHash, this.m_TransitionDuration);
            return ActionStatus.Success;
        }
    }
}
