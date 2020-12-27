using System.Linq;
using UnityEngine;

namespace DevionGames
{
    public class Sequence 
    {
        private ActionStatus m_Status;
        public ActionStatus Status { get => this.m_Status; }

        private int m_ActionIndex;
        private ActionStatus m_ActionStatus;
        private readonly IAction[] m_AllActions;
        private IAction[] m_Actions;

        public Sequence(GameObject gameObject, PlayerInfo playerInfo, Blackboard blackboard, IAction[] actions) {
            this.m_AllActions = actions;
            for (int i = 0; i < this.m_AllActions.Length; i++)
            {
                this.m_AllActions[i].Initialize(gameObject, playerInfo, blackboard);
            }
            this.m_Status = ActionStatus.Inactive;
            this.m_ActionStatus = ActionStatus.Inactive;
        }

        //Starts the task behavior
        public void Start() {
      
            this.m_Actions = this.m_AllActions.Where(x => x.isActiveAndEnabled).ToArray();
            for (int i = 0; i < this.m_Actions.Length; i++) {
                this.m_Actions[i].OnSequenceStart();
            }
            this.m_ActionIndex = 0;
            this.m_Status = ActionStatus.Running;
        }

        public void Stop() {
           
            for (int i = 0; i < this.m_Actions.Length; i++)
            {
                this.m_Actions[i].OnSequenceEnd();
            }
            this.m_Status = ActionStatus.Inactive;
        }

        public void Update()
        {
            for (int i = 0; i < this.m_Actions.Length; i++)
            {
                this.m_Actions[i].Update();
            }
        }

        //Ticks the custom actions using sequence implementation, returns true if the behavior is running.
        public bool Tick()
        {
            if (this.m_Status == ActionStatus.Running)
            {
                if (this.m_ActionIndex >= this.m_Actions.Length)
                {
                    this.m_ActionIndex = 0;
                }

                while (this.m_ActionIndex < this.m_Actions.Length)
                {
                    if (this.m_ActionStatus != ActionStatus.Running)
                    {

                        this.m_Actions[m_ActionIndex].OnStart();
                    }
                    this.m_ActionStatus = this.m_Actions[this.m_ActionIndex].OnUpdate();

                    if (this.m_ActionStatus != ActionStatus.Running)
                    {
                        this.m_Actions[m_ActionIndex].OnEnd();
                    }

                    if (this.m_ActionStatus == ActionStatus.Success)
                    {
                        ++m_ActionIndex;

                    }
                    else
                    {
                        break;
                    }
                }
                this.m_Status = this.m_ActionStatus;
                if (this.m_Status != ActionStatus.Running) {
                    
                    for (int i = 0; i < this.m_Actions.Length; i++)
                    {
                        this.m_Actions[i].OnSequenceEnd();
                    }
                }
            }
            return this.m_Status == ActionStatus.Running;
        }

    }
}