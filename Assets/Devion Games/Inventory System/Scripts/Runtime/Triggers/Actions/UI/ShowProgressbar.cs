using DevionGames.UIWidgets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(Canvas))]
    [ComponentMenu("UI/Show Progressbar")]
    [System.Serializable]
    public class ShowProgressbar : Action
    {
        [SerializeField]
        private string m_WidgetName = "General Progressbar";
        [SerializeField]
        private string m_Title = "";
        [SerializeField]
        private float  m_Duration = 1f;

        private float m_Time = 0f;

        private Progressbar m_Widget;

        public override void OnStart()
        {
            this.m_Time = 0f;
            this.m_Widget = WidgetUtility.Find<Progressbar>(this.m_WidgetName);
            if (this.m_Widget == null)
            {
                Debug.LogWarning("Missing progressbar widget " + this.m_WidgetName + " in scene!");
                return;
            }
            this.m_Widget.Show(this.m_Title);
        }

        public override ActionStatus OnUpdate()
        {

            if (this.m_Widget == null) {
                Debug.LogWarning("Missing progressbar widget " + this.m_WidgetName + " in scene!");
                return ActionStatus.Failure;
            }

            this.m_Time += Time.deltaTime;
            if (this.m_Time > this.m_Duration)
            {
                this.m_Widget.Close();
                return ActionStatus.Success;
            }
            this.m_Widget.SetProgress(this.m_Time / this.m_Duration);
            return ActionStatus.Running;
        }

        public override void OnInterrupt()
        {
            if (this.m_Widget != null)
                this.m_Widget.Close();
        }
    }
}