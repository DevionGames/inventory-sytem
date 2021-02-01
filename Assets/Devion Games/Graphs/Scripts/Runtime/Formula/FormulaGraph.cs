
namespace DevionGames.Graphs
{
    [System.Serializable]
    public class FormulaGraph : FlowGraph
    {
        public FormulaGraph() {
            GraphUtility.AddNode(this, typeof(FormulaOutput));
        }

        public static implicit operator float(FormulaGraph graph)
        {
            FormulaOutput output = graph.nodes.Find(x => x.GetType() == typeof(FormulaOutput)) as FormulaOutput;
            return output.GetInputValue<float>("result", output.result);
        }
    }
}
