using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(Animator))]
    [ComponentMenu("Animator/Set Random Float")]
    public class SetRandomFloat : Action
    {
        [SerializeField]
        private TargetType m_Target = TargetType.Player;
        [SerializeField]
        private string m_ParameterName = "Attack";
        [SerializeField]
        private float m_DampTime = 0f;
        [SerializeField]
        private float m_Min = 0;
        [SerializeField]
        private float m_Max = 1;
        [SerializeField]
        private bool m_RoundToInt = false;

        private Animator m_Animator;

        public override void OnStart()
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

            float random = Random.Range(this.m_Min, this.m_Max);
            if (this.m_RoundToInt) {
                random = Mathf.Round(random);
            }
            this.m_Animator.SetFloat(this.m_ParameterName, random);

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
            float random = Random.Range(this.m_Min, this.m_Max);
            if (this.m_RoundToInt)
            {
                random = Mathf.Round(random);
            }
            this.m_Animator.SetFloat(this.m_ParameterName, random, this.m_DampTime, Time.deltaTime);
        }
    }
}
