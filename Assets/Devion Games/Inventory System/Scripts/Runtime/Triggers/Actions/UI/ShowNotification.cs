using DevionGames.UIWidgets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(Canvas))]
    [ComponentMenu("UI/Show Notification")]
    [System.Serializable]
    public class ShowNotification : Action
    {
        [SerializeField]
        private string m_WidgetName = "Notification";
        [SerializeField]
        private NotificationOptions m_Notification = null;

        public override ActionStatus OnUpdate()
        {
            Notification widget = WidgetUtility.Find<Notification>(this.m_WidgetName);
            if (widget == null)
            {
                Debug.LogWarning("Missing notification widget " + this.m_WidgetName + " in scene!");
                return ActionStatus.Failure;
            }
            return widget.AddItem(this.m_Notification)?ActionStatus.Success:ActionStatus.Failure;
        }
    }
}