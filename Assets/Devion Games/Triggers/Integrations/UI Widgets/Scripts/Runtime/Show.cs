using DevionGames.UIWidgets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [ComponentMenu("UI/Show")]
    [System.Serializable]
    public class Show : Action
    {
        [SerializeField]
        private string m_WidgetName = "";
        
        public override ActionStatus OnUpdate()
        {
            UIWidget widget = WidgetUtility.Find<UIWidget>(this.m_WidgetName);
            if (widget == null)
            {
                Debug.LogWarning("Missing widget " + this.m_WidgetName + " in scene!");
                return ActionStatus.Failure;
            }
            widget.Show();

            return ActionStatus.Success;
        }
    }
}