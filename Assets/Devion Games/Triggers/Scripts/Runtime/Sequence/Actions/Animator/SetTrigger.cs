using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(Animator))]
    [ComponentMenu("Animator/Set Trigger")]
    public class SetTrigger: Action
    {
        [SerializeField]
        private TargetType m_Target = TargetType.Player;
        [SerializeField]
        private string m_ParameterName = string.Empty;

        private Animator m_Animator;

        public override void OnStart()
        {
            this.m_Animator = this.m_Target == TargetType.Self ? gameObject.GetComponentInChildren<Animator>() : playerInfo.animator;
        }

        public override ActionStatus OnUpdate()
        {
            if (m_Animator == null)
            {
                Debug.LogWarning("Missing Component of type Animator!");
                return ActionStatus.Failure;
            }
            this.m_Animator.SetTrigger(this.m_ParameterName);

            return ActionStatus.Success;
        }
    }
}
