using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DevionGames.InventorySystem
{
    public class SkillItemView : ItemView
    {
        [Tooltip("Text reference to display skill value.")]
        [SerializeField]
        protected Text m_Value;

        public override void Repaint(Item item)
        {
            Skill skill = item as Skill;

            if (this.m_Value != null)
            {
                this.m_Value.text = (skill != null ? skill.CurrentValue.ToString("F1") + "%" : string.Empty);
            }
        }
    }
}