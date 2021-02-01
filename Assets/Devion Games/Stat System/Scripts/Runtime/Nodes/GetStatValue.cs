using UnityEngine;

namespace DevionGames.Graphs
{
    [ComponentMenu("Stat System/Get Stat Value")]
    [System.Serializable]
    public class GetStatValue : StatNode
    {
        public override object OnRequestValue(Port port)
        {
            if (statValue == null)
            {
                Debug.LogError("Please ensure a stat named " + stat + " is added to the StatsHandler.");
            }
            return statValue.Value;
        }
    }
}