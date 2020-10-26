using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.StatSystem
{
    public class ApplyDamageOverTime : MonoBehaviour
    {
        [SerializeField]
        private string m_StatName="Health";
        [SerializeField]
        private float m_Damage = -1;
        [SerializeField]
        private int m_ValueType = 0;
        [SerializeField]
        private float m_Rate = 1.5f;
        private StatsHandler m_Handler;

        private void Start()
        {
            this.m_Handler = GetComponent<StatsHandler>();
            StartCoroutine(ApplyDamage());
        }

        private IEnumerator ApplyDamage() {
            while (true)
            {
                yield return new WaitForSeconds(this.m_Rate);
                this.m_Handler.ApplyDamage(this.m_StatName, this.m_Damage, this.m_ValueType);
            }
        }
    }
}