using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DevionGames.Graphs
{
    [System.Serializable]
    public class Graph : ISerializationCallbackReceiver
    {
        public string serializationData;
        [HideInInspector]
        public List<Object> serializedObjects = new List<Object>();
        [System.NonSerialized]
        public List<Node> nodes = new List<Node>();

        public List<T> FindNodesOfType<T>() where T: Node {
           return nodes.Where(x => typeof(T).IsAssignableFrom(x.GetType())).Cast<T>().ToList();
        }

        public void OnBeforeSerialize()
        {
          //  GraphUtility.Save(this);
        }

        public void OnAfterDeserialize()
        {
            GraphUtility.Load(this);
        }
    }
}