using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using DevionGames.Graphs;
using UnityEngine.UI;

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

        [SerializeField]
        private GameObject m_DamageText=null;

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

        protected void TriggerAnimationEvent(AnimationEvent ev)
        {
            SendMessage(ev.stringParameter,ev.objectReferenceParameter, SendMessageOptions.DontRequireReceiver);
        }

        //This is for testing
        private void SendDamage(Object data)
        {
            DamageData damageData = data as DamageData;
            Stat sendingStat = stats.FirstOrDefault(x => x.Name == damageData.sendingStat);
            if (sendingStat == null) return;

            Collider[] colliders = Physics.OverlapSphere(transform.position, damageData.maxDistance);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].transform != transform)
                {
                    Vector3 direction = colliders[i].transform.position - transform.position;
                    float angle = Vector3.Angle(direction, transform.forward);
                    if (Mathf.Abs(angle) < damageData.maxAngle)
                    {
                        StatsHandler receiver = colliders[i].GetComponent<StatsHandler>();
                        if(receiver != null)
                        {
                            Stat criticalStrikeStat = stats.FirstOrDefault(x => x.Name == damageData.criticalStrikeStat);

                            bool criticaleStrike = criticalStrikeStat!= null && criticalStrikeStat.Value > UnityEngine.Random.Range(0f, 100f);
                            float damage = sendingStat.Value;
                            if (criticaleStrike)
                                damage *= 2f;

                            receiver.ApplyDamageInternal(damageData.receivingStat, damage, 0, criticaleStrike);
                            if (damageData.particleEffect != null)
                            {
                                Vector3 pos = colliders[i].ClosestPoint(transform.position + damageData.offset);
                                Vector3 right = UnityEngine.Random.Range(-damageData.randomize.x, damageData.randomize.x)*transform.right;
                                Vector3 up = UnityEngine.Random.Range(-damageData.randomize.y, damageData.randomize.y) * transform.up ;
                                Vector3 forward = UnityEngine.Random.Range(-damageData.randomize.z, damageData.randomize.z)*transform.forward;

                                Vector3 relativePos = (transform.position + damageData.offset + right + up + forward) - pos;
                                GameObject effect = Instantiate(damageData.particleEffect, pos, Quaternion.LookRotation(relativePos, Vector3.up));
                                Destroy(effect, damageData.lifeTime);
                            }

                            CameraEffects.Shake(damageData.duration, damageData.speed, damageData.amount);
                            if(damageData.hitSounds.Length > 0)
                                UnityTools.PlaySound(damageData.hitSounds[UnityEngine.Random.Range(0,damageData.hitSounds.Length)],damageData.volume);
                        }
                       
                    }
                }
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

        private void ApplyDamageInternal(string name, float damage, int valueType, bool isCriticalStrike = false)
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
                if (stat.DisplayDamage)
                    DisplayDamage(damage, isCriticalStrike? stat.CriticalDamageColor:stat.DamageColor);

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

        private void DisplayDamage(float damage, Color color) {
            //TODO Pooling
            GameObject go = Instantiate(this.m_DamageText, this.m_DamageText.transform.parent);
            Vector3 randomizeIntensity = new Vector3(3f,2f,0f);
            go.transform.localPosition += new Vector3(UnityEngine.Random.Range(-randomizeIntensity.x,randomizeIntensity.x), UnityEngine.Random.Range(-randomizeIntensity.y, randomizeIntensity.y), UnityEngine.Random.Range(-randomizeIntensity.z, randomizeIntensity.z));
            Text text = go.GetComponentInChildren<Text>();
            text.color = color;
            text.text = (damage>0?"-":"+")+Mathf.Abs(damage).ToString();

            go.SetActive(true);
            Destroy(go, 4f);
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