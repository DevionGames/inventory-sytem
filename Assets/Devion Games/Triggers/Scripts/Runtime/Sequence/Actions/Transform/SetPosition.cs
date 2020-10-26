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

        private Transform m_Transform;


        public override void OnStart()
        {
            this.m_Transform = GetTarget(this.m_Target).transform;
        }

        public override ActionStatus OnUpdate()
        {
            this.m_Transform.position = this.m_Position;
            return ActionStatus.Success;
        }
    }
}