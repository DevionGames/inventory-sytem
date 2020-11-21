using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using DevionGames.Graphs;
using System.Linq;

namespace DevionGames.StatSystem
{
	[System.Serializable]
	public class Stat: IJsonSerializable, IBehavior
	{
        [SerializeField]
		private string m_Name = "New Stat";
		public string Name {
			get {
				return this.m_Name;
			}
		}

        private float m_BaseValue = 0f;

        public StatFormula formula;

        private float m_IncrementalValue = 0f;
        public float IncrementalValue {
            get { return this.m_IncrementalValue; }
            set{this.m_IncrementalValue = value;}
        }

        private float m_LastBaseValue;
        private float m_Value;

        public float Value {
			get{
                if (this.m_Dirty || this.m_LastBaseValue != this.m_BaseValue)
                {
                    this.m_LastBaseValue = this.m_BaseValue;
                    this.m_Value = CalculateFinalValue();

                    this.m_Value = Mathf.Clamp(this.m_Value, this.MinValue, this.MaxValue);
                    this.CurrentValue = Mathf.Clamp(this.m_CurrentValue, this.MinValue, this.MaxValue);
                    //GameState.MarkDirty();
                    this.m_Dirty = false;
                }
                return this.m_Value;
            }
		}

        private float m_CurrentValue;
        public float CurrentValue
        {
            get { return this.m_CurrentValue; }
            set
            {
                if (this.m_CurrentValue != value)
                {
                    this.m_CurrentValue = value;
                }
            }
        }

        [SerializeField]
        private float m_MinValue;
        public float MinValue
        {
            get { return this.m_MinValue; }
            set
            {this.m_MinValue = value;}
        }

        private float m_Delta=0f;
        public float NormalizedValue {
            get {
                return (CurrentValue-this.m_Delta) / (Value-this.m_Delta); 
            }
        }

        [SerializeField]
		private float m_MaxValue = -1f;
		public float MaxValue {
			get{
                if (this.m_MaxValue < 0f) {
                    return float.PositiveInfinity;
                }
                return this.m_MaxValue; 
            
            }
			set {this.m_MaxValue = value;}
		}

        [SerializeField]
        private bool m_Regenerate = false;
        public bool Regenerate {
            get { return this.m_Regenerate; }
            set { 
                this.m_Regenerate = value;
                if (this.m_Regenerate)
                    m_Handler.StartCoroutine(Regeneration());
            }
        }

        [SerializeField]
        private float m_Rate = 5f;
        public float Rate {
            get { return this.m_Rate; }
            set { this.m_Rate = value; }
        }

        [SerializeField]
        private bool m_DisplayDamage;
        public bool DisplayDamage
        {
            get { return this.m_DisplayDamage; }
            set { this.m_DisplayDamage=value; }
        }

        [InspectorLabel("Default")]
        [SerializeField]
        private Color m_DamageColor = Color.white;
        public Color DamageColor {
            get { return this.m_DamageColor; }
            set { this.m_DamageColor = value; }
        }

        [InspectorLabel("Critical")]
        [SerializeField]
        private Color m_CriticalDamageColor = Color.red;
        public Color CriticalDamageColor
        {
            get { return this.m_CriticalDamageColor; }
            set { this.m_CriticalDamageColor = value; }
        }

        private StatsHandler m_Handler;
        private List<StatModifier> m_StatModifiers;
        private bool m_Dirty = true;



        public Stat ()
		{
            this.m_StatModifiers = new List<StatModifier>();
        }

		public Stat (Stat other)
		{
			CopyFrom (other);
		}

        public void Initialize(StatsHandler handler) {
            this.m_Handler = handler;
            List<GetStat> statNodes = new List<GetStat>();
            for (int i = 0; i < handler.stats.Count; i++)
            {
                statNodes.AddRange(formula.nodes.Where(x => typeof(GetStat).IsAssignableFrom(x.GetType())).Cast<GetStat>());
            }
            for (int i = 0; i < statNodes.Count; i++)
            {
                statNodes[i].statValue = handler.GetStat(statNodes[i].stat.Trim());
            }
            if(this.m_Regenerate)
                handler.StartCoroutine(Regeneration());
        }

        private IEnumerator Regeneration()
        {
            while (this.m_Regenerate)
            {
                yield return new WaitForSeconds(this.m_Rate);

                float mValue =  CurrentValue;
                float maxValue = Value;

                mValue += 1;
                mValue = Mathf.Clamp(mValue, 0, maxValue);
                CurrentValue = mValue;
                this.m_Handler.UpdateStats();
            }
        }

        public void UpdateBaseValue(StatsHandler handler) {
            StatResult result = formula.nodes.Find(x => x.GetType() == typeof(StatResult)) as StatResult;
            this.m_BaseValue = result.GetInputValue<float>("value", result.value)+this.m_IncrementalValue;
            this.m_Delta = result.GetInputValue<float>("delta", result.delta);
        }

        private float CalculateFinalValue()
        {
            float finalValue = this.m_BaseValue;
            float sumPercentAdd = 0;

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
                        finalValue *= 1 + sumPercentAdd;
                        sumPercentAdd = 0;
                    }
                }
                else if (mod.Type == StatModType.PercentMult)
                {
                    finalValue *= 1 + mod.Value;
                }
            }
           // Debug.Log(Name+": "+finalValue+" "+this.m_BaseValue);
            return (float)Math.Round(finalValue, 4);
        }

        public void AddModifier(StatModifier modifier)
        {
            this.m_Dirty = true;
            this.m_StatModifiers.Add(modifier);
        }

        public bool RemoveModifier(StatModifier modifier)
        {
            if (this.m_StatModifiers.Remove(modifier))
            {
                this.m_Dirty = true;
                return true;
            }
            return false;
        }


        public bool RemoveModifiersFromSource(object source)
        {
            int numRemovals = this.m_StatModifiers.RemoveAll(mod => mod.source == source);

            if (numRemovals > 0)
            {
                this.m_Dirty = true;
                return true;
            }
            return false;
        }

        public void CopyFrom (Stat other)
		{
			this.m_Name = other.Name;
			this.m_MaxValue = other.MaxValue;
			this.m_Value = other.Value;
			this.m_CurrentValue = other.CurrentValue;
            this.m_IncrementalValue = other.m_IncrementalValue;
            this.m_StatModifiers = new List<StatModifier>(other.m_StatModifiers);
		}

		public void Refresh ()
		{
			CurrentValue = Value;
		}

        public void GetObjectData(Dictionary<string, object> data)
        {
            data.Add("Name",this.m_Name);
            data.Add("IncrementalValue", this.m_IncrementalValue);
            data.Add("CurrentValue",this.m_CurrentValue);
        }

        public void SetObjectData(Dictionary<string, object> data)
        {
            this.IncrementalValue = System.Convert.ToSingle(data["IncrementalValue"]); 
            this.m_CurrentValue = System.Convert.ToSingle(data["CurrentValue"]);

        }

        public Graph GetGraph()
        {
            return formula;
        }
	}
}