using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.Graphs
{
    [System.Serializable]
    public abstract class Node
    {
        [HideInInspector]
        public string id;
        public string name;
        [HideInInspector]
        public Vector2 position;
        [System.NonSerialized]
        public Graph graph;

        public Node()
        {
            id = System.Guid.NewGuid().ToString();
        }

        public virtual void OnAfterDeserialize() { }
        public virtual void OnBeforeSerialize() { }
    }
}