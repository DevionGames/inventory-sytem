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
        [HideInInspector]
        [SerializeField]
        private bool m_Enabled = true;
        public bool enabled {
            get { return this.m_Enabled; }
            set { this.m_Enabled = value; }
        }

        public bool isActiveAndEnabled { get { return enabled && gameObject.activeSelf; } }

        protected PlayerInfo playerInfo;
        protected GameObject gameObject;
        protected Blackboard blackboard;

        public Action() {
            this.m_Type = GetType().FullName;
        }

        public void Initialize(GameObject gameObject, PlayerInfo playerInfo, Blackboard blackboard) {
            this.gameObject = gameObject;
            this.playerInfo = playerInfo;
            this.blackboard = blackboard; 
        }

        public abstract ActionStatus OnUpdate();

        public virtual void Update() { }

        public virtual void OnStart(){}

        public virtual void OnEnd(){}

        public virtual void OnSequenceStart(){}

        public virtual void OnSequenceEnd(){}

        public virtual void OnInterrupt() { }

        protected GameObject GetTarget(TargetType type)
        {
            switch (type)
            {
                case TargetType.Player:
                    return playerInfo.gameObject;
                case TargetType.Camera:
                    return Camera.main.gameObject;
            }
            return gameObject;
        }
    }

    public enum TargetType { 
        Self,
        Player,
        Camera
    }
}
