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

        private Animator m_Animator;

        public override void OnStart()
        {
            this.m_Animator = this.m_Target == TargetType.Self ? gameObject.GetComponent<Animator>() : playerInfo.animator;
        }

        public override ActionStatus OnUpdate()
        {
            if (m_Animator == null)
            {
                Debug.LogWarning("Missing Component of type Animator!");
                return ActionStatus.Failure;
            }
            this.m_Animator.SetFloat(this.m_ParameterName, this.m_Value);

            return ActionStatus.Success;
        }
    }
}
