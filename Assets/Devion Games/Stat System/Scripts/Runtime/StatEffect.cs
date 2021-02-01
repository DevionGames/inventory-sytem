using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.StatSystem
{
    [System.Serializable]
    public class StatEffect : ScriptableObject, INameable
    {
        [InspectorLabel("Name")]
        [SerializeField]
        protected string m_StatEffectName="New Effect";
        public string Name { get => this.m_StatEffectName; set => this.m_StatEffectName = value; }
        [SerializeField]
        protected int m_Repeat = -1;

        [SerializeReference]
        protected List<Action> m_Actions = new List<Action>();

        protected Sequence m_Sequence;
        [System.NonSerialized]
        protected int m_CurrentRepeat = 0;
        protected StatsHandler m_Handler;

        public void Initialize(StatsHandler handler)
        {
            this.m_Handler = handler;
            this.m_Sequence = new Sequence(handler.gameObject, new PlayerInfo("Player"), handler.GetComponent<Blackboard>(), this.m_Actions.ToArray());
            this.m_Sequence.Start();
           
        }

        public void Execute() {
            if (!this.m_Sequence.Tick()) {
                this.m_Sequence.Stop();
                this.m_Sequence.Start();
                this.m_CurrentRepeat += 1;
            }
            this.m_Sequence.Update();

            if (this.m_Repeat > 0 && this.m_CurrentRepeat >= this.m_Repeat)
               this.m_Handler.RemoveEffect(this);
            
        }
    }
}