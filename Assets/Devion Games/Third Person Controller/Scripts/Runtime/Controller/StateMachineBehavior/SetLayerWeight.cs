using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    public class SetLayerWeight : StateMachineBehaviour
    {
        [SerializeField]
        private AnimationEventType m_Type = AnimationEventType.OnStateExit;
        [SerializeField]
        private int m_Layer = 1;
        [SerializeField]
        private float m_Weight = 0f;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (this.m_Type == AnimationEventType.OnStateEnter)
                animator.SetLayerWeight(this.m_Layer, this.m_Weight);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (this.m_Type == AnimationEventType.OnStateUpdate)
                animator.SetLayerWeight(this.m_Layer, this.m_Weight);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (this.m_Type == AnimationEventType.OnStateExit)
                animator.SetLayerWeight(this.m_Layer, this.m_Weight);
        }

        public enum AnimationEventType
        {
            OnStateEnter,
            OnStateUpdate,
            OnStateExit
        }
    }
}