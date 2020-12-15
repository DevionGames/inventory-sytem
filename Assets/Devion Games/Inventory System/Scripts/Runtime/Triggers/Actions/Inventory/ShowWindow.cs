using System.Collections;
using System.Collections.Generic;
using DevionGames.UIWidgets;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon("Item")]
    [ComponentMenu("Inventory System/Show Window")]
    public class ShowWindow : Action, ITriggerUnUsedHandler
    {
        [Tooltip("The name of the window to show.")]
        [SerializeField]
        private string m_WindowName = "Loot";
        [SerializeField]
        private bool m_DestroyWhenEmpty = false;


        private ItemContainer m_ItemContainer;
        private ItemCollection m_ItemCollection;
        private ActionStatus m_WindowStatus= ActionStatus.Inactive;

        public override void OnSequenceStart()
        {
            this.m_WindowStatus = ActionStatus.Inactive;
            this.m_ItemContainer = WidgetUtility.Find<ItemContainer>(this.m_WindowName);
            if (this.m_ItemContainer != null) {
                this.m_ItemContainer.RegisterListener("OnClose",(CallbackEventData eventData)=>{ this.m_WindowStatus = ActionStatus.Success;  });
            }
            this.m_ItemCollection = gameObject.GetComponent<ItemCollection>();
            if (this.m_ItemCollection != null)
            {
                this.m_ItemCollection.onChange.AddListener(delegate ()
                {
                    if (this.m_ItemCollection.IsEmpty && this.m_DestroyWhenEmpty)
                    {
                        InventoryManager.Destroy(gameObject);
                    }
                });
            }
        }

        public void OnTriggerUnUsed(GameObject player)
        {
            if (m_ItemContainer != null) {
                this.m_ItemContainer.Close();
                Trigger.currentUsedWindow = null;
            }
        }

        public override ActionStatus OnUpdate()
        {
            if (this.m_ItemContainer == null)
            {
                Debug.LogWarning("Missing window " + this.m_WindowName + " in scene!");
                return ActionStatus.Failure;
            }

            if (this.m_WindowStatus == ActionStatus.Inactive)
            {
                Trigger.currentUsedWindow = this.m_ItemContainer;
                if (this.m_ItemCollection == null) {
                    this.m_ItemContainer.Show();
                }else{
                    this.m_ItemContainer.Collection = this.m_ItemCollection;
                    this.m_ItemContainer.Show();
                
                }
                this.m_WindowStatus = ActionStatus.Running;
            }
            return this.m_WindowStatus;
        }

       
    }
}
