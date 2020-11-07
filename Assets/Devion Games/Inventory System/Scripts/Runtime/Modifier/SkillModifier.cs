using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [CreateAssetMenu(fileName = "SimpleSkillModifier", menuName = "Devion Games/Inventory System/Modifiers/Skill")]
    public class SkillModifier : ScriptableObject, IModifier<Skill>
    {
        [SerializeField]
        protected AnimationCurve m_Chance;
        [SerializeField]
        protected AnimationCurve m_Gain;

        public void Modify(Skill item)
        {
            float currentValue = item.CurrentValue;
            float chance = this.m_Chance.Evaluate(currentValue / 100f) * 100f;
            float p = Random.Range(0f, 100f);

            if (chance > p)
            {
                float gainValue = this.m_Gain.Evaluate(currentValue / 100f);
                item.CurrentValue = item.CurrentValue + gainValue;
                
                InventoryManager.Notifications.skillGain.Show(item.DisplayName, gainValue.ToString("F1"), item.CurrentValue.ToString("F1"));
            }

        }
    }
}