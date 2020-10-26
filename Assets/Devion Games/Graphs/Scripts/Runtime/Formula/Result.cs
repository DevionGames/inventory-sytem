using UnityEngine;

namespace DevionGames.Graphs
{
    [NodeStyle(false)]
    [System.Serializable]
    public class Result : EventNode
    {
        [Input(false,true)]
        public float result;

        public override object OnRequestValue(Port port)
        {   
            return GetInputValue("result", result);
        }
    }
}