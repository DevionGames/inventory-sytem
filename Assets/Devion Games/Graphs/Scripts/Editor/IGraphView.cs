using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.Graphs
{
    public interface IGraphView 
    {
        void OnGUI(Rect position);
        void CenterGraphView();
    }
}