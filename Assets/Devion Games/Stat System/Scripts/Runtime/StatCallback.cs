using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.StatSystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [System.Serializable]
    public class StatCallback
    {
        [SerializeField]
        protected ValueType m_ValueType = ValueType.CurrentValue;
        [SerializeField]
        protected ConditionType m_Condition;
        [SerializeField]
        protected float m_Value = 0f;
        [SerializeField]
        protected Actions m_Actions;

        protected Stat m_Stat;
        protected StatsHandler m_Handler;
        protected Sequence m_Sequence;

        public virtual void Initialize(StatsHandler handler, Stat stat) {
            this.m_Handler = handler;
            this.m_Stat = stat;
            switch (this.m_ValueType)
            {
                case ValueType.Value:
                    stat.onValueChange += OnValueChange;
                    break;
                case ValueType.CurrentValue:
                    if (stat is Attribute attribute)
                    {
                        attribute.onCurrentValueChange += OnCurrentValueChange;
                    }
                    break;
            }
            this.m_Sequence = new Sequence(handler.gameObject, new PlayerInfo("Player"),handler.GetComponent<Blackboard>(), this.m_Actions.actions.ToArray());
            this.m_Handler.onUpdate += Update;
        }

        private void Update() {
            if (this.m_Sequence != null)
            {
                this.m_Sequence.Tick();
            }
        }

        private void OnValueChange()
        {
            if (TriggerCallback(this.m_Stat.Value))
            {
               // Debug.Log("OnValueChange");
                this.m_Sequence.Start();
            }
        }

        private void OnCurrentValueChange()
        {
            if (TriggerCallback((this.m_Stat as Attribute).CurrentValue))
            {
               // Debug.Log("OnCurrentValueChange");
                this.m_Sequence.Start();
            }
        }



        private bool TriggerCallback(float value)
        {
            switch (this.m_Condition)
            {
                case ConditionType.Greater:
                    return value > this.m_Value;
                case ConditionType.GreaterOrEqual:
                    return value >= this.m_Value;
                case ConditionType.Less:
                    return value < this.m_Value;
                case ConditionType.LessOrEqual:
                    return value <= this.m_Value;
            }
            return false;
        }

    }

    public enum ValueType
    {
        Value, CurrentValue
    }

    public enum ConditionType
    {
        Greater,
        GreaterOrEqual,
        Less,
        LessOrEqual,
    }
}