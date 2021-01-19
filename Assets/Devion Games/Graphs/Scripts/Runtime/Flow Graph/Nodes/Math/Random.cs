using UnityEngine;

namespace DevionGames.Graphs
{
    [System.Serializable]
    [ComponentMenu("Math/Random")]
    [NodeStyle( true, "Math")]
    public class Random : FlowNode
    {
        [Input(false,true)]
        public float a;
        [Input(false,true)]
        public float b;
        [Output]
        public float output;

        public override object OnRequestValue(Port port)
        {
            return UnityEngine.Random.Range(GetInputValue("a", a), GetInputValue("b", b));
        }
    }
}
