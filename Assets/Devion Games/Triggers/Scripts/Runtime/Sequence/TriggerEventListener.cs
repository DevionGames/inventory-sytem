using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    public class TriggerEventListener : MonoBehaviour
    {
        [SerializeField]
        private List<TriggerEvent> m_TriggerEvents = new List<TriggerEvent>();

        [System.Serializable]
        public class TriggerEvent
        {
            public string name = "SFX";
            public ActionTemplate actionTemplate;
            [SerializeReference]
            public List<Action> actions = new List<Action>();
        }
    }
}