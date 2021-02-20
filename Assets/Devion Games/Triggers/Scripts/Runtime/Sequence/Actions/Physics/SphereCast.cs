using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(GraphicRaycaster))]
    [ComponentMenu("Physics/SphereCast")]
    public class SphereCast : Action
    {
        [SerializeField]
        private TargetType m_Target = TargetType.Camera;
        [SerializeField]
        private Direction m_Direction = Direction.Forward;
        [SerializeField]
        private float m_Radius = 1f;
        [SerializeField]
        private float m_MaxDistance = 5f;
        [SerializeField]
        private LayerMask m_LayerMask = Physics.DefaultRaycastLayers;
        [SerializeField]
        private LayerMask m_HitSuccessLayer = Physics.DefaultRaycastLayers;
        [SerializeField]
        private QueryTriggerInteraction m_QueryTriggerInteraction = QueryTriggerInteraction.Collide;

        private Transform m_TargetTransform;

        public override void OnStart()
        {
            this.m_TargetTransform = GetTarget(this.m_Target).transform;
        }

        public override ActionStatus OnUpdate()
        {
            Vector3 startPosition = this.m_TargetTransform.position;
            Vector3 direction = PhysicsUtility.GetDirection(this.m_TargetTransform, this.m_Direction);
            RaycastHit hit;
            if (Physics.SphereCast(startPosition+Vector3.up*0.2f, this.m_Radius,direction,out hit,this.m_MaxDistance, this.m_LayerMask, this.m_QueryTriggerInteraction) && this.m_HitSuccessLayer.Contains(hit.collider.gameObject.layer )){

                Debug.Log(hit.collider.name);
                return ActionStatus.Success;
            }
            return ActionStatus.Failure;
        }

    }
}