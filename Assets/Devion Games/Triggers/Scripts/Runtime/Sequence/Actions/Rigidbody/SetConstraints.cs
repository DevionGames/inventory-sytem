using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(Rigidbody))]
    [ComponentMenu("Rigidbody/Set Constraints")]
    public class SetConstraints : Action
    {
        [SerializeField]
        private TargetType m_Target = TargetType.Player;
        [SerializeField]
        private RigidbodyConstraints m_Constraints= RigidbodyConstraints.FreezePosition;

        private Rigidbody m_Rigidbody;

        public override void OnStart()
        {
            this.m_Rigidbody = this.m_Target == TargetType.Self ? gameObject.GetComponent<Rigidbody>() : playerInfo.gameObject.GetComponent<Rigidbody>();
        }

        public override ActionStatus OnUpdate()
        {
            if (this.m_Rigidbody == null)
            {
                Debug.LogWarning("Missing Component of type Rigidbody!");
                return ActionStatus.Failure;
            }
            this.m_Rigidbody.constraints = m_Constraints;

            return ActionStatus.Success;
        }
    }
}
