using DevionGames.StatSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.Graphs
{
    [NodeStyle(true, "Stats")]
    [System.Serializable]
    public class GetStatCurrentValue : GetStat
    {

        public override object OnRequestValue(Port port)
        {
            if (statValue == null)
            {
                Debug.LogError("Please ensure a stat named "+stat+" is added to the StatsHandler.");
            }
            return statValue.CurrentValue;
        }
    }
}