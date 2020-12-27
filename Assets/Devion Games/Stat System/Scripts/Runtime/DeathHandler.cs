using DevionGames.UIWidgets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DevionGames.StatSystem
{
    [RequireComponent(typeof(StatsHandler))]
    public class DeathHandler : CallbackHandler
    {
        [SerializeField]
        protected string m_StatName = "Health";
        [SerializeField]
        protected string m_DeathState="Death";
        [SerializeField]
        protected string m_RespawnState = "Free Movement";
        [SerializeField]
        protected string m_RespawnTag = "Respawn";
        [SerializeField]
        protected float m_RespawnDelay = 2f;
        [SerializeField]
        protected float m_RespawnDuration = 10f;
        [InspectorLabel("Progressbar")]
        [SerializeField]
        protected string m_ProgressbarName = "General Progressbar";

        protected Stat m_Stat;
        protected Animator m_Animator;
        protected StatsHandler m_Handler;
        protected Progressbar m_Progressbar;
        protected float m_Time;

        public override string[] Callbacks => new string[] { "OnDeath", "OnRespawn" };

        protected virtual void Start()
        {
            this.m_Animator = GetComponent<Animator>();
            this.m_Handler = GetComponent<StatsHandler>();
            this.m_Stat = this.m_Handler.GetStat(this.m_StatName);
            this.m_Progressbar = WidgetUtility.Find<Progressbar>(this.m_ProgressbarName);

            if (this.m_Stat == null){
                Debug.LogWarning("StatsHandler (" + gameObject.name + ") does not contain a Stat with name " + this.m_StatName + ".");
                return;
            }

            this.m_Stat.onChange += OnStatChange;
        }

        protected virtual void OnStatChange(Stat stat) {
            if (stat.CurrentValue == 0f) {
                OnDeath();
            }
        }

        protected virtual void OnDeath() {
            this.m_Animator.CrossFadeInFixedTime(this.m_DeathState, 0.15f);
            this.m_Handler.enabled = false;
            UIWidget.LockAll(true);
            EventHandler.Execute("OnDeath", gameObject);
            Execute("OnDeath", new CallbackEventData());
            StartCoroutine(RespawnTimer());
        }

        private IEnumerator RespawnTimer() {
            yield return new WaitForSeconds(this.m_RespawnDelay);
            this.m_Progressbar.Show("Respawning");

            this.m_Time = 0f;
            while (this.m_Time < this.m_RespawnDuration)
            {
                this.m_Time += Time.deltaTime;
                this.m_Progressbar.SetProgress(this.m_Time / this.m_RespawnDuration);
                yield return null; 
            }
            this.m_Progressbar.Close();
            OnRespawn();

        }

        protected virtual void OnRespawn() {
            GameObject[] respawn = GameObject.FindGameObjectsWithTag(this.m_RespawnTag);
            Transform closestRespawn = GetClosest(respawn.Select(x=>x.transform).ToArray());
            if (closestRespawn != null){
                transform.position = closestRespawn.position;
            }

            Execute("OnRespawn", new CallbackEventData());
            this.m_Handler.enabled = true;
            this.m_Animator.CrossFadeInFixedTime(this.m_RespawnState, 0.15f);
            this.m_Handler.Refresh();
            UIWidget.LockAll(false);
        }

        Transform GetClosest(Transform[] transforms)
        {
            Transform bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;
            foreach (Transform potentialTarget in transforms)
            {
                Vector3 directionToTarget = potentialTarget.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget;
                }
            }

            return bestTarget;
        }
    }
}