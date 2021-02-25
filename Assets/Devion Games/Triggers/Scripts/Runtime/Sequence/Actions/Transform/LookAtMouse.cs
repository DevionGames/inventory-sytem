using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(Transform))]
    [ComponentMenu("Transform/Look At Mouse")]
    public class LookAtMouse : Action
    {
        [SerializeField]
        private TargetType m_Target = TargetType.Player;
        [SerializeField]
        private float m_MaxDistance = 100f;
        [SerializeField]
        private float m_Speed = 15f;

        private Quaternion m_LastRotation;
        private Quaternion m_DesiredRotation;

        private Transform m_Transform;


        public override void OnStart()
        {
            this.m_Transform = GetTarget(this.m_Target).transform;
            this.m_LastRotation = this.m_Transform.rotation;
            this.m_DesiredRotation = m_LastRotation;
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, this.m_MaxDistance))
            {

                Vector3 targetPosition = hit.point;
                Vector3 position = this.m_Transform.position;
                targetPosition.y = position.y;

                Vector3 dir = targetPosition - position;
                if (dir.sqrMagnitude > 0f)
                {
                    m_DesiredRotation = Quaternion.LookRotation(dir);
                }
            }
        }

        public override ActionStatus OnUpdate()
        {
            m_LastRotation = Quaternion.Slerp(m_LastRotation, m_DesiredRotation, this.m_Speed * Time.deltaTime);
            playerInfo.transform.rotation = m_LastRotation;
            return Quaternion.Angle(m_LastRotation, m_DesiredRotation) > 5f ? ActionStatus.Running : ActionStatus.Success;
        }
    }
}