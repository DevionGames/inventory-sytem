using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon("Component")]
    [ComponentMenu("Component/Set Enabled")]
    public class SetEnabled : Action
    {
        [SerializeField]
        private TargetType m_Target = TargetType.Player;
        [SerializeField]
        private string m_ComponentName=string.Empty;
        [SerializeField]
        private bool m_Enable=false;

        private Behaviour m_Component;
        private bool m_IsEnabled;

        public override void OnStart()
        {
            GameObject target = GetTarget(this.m_Target);
            this.m_Component = target.GetComponent(this.m_ComponentName) as Behaviour;
            if (this.m_Component != null)
                this.m_IsEnabled = this.m_Component.enabled;
        }

        public override ActionStatus OnUpdate()
        {
            if (this.m_Component == null)
            {
                Debug.LogWarning("Missing Component of type "+this.m_ComponentName+"!");
                return ActionStatus.Failure;
            }
            this.m_Component.enabled = this.m_Enable;
            return ActionStatus.Success;
        }

        public override void OnInterrupt()
        {
            if (this.m_Component != null)
                this.m_Component.enabled = this.m_IsEnabled;
        }
    }
}