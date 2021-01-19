using UnityEngine;

namespace DevionGames.Graphs
{
    [NodeStyle(true)]
    [System.Serializable]
    public class FormulaOutput : EventNode
    {
        [Input(false,true)]
        public float result;

        public override object OnRequestValue(Port port)
        {   
            return GetInputValue("result", result);
        }
    }
}