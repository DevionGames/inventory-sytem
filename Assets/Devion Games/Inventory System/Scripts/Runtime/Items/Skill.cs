using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [System.Serializable]
    public class Skill : UsableItem
    {
        [Range(0f,100f)]
        [SerializeField]
        protected float m_FixedSuccessChance = 50f;


        protected float m_CurrentValue = 0f;
        public float CurrentValue {
            get { return this.m_CurrentValue; }
            set {
                if (this.m_CurrentValue != value)
                {
                    this.m_CurrentValue = value;
                    if (Slot != null)
                        Slot.Repaint();
                }
            
            }
        }

        [SerializeField]
        protected SkillModifier m_GainModifier;
        public SkillModifier GainModifier {
            get { return this.m_GainModifier; }
            set { this.m_GainModifier = value; }
        }

        public bool CheckSkill() {
            m_GainModifier.Modify(this);

            bool result = (CurrentValue + this.m_FixedSuccessChance) > Random.Range(0f, 100f);
            return result;
        }

        public override void GetObjectData(Dictionary<string, object> data)
        {
            base.GetObjectData(data);
            data.Add("SkillValue",CurrentValue);
        }

        public override void SetObjectData(Dictionary<string, object> data)
        {
            base.SetObjectData(data);
            if(data.ContainsKey("SkillValue"))
                this.CurrentValue = System.Convert.ToSingle(data["SkillValue"]);
        }
    }
}