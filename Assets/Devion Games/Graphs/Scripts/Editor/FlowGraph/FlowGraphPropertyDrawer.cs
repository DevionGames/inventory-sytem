using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DevionGames.Graphs
{
    [CustomPropertyDrawer(typeof(FlowGraph),true)]
    public class FlowGraphPropertyDrawer : GraphPropertyDrawer<FlowGraphView>{}
}