using UnityEngine;

namespace DevionGames.Graphs
{
    [ComponentMenu("Stat System/Get Stat Current Value")]
    [System.Serializable]
    public class GetStatCurrentValue : StatNode
    {
        public override object OnRequestValue(Port port)
        {
            if (statValue == null)
            {
                Debug.LogError("Please ensure a stat named " + stat + " is added to the StatsHandler.");
            }
            return ((StatSystem.Attribute)statValue).CurrentValue;
        }
    }
}