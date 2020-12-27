using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    public class AnimationEventSender : StateMachineBehaviour
    {
        [SerializeField]
        private AnimationEventType m_Type= AnimationEventType.OnStateExit;
        [SerializeField]
        private string m_EventName="OnEndUse";

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (this.m_Type == AnimationEventType.OnStateEnter)
                animator.SendMessage(this.m_EventName, SendMessageOptions.DontRequireReceiver);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (this.m_Type == AnimationEventType.OnStateUpdate)
                animator.SendMessage(this.m_EventName, SendMessageOptions.DontRequireReceiver);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(this.m_Type== AnimationEventType.OnStateExit)
                animator.SendMessage(this.m_EventName, SendMessageOptions.DontRequireReceiver);
        }

        public enum AnimationEventType { 
            OnStateEnter,
            OnStateUpdate,
            OnStateExit
        }
    }
}