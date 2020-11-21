using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(Transform))]
    [ComponentMenu("Transform/Look Forward")]
    public class LookForward : Action
    {
        [SerializeField]
        private TargetType m_Target = TargetType.Player;
        private Transform m_Transform;
        private Transform m_CameraTransform;

        public override void OnStart()
        {
            this.m_CameraTransform = Camera.main.transform;
            this.m_Transform = GetTarget(m_Target).transform;
        }

        public override ActionStatus OnUpdate()
        {

            Quaternion lookRotation = Quaternion.Euler(this.m_Transform.eulerAngles.x, this.m_CameraTransform.eulerAngles.y, this.m_Transform.eulerAngles.z);

            if (SelectableObject.current != null)
            {
                Vector3 direction = SelectableObject.current.transform.position - this.m_Transform.position;
                direction.y = 0f;
                lookRotation = Quaternion.LookRotation(direction, Vector3.up);
                

            }
            this.m_Transform.rotation = lookRotation;

            return ActionStatus.Success;
        }
    }
}