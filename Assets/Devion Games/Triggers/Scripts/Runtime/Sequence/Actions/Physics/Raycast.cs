using UnityEngine;
using UnityEngine.UI;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(GraphicRaycaster))]
    [ComponentMenu("Physics/Raycast")]
    public class Raycast : Action
    {
        [SerializeField]
        protected TargetType m_Target = TargetType.Camera;
        [SerializeField]
        protected Vector3 m_Offset = Vector3.zero;
        [SerializeField]
        protected Direction m_Direction = Direction.Forward;
        [SerializeField]
        protected float m_MaxDistance = 15f;
        [SerializeField]
        protected LayerMask m_LayerMask = Physics.DefaultRaycastLayers;
        [SerializeField]
        protected LayerMask m_HitLayer = Physics.DefaultRaycastLayers;
        [SerializeField]
        protected QueryTriggerInteraction m_QueryTriggerInteraction = QueryTriggerInteraction.Collide;

        protected Transform m_TargetTransform;

        public override void OnStart()
        {
            this.m_TargetTransform = GetTarget(this.m_Target).transform;
        }

        public override ActionStatus OnUpdate()
        {
            return DoRaycast() ? ActionStatus.Success:ActionStatus.Failure;
        }

        protected virtual bool DoRaycast() {
            Vector3 startPosition = this.m_TargetTransform.position + this.m_TargetTransform.InverseTransformDirection(this.m_Offset);
            Vector3 direction = PhysicsUtility.GetDirection(this.m_TargetTransform, this.m_Direction);
            RaycastHit hit;
            if (Physics.Raycast(startPosition, direction, out hit, this.m_MaxDistance, this.m_LayerMask, this.m_QueryTriggerInteraction) && this.m_HitLayer.Contains(hit.collider.gameObject.layer))
            {
                return true;
            }

            return false;
        }
    }
}