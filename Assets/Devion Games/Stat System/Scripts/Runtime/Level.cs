using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.StatSystem
{
    [System.Serializable]
    public class Level : Stat
    {
        [StatPicker]
        [SerializeField]
        protected Attribute m_Experience;

        public override void Initialize(StatsHandler handler, StatOverride statOverride)
        {
            base.Initialize(handler, statOverride);
            this.m_Experience = handler.GetStat(this.m_Experience.Name) as Attribute;
            this.m_Experience.onCurrentValueChange += () =>
            {
                if (this.m_Experience.CurrentValue >= this.m_Experience.Value)
                {
                    this.m_Experience.CurrentValue = 0f;
                    Add(1f);
                }
            };
        }
    }
}