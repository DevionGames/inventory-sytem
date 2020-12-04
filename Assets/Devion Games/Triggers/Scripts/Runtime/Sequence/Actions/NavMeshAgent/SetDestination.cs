using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(NavMeshAgent))]
    [ComponentMenu("NavMeshAgent/SetDestination")]
    public class SetDestination : Action
    {
        [SerializeField]
        private TargetType m_Target= TargetType.Player;
        [SerializeField]
        private Vector3 m_Destination= Vector3.zero;

        private NavMeshAgent m_Agent;

        public override void OnStart()
        {
            this.m_Agent = this.m_Target == TargetType.Self ? gameObject.GetComponent<NavMeshAgent>() : playerInfo.gameObject.GetComponent<NavMeshAgent>(); 
        }

        public override ActionStatus OnUpdate()
        {
            if (m_Agent == null)
            {
                Debug.LogWarning("Missing Component of type NavMeshAgent!");
                return ActionStatus.Failure;
            }
            this.m_Agent.SetDestination(this.m_Destination);
            return ActionStatus.Success;
        }
    }
}
