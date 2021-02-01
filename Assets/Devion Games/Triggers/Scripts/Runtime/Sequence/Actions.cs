using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    [System.Serializable]
    public class Actions 
    {
        [SerializeReference]
        public List<Action> actions = new List<Action>();


    }
}