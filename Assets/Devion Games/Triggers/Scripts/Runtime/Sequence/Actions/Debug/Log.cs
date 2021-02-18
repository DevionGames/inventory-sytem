using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [ComponentMenu("Debug/Log")]
    [System.Serializable]
    public class Log : Action
    {
        [SerializeField]
        private string m_Message = string.Empty;

        public override ActionStatus OnUpdate()
        {
            Debug.Log(this.m_Message);
            return ActionStatus.Success;
        }
    }
}
