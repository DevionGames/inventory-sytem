using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [System.Serializable]
    public abstract class Action : IAction
    {
        [HideInInspector]
        [SerializeField]
        private string m_Type;

        protected PlayerInfo playerInfo;
        protected GameObject gameObject;

        [HideInInspector]
        [SerializeField]
        private bool m_Enabled = true;
        public bool enabled {
            get { return this.m_Enabled; }
            set { this.m_Enabled = value; }
        }

        public bool isActiveAndEnabled { get { return enabled && gameObject.activeSelf; } }

        public Action() {
            this.m_Type = GetType().FullName;
        }

        public void Initialize(GameObject gameObject, PlayerInfo playerInfo) {
            this.gameObject = gameObject;
            this.playerInfo = playerInfo;
        }

        public abstract ActionStatus OnUpdate();

        public virtual void OnStart(){}

        public virtual void OnEnd(){}

        public virtual void OnSequenceStart(){}

        public virtual void OnSequenceEnd(){}

        protected GameObject GetTarget(TargetType type)
        {
            return type == TargetType.Player ? playerInfo.gameObject : gameObject;
        }
    }

    public enum TargetType { 
        Self,
        Player
    }
}
