using DevionGames.UIWidgets;
using UnityEngine;
using UnityEngine.UI;

namespace DevionGames.InventorySystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(GraphicRaycaster))]
    [ComponentMenu("Physics/Raycast Notify")]
    public class RaycastNotify : Raycast
    {
        [SerializeField]
        protected NotificationOptions m_SuccessNotification = null;
        [SerializeField]
        protected NotificationOptions m_FailureNotification = null;


        public override ActionStatus OnUpdate()
        {
            if (DoRaycast()) {
                if (this.m_SuccessNotification != null && !string.IsNullOrEmpty(this.m_SuccessNotification.text))
                    this.m_SuccessNotification.Show();
                return ActionStatus.Success;
            }
            if (this.m_FailureNotification != null && !string.IsNullOrEmpty(this.m_FailureNotification.text))
                this.m_FailureNotification.Show();
            return ActionStatus.Failure;
        }

    }
}