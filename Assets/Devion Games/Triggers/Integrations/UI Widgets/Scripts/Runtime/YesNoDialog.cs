using DevionGames.UIWidgets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [ComponentMenu("UI/Yes No Dialog")]
    [System.Serializable]
    public class YesNoDialog : Action
    {
        [SerializeField]
        private string m_WidgetName = "Dialog Box";
        [SerializeField]
        private string m_Title = "Are you sure?";
        [SerializeField]
        private string m_Text = string.Empty;
        [SerializeField]
        private Sprite m_Icon = null;

        private ActionStatus m_Status;
        private DialogBox m_DialogBox;

        public override void OnStart()
        {
            this.m_DialogBox = WidgetUtility.Find<DialogBox>(this.m_WidgetName);
            if (this.m_DialogBox == null)
            {
                Debug.LogWarning("Missing dialog box widget " + this.m_WidgetName + " in scene!");
                return;
            }
            this.m_DialogBox.RegisterListener("OnClose", OnClose);
            this.m_Status = ActionStatus.Running;
            this.m_DialogBox.Show(this.m_Title,this.m_Text,this.m_Icon,OnResponse,"Yes","No");
        }

        public override ActionStatus OnUpdate()
        {
            return this.m_Status;
        }

        private void OnClose(CallbackEventData ev) {
            this.m_Status = ActionStatus.Failure;
            this.m_DialogBox.RemoveListener("OnClose", OnClose);
        }

        private void OnResponse(int result) {
            if (result == 0){
                this.m_Status = ActionStatus.Success;
            }else {
                this.m_Status = ActionStatus.Failure;
            }
        }
    }
}