using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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