using DevionGames.UIWidgets;
using UnityEngine;


namespace DevionGames.InventorySystem
{
    public class DisplayTriggerTooltip : MonoBehaviour, ITriggerWentOutOfRange, ITriggerUsedHandler
    {
        [SerializeField]
        protected string m_Title;
        [SerializeField]
        protected string m_Instruction = "Pickup";

        protected TriggerTooltip m_Tooltip;
        protected BaseTrigger m_Trigger;


        private void Start()
        {
            m_Trigger = GetComponentInChildren<BaseTrigger>(true);
            this.m_Tooltip = WidgetUtility.Find<TriggerTooltip>("Trigger Tooltip");
        }

        private void Update()
        {

            if (!this.m_Trigger.InUse && this.m_Trigger.InRange && this.m_Trigger.IsBestTrigger())
            {
                DoDisplayTooltip(true);
            }
        }

        protected virtual void DoDisplayTooltip(bool state)
        {
            if (this.m_Tooltip == null) return;

            if (state)
            {
                this.m_Tooltip.Show(this.m_Title, this.m_Instruction);
            }
            else
            {
                this.m_Tooltip.Close();
            }
        }
   
        private void OnDestroy()
        {
            DoDisplayTooltip(false);
        }

        public void OnWentOutOfRange(GameObject player)
        {
            DoDisplayTooltip(false);
        }

        public void OnTriggerUsed(GameObject player)
        {
            DoDisplayTooltip(false);
        }
    }
}