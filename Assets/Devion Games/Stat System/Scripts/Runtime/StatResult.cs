using DevionGames.Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.StatSystem
{
    [NodeStyle(false)]
    [System.Serializable]
    public class StatResult : EventNode
    {
        [Input(true,true)]
        public float value;
        [Input(true,true)]
        public float delta;
        public override object OnRequestValue(Port port)
        {
            return 0;
        }
    }
}