using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(GraphicRaycaster))]
    [ComponentMenu("Physics/Overlap Sphere")]
    public class OverlapSphere : Action
    {
        [SerializeField]
        private TargetType m_Target = TargetType.Camera;
        [SerializeField]
        private float m_Radius = 1f;
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

            Collider[] colliders = Physics.OverlapSphere(startPosition, this.m_Radius, this.m_LayerMask, this.m_QueryTriggerInteraction);
            for (int i = 0; i < colliders.Length; i++) {
                if (this.m_HitSuccessLayer.Contains(colliders[i].gameObject.layer)) {
                    return ActionStatus.Success;
                }
            }
            return ActionStatus.Failure;
        }

    }
}