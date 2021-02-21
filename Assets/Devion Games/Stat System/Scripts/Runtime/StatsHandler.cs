using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace DevionGames.StatSystem
{
    public class StatsHandler : MonoBehaviour, IJsonSerializable
    {
        [InspectorLabel("Name")]
        [SerializeField]
        private string m_HandlerName = string.Empty;
        public string HandlerName
        {
            get { return this.m_HandlerName; }
        }
        public bool saveable = false;
        [StatPicker]
        [SerializeField]
        public List<Stat> m_Stats = new List<Stat>();
        [HideInInspector]
        [SerializeField]
        private List<StatOverride> m_StatOverrides = new List<StatOverride>();

        [SerializeField]
        protected List<StatEffect> m_Effects = new List<StatEffect>();
        public System.Action onUpdate;
        private AudioSource m_AudioSource;

        private void Start()
        {
            for (int i = 0; i < this.m_Stats.Count; i++)
                this.m_Stats[i] = Instantiate(this.m_Stats[i]);


            if (this.m_StatOverrides.Count < this.m_Stats.Count)
            {
                for (int i = this.m_StatOverrides.Count; i < this.m_Stats.Count; i++)
                {
                    this.m_StatOverrides.Insert(i, new StatOverride());
                }
            }

            for (int i = 0; i < this.m_Stats.Count; i++)
                this.m_Stats[i].Initialize(this,this.m_StatOverrides[i]);

            for (int i = 0; i < this.m_Stats.Count; i++)
                this.m_Stats[i].ApplyStartValues();

            for (int i = 0; i < this.m_Effects.Count; i++) {
                this.m_Effects[i] = Instantiate(this.m_Effects[i]);
                this.m_Effects[i].Initialize(this);
            }

            if (!string.IsNullOrEmpty(this.m_HandlerName))
            {
                StatsManager.RegisterStatsHandler(this);
            }
            EventHandler.Register<GameObject, Object>(gameObject, "SendDamage", SendDamage);
        }

       /* private void OnGUI()
        {
            for (int i = 0; i < this.m_Stats.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(this.m_Stats[i].ToString());
                if (GUILayout.Button("Add(1)")) {
                    this.m_Stats[i].Add(1);
                }
                if (GUILayout.Button("Subtract(1)"))
                {
                    this.m_Stats[i].Subtract(1);
                }
                if (this.m_Stats[i] is Attribute attribute)
                {
                    if (GUILayout.Button("ApplyDamage(1f)"))
                    {
                        ApplyDamage(this.m_Stats[i].Name, 1f);
                    }
                    if (GUILayout.Button("Heal(1f)"))
                    {
                        ApplyDamage(this.m_Stats[i].Name, -1f);
                    }
                    if (GUILayout.Button("Heal(500f)"))
                    {
                        ApplyDamage(this.m_Stats[i].Name, -500f);
                    }
                }
                GUILayout.EndHorizontal();
            }
        }*/

        private void Update()
        {
            onUpdate?.Invoke();
            for (int i = 0; i < this.m_Effects.Count; i++)
                this.m_Effects[i].Execute();
        }

        public void ApplyDamage(object[] data) {
            string name = (string)data[0];
            float damage = (float)data[1];
            ApplyDamage(name, damage);
        }

        public void ApplyDamage(string name, float damage)
        {
            Attribute stat = GetStat(name) as Attribute;
            if (stat == null) return;

            float currentValue = stat.CurrentValue;
            currentValue = Mathf.Clamp(currentValue - damage, 0f, stat.Value);
            stat.CurrentValue = currentValue;
        }

        protected void TriggerAnimationEvent(AnimationEvent ev)
        {
            if (ev.animatorClipInfo.weight > 0.5f)
                SendMessage(ev.stringParameter, ev.objectReferenceParameter, SendMessageOptions.DontRequireReceiver);
        }

        //Received by animation
        private void SendDamage(Object data)
        {
            DamageData damageData = data as DamageData;
            if (damageData == null) return;
            damageData.sender = gameObject;

            Collider[] colliders = Physics.OverlapSphere(transform.position, damageData.maxDistance);

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].transform != transform)
                {
                    Vector3 direction = colliders[i].transform.position - transform.position;
                    float angle = Vector3.Angle(direction, transform.forward);
                    if (Mathf.Abs(angle) < damageData.maxAngle)
                    {
                        SendDamage(colliders[i].gameObject, damageData);
                    }
                }
            }
        }

        private void SendDamage(GameObject receiver, Object data)
        {
            StatsHandler receiverHandler = receiver.GetComponent<StatsHandler>();
            DamageData damageData = data as DamageData;

            //Exclude self tag, needs some kind of frieds tag system
            if (gameObject.tag == receiver.tag)
                return;

            if (receiverHandler != null && receiverHandler.enabled && damageData != null)
            {
                Stat sendingStat = this.m_Stats.FirstOrDefault(x => x.Name == damageData.sendingStat);
                if (sendingStat == null) return;

                Stat criticalStrikeStat = this.m_Stats.FirstOrDefault(x => x.Name == damageData.criticalStrikeStat);

                bool criticaleStrike = criticalStrikeStat != null && criticalStrikeStat.Value > UnityEngine.Random.Range(0f, 100f);
                sendingStat.CalculateValue();
                float damage = sendingStat.Value;

                if (criticaleStrike)
                    damage *= 2f;

                receiverHandler.ApplyDamage(damageData.receivingStat, damage);
                EventHandler.Execute(receiver, "OnGetHit", gameObject, damageData.receivingStat, damage);

                SendMessage("UseItem", SendMessageOptions.DontRequireReceiver);

                if (damageData.particleEffect != null)
                {
                    Vector3 pos = receiver.GetComponent<Collider>().ClosestPoint(transform.position + damageData.offset);
                    Vector3 right = UnityEngine.Random.Range(-damageData.randomize.x, damageData.randomize.x) * transform.right;
                    Vector3 up = UnityEngine.Random.Range(-damageData.randomize.y, damageData.randomize.y) * transform.up;
                    Vector3 forward = UnityEngine.Random.Range(-damageData.randomize.z, damageData.randomize.z) * transform.forward;

                    Vector3 relativePos = (transform.position + damageData.offset + right + up + forward) - pos;
                    GameObject effect = Instantiate(damageData.particleEffect, pos, Quaternion.LookRotation(relativePos, Vector3.up));
                    Destroy(effect, damageData.lifeTime);
                }
                if(damageData.enableShake)
                    CameraEffects.Shake(damageData.duration, damageData.speed, damageData.amount);

                if (damageData.hitSounds.Length > 0)
                    receiverHandler.PlaySound(damageData.hitSounds[UnityEngine.Random.Range(0, damageData.hitSounds.Length)], damageData.audioMixerGroup, damageData.volumeScale);

                if (damageData.displayDamage)
                    receiverHandler.DisplayDamage(damageData.damagePrefab,damage,criticaleStrike?damageData.criticalDamageColor:damageData.damageColor,damageData.intensity);
               
                if (damageData.enableKnockback && damageData.knockbackChance > Random.Range(0f,1f))
                {
                    StartCoroutine(Knockback(receiver, damageData));
                }

                Animator animator = receiver.GetComponent<Animator>();
                if ( animator != null && animator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion") && !animator.IsInTransition(0))
                {
                    animator.SetTrigger("Hit");
                }
            }
        }

        private IEnumerator Knockback(GameObject receiver, DamageData damageData) {
            NavMeshAgent agent = receiver.GetComponent<NavMeshAgent>();
            if (agent == null) yield break;
          
            Vector3 direction = receiver.transform.position - transform.position;
            float speed = agent.speed;
            float angularSpeed = agent.angularSpeed;
            float acceleration = agent.acceleration;
            agent.speed = damageData.knockbackStrength;
            agent.angularSpeed = 0f;
            agent.acceleration = damageData.knockbackAcceleration;
            agent.SetDestination(receiver.transform.position+direction.normalized);
            yield return new WaitForSeconds(damageData.knockbackAcceleration);
            if (agent == null) yield break;
            agent.speed = speed;
            agent.angularSpeed = angularSpeed;
            agent.acceleration = acceleration;
        }

        

        private void DisplayDamage(GameObject prefab, float damage, Color color, Vector3 intensity)
        {
            Canvas canvas = GetComponentInChildren<Canvas>();
            //TODO Pooling
            if (canvas != null)
            {
                GameObject go = Instantiate(prefab, canvas.transform);
                go.transform.localPosition += new Vector3(UnityEngine.Random.Range(-intensity.x, intensity.x), UnityEngine.Random.Range(-intensity.y, intensity.y), UnityEngine.Random.Range(-intensity.z, intensity.z));
                Text text = go.GetComponentInChildren<Text>();
                text.color = color;
                text.text = (damage > 0 ? "-" : "+") + Mathf.Abs(damage).ToString();

                go.SetActive(true);
                Destroy(go, 4f);
            }
        }

        private void PlaySound(AudioClip clip, AudioMixerGroup audioMixerGroup, float volumeSclae)
        {
            if (this.m_AudioSource == null)
            {
                this.m_AudioSource = gameObject.AddComponent<AudioSource>();
            }
            this.m_AudioSource.outputAudioMixerGroup = audioMixerGroup;
            this.m_AudioSource.spatialBlend = 1f;
            this.m_AudioSource.PlayOneShot(clip, volumeSclae);

        }

        public Stat GetStat(Stat stat)
        {
            return GetStat(stat.Name);
        }
       
        public Stat GetStat(string name)
        {
            return this.m_Stats.Find(x => x.Name == name);
        }

        public bool CanApplyDamage(string name, float damage)
        {
            Attribute stat = GetStat(name) as Attribute;
            if (stat != null)
            {
                if ((damage > 0 && stat.CurrentValue >= damage) || (damage < 0 && stat.CurrentValue < stat.Value))
                {
                    return true;
                }
            }
            return false;
        }

        public void AddEffect(StatEffect effect)
        {
            effect = Instantiate(effect);
            this.m_Effects.Add(effect);
            effect.Initialize(this);
        }

        public void RemoveEffect(StatEffect effect)
        {
            StatEffect instance = this.m_Effects.Find(x => x.Name == effect.Name);
            this.m_Effects.Remove(instance);
        }

        public void AddModifier(object[] data)
        {
            string name = (string)data[0];
            float value = (float)data[1];
            int mod = (int)data[2];
            object source = data[3];
            AddModifier(name, value, (StatModType)mod, source);
        }

        public void AddModifier(string statName, float value, StatModType type, object source)
        {
            Stat stat = GetStat(statName);
            if (stat != null)
            {
                stat.AddModifier(new StatModifier(value, type, source));
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
                return stat.RemoveModifiersFromSource(source);
            }
            return false;
        }

        public float GetStatValue(string name)
        {
            Stat stat = GetStat(name);
            if (stat != null)
                return stat.Value;
            return 0f;
        }

        public float GetStatCurrentValue(string name)
        {
            Stat stat = GetStat(name);
            if (stat != null && stat is Attribute attribute)
                return attribute.CurrentValue;
            return 0f;
        }

        public void GetObjectData(Dictionary<string, object> data)
        {
            data.Add("Name", m_HandlerName);
            List<object> statsList = new List<object>();
            for (int i = 0; i < this.m_Stats.Count; i++)
            {
                Stat stat = this.m_Stats[i];
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
        }
   

    }
}