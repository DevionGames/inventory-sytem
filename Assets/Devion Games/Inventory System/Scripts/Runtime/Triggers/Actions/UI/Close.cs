using DevionGames.UIWidgets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(Canvas))]
    [ComponentMenu("UI/Close")]
    [System.Serializable]
    public class Close : Action
    {
        [SerializeField]
        private string m_WidgetName = "General Progressbar";


        public override ActionStatus OnUpdate()
        {
            UIWidget widget = WidgetUtility.Find<UIWidget>(this.m_WidgetName);
            if (widget == null)
            {
                Debug.LogWarning("Missing notification widget " + this.m_WidgetName + " in scene!");
                return ActionStatus.Failure;
            }
            widget.Close();
            return ActionStatus.Success;
        }
    }
}