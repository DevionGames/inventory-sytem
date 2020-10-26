
namespace DevionGames.Graphs
{
    [System.Serializable]
    public class FormulaGraph : FlowGraph
    {
        public FormulaGraph() {
            GraphUtility.AddNode(this, typeof(Result));
        }
    }
}
