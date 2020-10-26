using DevionGames.StatSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.Graphs
{
    [NodeStyle(true, "Stats")]
    [System.Serializable]
    public abstract class GetStat: FlowNode
    {
        [Input(true,false)]
        public string stat =string.Empty;
        [Output]
        public Stat statValue;

        public GetStat() { }
    }
}