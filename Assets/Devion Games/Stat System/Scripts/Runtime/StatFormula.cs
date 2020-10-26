using DevionGames.Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.StatSystem
{
    [System.Serializable]
    public class StatFormula : FlowGraph
    {
        public StatFormula()
        {
            GraphUtility.AddNode(this, typeof(StatResult));
        }
    }
}