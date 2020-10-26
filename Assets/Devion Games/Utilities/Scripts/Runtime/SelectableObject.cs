using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DevionGames
{
    public class SelectableObject : CallbackHandler, ISelectable
    {
        private Transform m_Transform;

        public Vector3 position { get { return this.m_Transform.position; } }

        public override string[] Callbacks => new string[] {"OnSelect","OnDeselect" };

        private void Awake()
        {
            this.m_Transform = transform;    
        }

        public void OnSelect()
        {
            Execute("OnSelect", new CallbackEventData());
        }

        public void OnDeselect()
        {
            Execute("OnDeselect", new CallbackEventData());
        }
    }
}