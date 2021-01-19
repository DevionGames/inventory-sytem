using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.Graphs
{
    public interface IGraphProvider 
    {
        Graph GetGraph();
    }
}