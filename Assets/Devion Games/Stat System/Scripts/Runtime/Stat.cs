using DevionGames.Graphs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.StatSystem
{
    [System.Serializable]
    public class Stat : ScriptableObject, INameable, IGraphProvider, IJsonSerializable
    {
        public System.Action onValueChange;
        private System.Action onValueChangeInternal;

        [InspectorLabel("Name")]
        [SerializeField]
        protected string m_StatName = "New Stat";
        public string Name { get => this.m_StatName; set => this.m_StatName=value; }

        [SerializeField]
        protected float m_BaseValue;
       /* [HideInInspector]
        [SerializeField]
        protected bool m_InheritBaseValue = true;
        [InspectorLabel("Base Value")]
        [HideInInspector]
        [SerializeField]
        protected float m_OverrideBaseValue;*/

        [SerializeField]
        protected FormulaGraph m_FormulaGraph;
        [SerializeField]
        protected float m_Cap = -1;
        [SerializeReference]
        protected List<StatCallback> m_Callbacks = new List<StatCallback>();

        [System.NonSerialized]
        protected float m_Value;
        public float Value { get => this.m_Value; }

        protected List<StatModifier> m_StatModifiers= new List<StatModifier>();
        protected StatsHandler m_StatsHandler;

        public virtual void Initialize(StatsHandler handler, StatOverride statOverride)
        {
            this.m_StatsHandler = handler;
            
            if (statOverride.overrideBaseValue)
                this.m_BaseValue = statOverride.baseValue;

            List<StatNode> statNodes = this.m_FormulaGraph.FindNodesOfType<StatNode>();

            for (int i = 0; i < statNodes.Count; i++)
            {
                Stat referencedStat = handler.GetStat(statNodes[i].stat.Trim());
                statNodes[i].statValue = referencedStat;
                referencedStat.onValueChangeInternal += CalculateValue;
            }
        
            for (int i = 0; i < this.m_Callbacks.Count; i++)
            {
                this.m_Callbacks[i].Initialize(handler, this);
            }
        }

        public virtual void ApplyStartValues() {
            CalculateValue();
        }

        public void Add(float amount) {
            this.m_BaseValue += amount;
            this.m_BaseValue = Mathf.Clamp(this.m_BaseValue, 0, float.MaxValue);
            CalculateValue();
        }

        public void Subtract(float amount) {
            this.m_BaseValue -= amount;
            this.m_BaseValue = Mathf.Clamp(this.m_BaseValue, 0, float.MaxValue);
            CalculateValue();
        }

        public void CalculateValue() {
            CalculateValue(true);
        }

        public void CalculateValue(bool invokeCallbacks) {
            float finalValue = this.m_BaseValue + this.m_FormulaGraph;
            float sumPercentAdd = 0f;
            this.m_StatModifiers.Sort((x, y) => x.Type.CompareTo(y.Type));

            for (int i = 0; i < this.m_StatModifiers.Count; i++)
            {
                StatModifier mod = this.m_StatModifiers[i];
                if (mod.Type == StatModType.Flat)
                {
                    finalValue += mod.Value;
                }
                else if (mod.Type == StatModType.PercentAdd)
                {
                    sumPercentAdd += mod.Value;

                    if (i + 1 >= this.m_StatModifiers.Count || this.m_StatModifiers[i + 1].Type != StatModType.PercentAdd)
                    {
                        finalValue *= 1f + sumPercentAdd;
                        sumPercentAdd = 0f;
                    }
                }
                else if (mod.Type == StatModType.PercentMult)
                {
                    finalValue *= 1f + mod.Value;
                }
            }
            if (this.m_Cap >= 0)
                finalValue = Mathf.Clamp(finalValue, 0, this.m_Cap);



            if (this.m_Value != finalValue)
            {
                this.m_Value = finalValue;
                if(invokeCallbacks)
                    onValueChange?.Invoke();

                onValueChangeInternal?.Invoke();
            }
        }

        public void AddModifier(StatModifier modifier)
        {
            this.m_StatModifiers.Add(modifier);
            CalculateValue();
        }

        public bool RemoveModifier(StatModifier modifier)
        {
            if (this.m_StatModifiers.Remove(modifier))
            {
                CalculateValue();
                return true;
            }
            return false;
        }

        public bool RemoveModifiersFromSource(object source)
        {
            int numRemovals = this.m_StatModifiers.RemoveAll(mod => mod.source == source);

            if (numRemovals > 0)
            {
                CalculateValue();
                return true;
            }
            return false;
        }

        public Graph GetGraph()
        {
            return this.m_FormulaGraph;
        }

        public override string ToString()
        {
            return this.m_StatName+": "+this.Value.ToString();
        }

        public virtual void GetObjectData(Dictionary<string, object> data)
        {
            data.Add("Name", this.m_StatName);
            data.Add("BaseValue", this.m_BaseValue);
        }

        public virtual void SetObjectData(Dictionary<string, object> data)
        {
            this.m_BaseValue = System.Convert.ToSingle(data["BaseValue"]);
            CalculateValue(false);
        }
    }

    [System.Serializable]
    public class StatOverride
    {
        public bool overrideBaseValue = false;
        public float baseValue;

    }
}