using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.Graphs
{
    [System.Serializable]
    [ComponentMenu("Math/Round")]
    [NodeStyle("Icons/Round",false, "Math")]
    public class Round : FlowNode
    {
        [Input(false,true)]
        public float value;
        [Output]
        public float output;

        public override object OnRequestValue(Port port)
        {
            return Mathf.Round(GetInputValue("value", value));
        }
    }
}