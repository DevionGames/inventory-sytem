using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(GameObject))]
    [ComponentMenu("GameObject/Set Name")]
    public class SetName : Action
    {
        [SerializeField]
        private TargetType m_Target = TargetType.Self;
        [InspectorLabel("Name")]
        [SerializeField]
        private string m_Value = string.Empty;

        public override ActionStatus OnUpdate()
        {
            GameObject target = GetTarget(this.m_Target);
            target.name = this.m_Value;
            Debug.Log("Executed");
            return ActionStatus.Success;
        }
    }
}