using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    public class BehaviorTrigger : BaseTrigger
    {
        public ActionTemplate actionTemplate;
        //Actions to run when the trigger is used.

        [SerializeReference]
        public List<Action> actions = new List<Action>();
        [SerializeField]
        protected bool m_Interruptable=false;

        //Task behavior that runs custom actions
        private Sequence m_ActionBehavior;

        protected AnimatorStateInfo[] m_LayerStateMap;

        private PlayerInfo m_PlayerInfo;

        public override PlayerInfo PlayerInfo {
            get { 
                if (this.m_PlayerInfo == null) {
                    this.m_PlayerInfo = new PlayerInfo("Player");
                }
                return this.m_PlayerInfo;
            }
        }

        protected override void Start()
        {
            base.Start();
            List<ITriggerEventHandler> list = new List<ITriggerEventHandler>(this.m_TriggerEvents);
            list.AddRange(actions.Where(x => x is ITriggerEventHandler).Cast<ITriggerEventHandler>());
            this.m_TriggerEvents = list.ToArray();
            if(actionTemplate != null)
                actionTemplate = Instantiate(actionTemplate);
            this.m_ActionBehavior = new Sequence(gameObject, PlayerInfo, GetComponent<Blackboard>(), actionTemplate != null? actionTemplate.actions.ToArray() : actions.ToArray());
        }

        //Called once per frame
        protected override void Update()
        {
            if (!InRange) { return; }

            //Check for key down and if trigger input type supports key.
            if (Input.GetKeyDown(key) && triggerType.HasFlag<TriggerInputType>(TriggerInputType.Key) && InRange && IsBestTrigger())
            {
                Use();
            }
            if (this.m_Interruptable && this.InUse && (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.5f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.5f))
            {
                NotifyInterrupted();
                this.m_ActionBehavior.Interrupt();  
                return;
            }
            //Update task behavior, set in use if it is running
            this.InUse = this.m_ActionBehavior.Tick();

        }

        protected override void OnDisable()
        {
            if (Time.frameCount > 0)
            {
                if (this.m_Interruptable && this.InUse) {
                    NotifyInterrupted();
                    this.m_ActionBehavior.Interrupt();
                }
                this.InRange = false;
            }
        }

        protected override void OnDestroy()
        {

            if (Time.frameCount > 0)
            {
                if (this.m_Interruptable && this.InUse)
                {
                    NotifyInterrupted();
                    this.m_ActionBehavior.Interrupt();
                }
                this.InRange = false;
            }
        }

        protected void NotifyInterrupted() {
            this.InUse = false;
          //  NotifyUnUsed();
            OnTriggerInterrupted();
        }

        protected virtual void OnTriggerInterrupted() { 
        
        }

        protected override void OnTriggerUsed()
        {
            CacheAnimatorStates();
        }

        protected override void OnTriggerUnUsed()
        {
            this.m_ActionBehavior.Stop();
            LoadCachedAnimatorStates();
        }

        //Use the trigger
        public override bool Use()
        {
            //Can the trigger be used?
            if (!CanUse())
            {
                return false;
            }
            //Trigger.currentUsedTrigger = this;
            //Set the trigger in use
            this.InUse = true;
            this.m_ActionBehavior.Start();
            return true;
        }

        protected void CacheAnimatorStates()
        {
            if (PlayerInfo == null) return;

            Animator animator = PlayerInfo.animator;
            if (animator != null)
            {
                this.m_LayerStateMap = new AnimatorStateInfo[animator.layerCount];
                for (int j = 0; j < animator.layerCount; j++)
                {
                    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(j);
                    this.m_LayerStateMap[j] = stateInfo;
                }
            }
        }

        protected void LoadCachedAnimatorStates()
        {
            if (PlayerInfo == null) return;

            Animator animator = PlayerInfo.animator;
            if (animator != null)
            {
                for (int j = 0; j < this.m_LayerStateMap.Length; j++)
                {
                    if (animator.GetCurrentAnimatorStateInfo(j).shortNameHash != this.m_LayerStateMap[j].shortNameHash && !animator.IsInTransition(j))
                    {
                        animator.CrossFadeInFixedTime(this.m_LayerStateMap[j].shortNameHash, 0.15f);
                    }
                }
            }
        }

    }
}