using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.Graphs
{
    [System.Serializable]
    [ComponentMenu("Math/Pow")]
    [NodeStyle("Icons/Pow",false, "Math")]
    public class Pow : FlowNode
    {
        [Input(false,true)]
        public float f;
        [Input(false, true)]
        public float p;
        [Output]
        public float output;

        public override object OnRequestValue(Port port)
        {
            return Mathf.Pow(GetInputValue("f", f),GetInputValue("p",p));
        }
    }
}