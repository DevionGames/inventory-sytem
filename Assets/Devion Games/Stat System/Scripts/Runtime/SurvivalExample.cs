using DevionGames.StatSystem;
using System.Collections;
using UnityEngine;

public class SurvivalExample : MonoBehaviour
{
    [SerializeField]
    protected string m_SurvivalStat = "Hunger";
    [SerializeField]
    protected float m_SurvivalDamage = 1f;
    [SerializeField]
    protected float m_SurvivalRate = 3f;

    [SerializeField]
    protected string m_HealthStat = "Health";
    [SerializeField]
    protected float m_HealthDamage = 1f;
    [SerializeField]
    protected float m_HealthRate = 2f;


    protected StatsHandler m_Handler;
    protected Stat m_Stat;

    protected virtual void Start()
    {
        this.m_Handler = GetComponent<StatsHandler>();
        this.m_Stat = this.m_Handler.GetStat(this.m_SurvivalStat);

        if (this.m_Stat == null)
        {
            Debug.LogWarning("StatsHandler (" + gameObject.name + ") does not contain a Stat with name " + this.m_SurvivalStat + ".");
            return;
        }

        this.m_Stat.onChange += OnStatChange;
        StartCoroutine(ApplySurvivalDamage());
    }

    protected virtual void OnStatChange(Stat stat)
    {
        if (stat.CurrentValue == 0f)
        {
            StartCoroutine(ApplyHealthDamage());
        }
    }

    protected virtual IEnumerator ApplySurvivalDamage()
    {
        while (true)
        {
            yield return new WaitForSeconds(this.m_SurvivalRate);
            this.m_Handler.ApplyDamage(this.m_SurvivalStat, this.m_SurvivalDamage);
        }
    }

    protected virtual IEnumerator ApplyHealthDamage()
    {
        while (this.m_Stat.CurrentValue <= 0f)
        {
            yield return new WaitForSeconds(this.m_SurvivalRate);
            this.m_Handler.ApplyDamage(this.m_SurvivalStat, this.m_SurvivalDamage);
        }
    }
}
