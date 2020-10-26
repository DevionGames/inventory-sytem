using UnityEngine;

namespace DevionGames.Graphs
{
    [NodeStyle("Icons/Multiply",false,"Math")]
    public class Multiply : FlowNode
    {
        [Input(false, true)]
        public float a;
        [Input(false, true)]
        public float b;
        [Output]
        public float output;

        public override object OnRequestValue(Port port)
        {
            return GetInputValue("a", a) * GetInputValue("b", b);
        }
    }
}