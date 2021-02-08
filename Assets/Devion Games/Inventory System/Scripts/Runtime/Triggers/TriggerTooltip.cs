using DevionGames.UIWidgets;
using UnityEngine;
using UnityEngine.UI;

namespace DevionGames.InventorySystem
{
    public class TriggerTooltip : UIWidget
    {
        [Header("References")]
        [SerializeField]
        protected Text m_Title;
        [SerializeField]
        protected Text m_Instruction;

        public void Show(string title, string instruction) {
            this.m_Title.text = title;
            this.m_Instruction.text = instruction;
            base.Show();
        }
    }
}