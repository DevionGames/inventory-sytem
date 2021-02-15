using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(Animator))]
    [ComponentMenu("Animator/Set Float")]
    public class SetFloat : Action
    {
        [SerializeField]
        private TargetType m_Target = TargetType.Player;
        [SerializeField]
        private string m_ParameterName = "Forward Input";
        [SerializeField]
        private float m_Value = 1f;
        [SerializeField]
        private float m_DampTime = 0f;

        private Animator m_Animator;

        public override void OnSequenceStart()
        {
            this.m_Animator = this.m_Target == TargetType.Self ? gameObject.GetComponentInChildren<Animator>() : playerInfo.animator;
        }

        public override ActionStatus OnUpdate()
        {
            if (this.m_DampTime > 0f)
                return ActionStatus.Success;

            if (m_Animator == null)
            {
                Debug.LogWarning("Missing Component of type Animator!");
                return ActionStatus.Failure;
            }
            this.m_Animator.SetFloat(this.m_ParameterName, this.m_Value);

            return ActionStatus.Success;
        }

        public override void Update()
        {
            if (this.m_DampTime <= 0f)
                return;

            if (m_Animator == null)
            {
                Debug.LogWarning("Missing Component of type Animator!");
                return;
            }
            this.m_Animator.SetFloat(this.m_ParameterName, this.m_Value, this.m_DampTime, Time.deltaTime);
        }
    }
}
