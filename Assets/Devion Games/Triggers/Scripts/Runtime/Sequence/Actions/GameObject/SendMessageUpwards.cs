using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(GameObject))]
    [ComponentMenu("GameObject/SendMessageUpwards")]
    public class SendMessageUpwards : Action
    {
        [SerializeField]
        private TargetType m_Target = TargetType.Player;
        [SerializeField]
        private string methodName = string.Empty;
        [SerializeField]
        private ArgumentVariable m_Argument = null;
        [SerializeField]
        private SendMessageOptions m_Options = SendMessageOptions.DontRequireReceiver;

        private GameObject m_TargetÓbject;

        public override void OnStart()
        {
            this.m_TargetÓbject = GetTarget(this.m_Target);
        }

        public override ActionStatus OnUpdate()
        {
            if (m_Argument.ArgumentType != ArgumentType.None)
            {
                this.m_TargetÓbject.SendMessageUpwards(methodName, m_Argument.GetValue(), m_Options);
            }
            else
            {
                this.m_TargetÓbject.SendMessageUpwards(methodName, m_Options);
            }

            return ActionStatus.Success;
        }
    }
}