using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(Transform))]
    [ComponentMenu("Transform/Set Position")]
    public class SetPosition : Action
    {
        [SerializeField]
        private TargetType m_Target = TargetType.Player;
        [SerializeField]
        private Vector3 m_Position=Vector3.zero;
        [SerializeField]
        private bool m_SetCameraRelative = true;

        private Transform m_Transform;
        private Transform m_CameraTransform;

        public override void OnStart()
        {
            this.m_Transform = GetTarget(this.m_Target).transform;
            this.m_CameraTransform = Camera.main.transform;
        }

        public override ActionStatus OnUpdate()
        {
            Vector3 dir =  this.m_CameraTransform.position - this.m_Transform.position;

            this.m_Transform.position = this.m_Position;
            if(this.m_SetCameraRelative)
                this.m_CameraTransform.position = this.m_Position + dir;

            return ActionStatus.Success;
        }
    }
}