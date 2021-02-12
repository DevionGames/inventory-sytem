using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    [CreateAssetMenu(fileName = "ActionTemplate", menuName = "Devion Games/Triggers/Action Template")]
    [System.Serializable]
    public class ActionTemplate : ScriptableObject
    {
        [SerializeReference]
        public List<Action> actions= new List<Action>();
    }
}