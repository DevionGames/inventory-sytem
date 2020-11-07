using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using DevionGames.Graphs;

namespace DevionGames.StatSystem
{
	public class StatsHandler : MonoBehaviour, IJsonSerializable
    {
        [InspectorLabel("Name")]
        [SerializeField]
        private string m_HandlerName = string.Empty;
        public string HandlerName {
            get { return this.m_HandlerName; }
        }
        public bool saveable = false;
        public float freeStatPoints = 3;

		public List<Stat> stats = new List<Stat> ();

        private void Awake()
        {
            for (int i = 0; i < stats.Count; i++)
            {
                stats[i].Initialize(this);
            }
        }

        private void Start()
        {
            if (!string.IsNullOrEmpty(this.m_HandlerName))
            {
                StatsManager.RegisterStatsHandler(this);
            }
            UpdateStats();
            Refresh();
        }

        public void UpdateStats()
        {
            for (int i = 0; i < stats.Count; i++)
            {
                Stat stat = stats[i];
                stat.UpdateBaseValue(this);
            }
        }



        /// <summary>
        /// Apply damage to stat, negative values heal.
        /// Type:
        /// 0: CurrentValue
        /// 1: IncrementalValue
        /// </summary>
        /// <param name="data"></param>
        public void ApplyDamage(object[] data)
        {
            string name = (string)data[0];
            float damage = (float)data[1];
            int valueType = 0;
            if (data.Length > 2)
                valueType = (int)data[2];

            ApplyDamage(name, damage, valueType);
        }

        public void ApplyDamage(string name, float damage)
        {
            ApplyDamageInternal(name, damage, 0);
        }

        public void ApplyDamage(string name, float damage, int valueType)
        {
            ApplyDamageInternal(name, damage, valueType);
        }

        private void ApplyDamageInternal(string name, float damage, int valueType)
        {
            Stat stat = GetStat(name);
            if (stat != null)
            {
                float mValue = valueType==0?stat.CurrentValue:stat.IncrementalValue;
                float maxValue = valueType==0?stat.Value:stat.MaxValue;
              
                mValue -= damage;
                mValue = Mathf.Clamp(mValue, 0, maxValue);
                switch (valueType) {
                    case 0:
                        stat.CurrentValue = mValue;
                        break;
                    case 1:
                        stat.IncrementalValue = mValue;
                        break;
                }
                UpdateStats();
                if (StatsManager.DefaultSettings.debugMessages && mValue < maxValue)
                {
                    Debug.Log("[StatusSystem]Apply Damage: " + gameObject.name + " " + stat.Name + " CurrentValue:" + stat.CurrentValue + " Value:" + stat.Value +" IncrementalValue: "+stat.IncrementalValue +" Damage: " + damage);
                }
            }
        }

        public bool CanApplyDamage(string name, float damage)
        {
            return CanApplyDamageInternal(name, damage);
        }

        private bool CanApplyDamageInternal(string name, float damage)
        {
            Stat stat = GetStat(name);
            if (stat != null)
            {
                if ((damage > 0 && stat.CurrentValue >= damage) || (damage < 0 && stat.CurrentValue < stat.Value)) {
                    return true;
                }
            }
            return false;
        }

        public void AddModifier(object[] data)
        {
            string name = (string)data[0];
            float value = (float)data[1];
            int mod = (int)data[2];
            object source = data[3];
            AddModifier(name, value,(StatModType)mod, source);
        }

        public void AddModifier(string statName, float value, StatModType type, object source)
        {
            Stat stat = GetStat(statName);
            if (stat != null) {
                stat.AddModifier(new StatModifier(value, type, source));
                UpdateStats();
            }
        }

        public bool RemoveModifiersFromSource(object[] data)
        {
            string name = (string)data[0];
            object source = data[1];
            return RemoveModifiersFromSource(name, source);
        }

        public bool RemoveModifiersFromSource(string statName, object source)
        {
            Stat stat = GetStat(statName);
            if (stat != null)
            {
                bool result = stat.RemoveModifiersFromSource(source);
                UpdateStats();
                return result;
            }
            return false;
        }

        public Stat GetStat (string name)
		{
			for (int i = 0; i < stats.Count; i++) {
				Stat stat = stats [i];
				if (stat.Name == name) {
					return stat;
				}
			}
			return null;
		}

        public float GetStatValue(string name) {
            Stat stat = GetStat(name);
            if (stat != null)
                return stat.Value;
            return 0f;
        }

        public float GetStatCurrentValue(string name)
        {
            Stat stat = GetStat(name);
            if (stat != null)
                return stat.CurrentValue;
            return 0f;
        }

        public void Refresh ()
		{
			for (int i = 0; i < stats.Count; i++) {
				Stat stat = stats [i];
				stat.Refresh ();
			}
		}

        public void GetObjectData(Dictionary<string, object> data)
        {
            data.Add("Name", m_HandlerName);
            data.Add("FreeStatPoints",freeStatPoints);
            List<object> statsList= new List<object>();
            for (int i = 0; i < stats.Count; i++)
            {
                Stat stat = stats[i];
                if (stat != null)
                {
                    Dictionary<string, object> statData = new Dictionary<string, object>();
                    stat.GetObjectData(statData);
                    statsList.Add(statData);
                }

            }
            data.Add("Stats", statsList);
        }

        public void SetObjectData(Dictionary<string, object> data)
        {
            freeStatPoints = System.Convert.ToSingle(data["FreeStatPoints"]);
            if (data.ContainsKey("Stats"))
            {
                List<object> statList = data["Stats"] as List<object>;
                for (int i = 0; i < statList.Count; i++)
                {
                    Dictionary<string, object> statData = statList[i] as Dictionary<string, object>;
                    if (statData != null)
                    {
                        Stat stat = GetStat((string)statData["Name"]);
                       
                        if (stat != null)
                        {
                            stat.SetObjectData(statData);
                        }
                    }
                }
            }
            UpdateStats();
        }
    }
}