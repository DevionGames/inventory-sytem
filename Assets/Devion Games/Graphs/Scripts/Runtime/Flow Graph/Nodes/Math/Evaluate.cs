using UnityEngine;

namespace DevionGames.Graphs
{
    [System.Serializable]
    [ComponentMenu("Math/Evaluate")]
    [NodeStyle(true)]
    public class Evaluate : FlowNode
    {
        [Input(false,true)]
        public float time;
        [Input(false, false)]
        public AnimationCurve curve=new AnimationCurve();
        [Output]
        public float output;

        public override object OnRequestValue(Port port)
        {
            return curve.Evaluate(GetInputValue("time", time)) ;
        }
    }
}
