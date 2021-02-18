using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevionGames.UIWidgets;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    public class CraftingTrigger : Trigger
    {
        public override string[] Callbacks
        {
            get
            {
                List<string> callbacks = new List<string>(base.Callbacks);
                callbacks.Add("OnCraftStart");
                callbacks.Add("OnFailedCraftStart");
                callbacks.Add("OnCraftItem");
                callbacks.Add("OnFailedToCraftItem");
                callbacks.Add("OnCraftStop");
                return callbacks.ToArray();
            }
        }

        [Header("Crafting")]
        [SerializeField]
        protected string m_RequiredIngredientsWindow = "Inventory";
        [SerializeField]
        protected string m_ResultStorageWindow = "Inventory";
        [SerializeField]
        protected string m_CraftingProgressbar = "CraftingProgress";

        protected static void Execute(ITriggerFailedCraftStart handler, Item item, GameObject player, FailureCause failureCause)
        {
            handler.OnFailedCraftStart(item, player, failureCause);
        }

        protected static void Execute(ITriggerCraftStart handler, Item item, GameObject player)
        {
            handler.OnCraftStart(item,player);
        }

        protected static void Execute(ITriggerCraftItem handler, Item item, GameObject player)
        {
            handler.OnCraftItem(item, player);
        }

        protected static void Execute(ITriggerFailedToCraftItem handler, Item item, GameObject player, FailureCause failureCause)
        {
            handler.OnFailedToCraftItem(item, player, failureCause);
        }

        protected static void Execute(ITriggerCraftStop handler, Item item, GameObject player)
        {
            handler.OnCraftStop(item, player);
        }

        private ItemContainer m_ResultStorageContainer;
        private ItemContainer m_RequiredIngredientsContainer;
        private bool m_IsCrafting;
        private float m_ProgressDuration;
        private float m_ProgressInitTime;
        private Progressbar m_Progressbar;
        private Spinner m_AmountSpinner;

        private Coroutine coroutine;

        protected override void Start()
        {
            base.Start();
            this.m_ResultStorageContainer = WidgetUtility.Find<ItemContainer>(this.m_ResultStorageWindow);
            this.m_RequiredIngredientsContainer = WidgetUtility.Find<ItemContainer>(this.m_RequiredIngredientsWindow);
            this.m_Progressbar = WidgetUtility.Find<Progressbar>(this.m_CraftingProgressbar);
        }


        public override bool OverrideUse(Slot slot, Item item)
        {

            if (Trigger.currentUsedWindow == item.Container && !slot.MoveItem())
            {
                this.m_AmountSpinner = Trigger.currentUsedWindow.GetComponentInChildren<Spinner>();
                if (this.m_AmountSpinner != null)
                {
                    this.m_AmountSpinner.min = 1;
                    this.m_AmountSpinner.max = int.MaxValue;
                    StartCrafting(item, (int)this.m_AmountSpinner.current);
                }else {
                    StartCrafting(item, 1);
                }
            }
            return true;
        }

        protected override void Update()
        {
            base.Update();
            if (this.m_IsCrafting)
            {
                this.m_Progressbar.SetProgress(GetCraftingProgress());
            }
            
        }

        protected override void OnTriggerInterrupted()
        {
            StopAllCoroutines();
            this.m_IsCrafting = false;
            this.m_Progressbar.SetProgress(0f);
            GameObject user = InventoryManager.current.PlayerInfo.gameObject;
            if (user != null)
                user.SendMessage("SetControllerActive", true, SendMessageOptions.DontRequireReceiver);

            LoadCachedAnimatorStates();
    
        }

        private float GetCraftingProgress()
        {
            if (Time.time - m_ProgressInitTime < m_ProgressDuration)
            {
                return Mathf.Clamp01(((Time.time - m_ProgressInitTime) / m_ProgressDuration));
            }
            return 1f;
        }

        public void StartCrafting(Item item, int amount)
        {
            if (item == null) {
                InventoryManager.Notifications.selectItem.Show();
                ExecuteEvent<ITriggerFailedCraftStart>(Execute, item, FailureCause.FurtherAction);
                return;
            }
            if (this.m_IsCrafting) {
                InventoryManager.Notifications.alreadyCrafting.Show();
                ExecuteEvent<ITriggerFailedCraftStart>(Execute, item, FailureCause.InUse);
                return;
            }

            if (item.UseCraftingSkill)
            {
                ItemContainer skills = WidgetUtility.Find<ItemContainer>(item.SkillWindow);
                if (skills != null)
                {
                    Skill skill = (Skill)skills.GetItems(item.CraftingSkill.Id).FirstOrDefault();
                    if (skill == null)
                    {
                        InventoryManager.Notifications.missingSkillToCraft.Show(item.DisplayName);
                        return;
                    }

                    if (skill.CurrentValue < item.MinCraftingSkillValue)
                    {
                        InventoryManager.Notifications.requiresHigherSkill.Show(item.DisplayName, skill.DisplayName);
                        return;
                    }
                }
                else {
                    Debug.LogWarning("Item is set to use a skill but no skill window with name "+item.SkillWindow+" found!");
                }
            }

            if (!HasIngredients(this.m_RequiredIngredientsContainer, item))
            {
                InventoryManager.Notifications.missingIngredient.Show();
                ExecuteEvent<ITriggerFailedCraftStart>(Execute, item, FailureCause.Requirement);
                return;
            }

            GameObject user = InventoryManager.current.PlayerInfo.gameObject;
            if (user != null)
            {
                //user.transform.LookAt(new Vector3(Trigger.currentUsedTrigger.transform.position.x, user.transform.position.y, Trigger.currentUsedTrigger.transform.position.z));
                user.SendMessage("SetControllerActive", false, SendMessageOptions.DontRequireReceiver);

                Animator animator = InventoryManager.current.PlayerInfo.animator;
                if(animator != null)
                    animator.CrossFadeInFixedTime(Animator.StringToHash(item.CraftingAnimatorState), 0.2f);

            }
            //this.m_RequiredIngredientsContainer.Lock(true);
           coroutine= StartCoroutine(CraftItems(item, amount));
            ExecuteEvent<ITriggerCraftStart>(Execute, item);

        }

        private bool HasIngredients(ItemContainer container, Item item) {
            for (int i = 0; i < item.ingredients.Count; i++)
            {
                if (!container.HasItem(item.ingredients[i].item, item.ingredients[i].amount))
                {
                    return false;
                }
            }
            return true;
        }

        public void StopCrafting(Item item)
        {
            this.m_IsCrafting = false;
            GameObject user = InventoryManager.current.PlayerInfo.gameObject;
            if(user != null)
                user.SendMessage("SetControllerActive", true, SendMessageOptions.DontRequireReceiver);

            LoadCachedAnimatorStates();
            StopCoroutine("CraftItems");
            ExecuteEvent<ITriggerCraftStop>(Execute, item);
            this.m_Progressbar.SetProgress(0f);
        }

        private IEnumerator CraftItems(Item item, int amount)
        {
            this.m_IsCrafting = true;
            for (int i = 0; i < amount; i++)
            {
                if (HasIngredients(this.m_RequiredIngredientsContainer,item))
                {

                    yield return StartCoroutine(CraftItem(item));
                }
                else
                {
                    InventoryManager.Notifications.missingIngredient.Show();
                    ExecuteEvent<ITriggerFailedToCraftItem>(Execute, item, FailureCause.Requirement);
                    break;
                }
            }
            StopCrafting(item);
            this.m_IsCrafting = false;
        }

        private IEnumerator CraftItem(Item item)
        {
            this.m_ProgressDuration = item.CraftingDuration;
            this.m_ProgressInitTime = Time.time;
            yield return new WaitForSeconds(item.CraftingDuration);
            if (item.UseCraftingSkill) {
                ItemContainer skills = WidgetUtility.Find<ItemContainer>(item.SkillWindow);
                Skill skill = (Skill)skills.GetItems(item.CraftingSkill.Id).FirstOrDefault();
                if (skill == null) { Debug.LogWarning("Skill not found in " + item.SkillWindow + "."); }
                if (!skill.CheckSkill()) {
                    InventoryManager.Notifications.failedToCraft.Show(item.DisplayName);
                    if (item.RemoveIngredientsWhenFailed) {
                        for (int i = 0; i < item.ingredients.Count; i++)
                        {
                            this.m_RequiredIngredientsContainer.RemoveItem(item.ingredients[i].item, item.ingredients[i].amount);
                        }
                    }
                    yield break;
                }  

            }

            Item craftedItem = Instantiate(item);
            craftedItem.Stack = 1;
            craftedItem.CraftingModifier.Modify(craftedItem);



            if (this.m_ResultStorageContainer.StackOrAdd(craftedItem))
            {
                for (int i = 0; i < item.ingredients.Count; i++)
                {
                    this.m_RequiredIngredientsContainer.RemoveItem(item.ingredients[i].item, item.ingredients[i].amount);
                }
                InventoryManager.Notifications.craftedItem.Show(UnityTools.ColorString(craftedItem.Name, craftedItem.Rarity.Color));
                ExecuteEvent<ITriggerCraftItem>(Execute, craftedItem);
            }
            else
            {
                InventoryManager.Notifications.containerFull.Show(this.m_ResultStorageContainer.Name);
                ExecuteEvent<ITriggerFailedToCraftItem>(Execute, item, FailureCause.ContainerFull);
                StopCrafting(item);
            }
        }

        protected override void RegisterCallbacks()
        {
            base.RegisterCallbacks();
            this.m_CallbackHandlers.Add(typeof(ITriggerFailedCraftStart), "OnFailedCraftStart");
            this.m_CallbackHandlers.Add(typeof(ITriggerCraftStart), "OnCraftStart");
            this.m_CallbackHandlers.Add(typeof(ITriggerCraftItem), "OnCraftItem");
            this.m_CallbackHandlers.Add(typeof(ITriggerFailedToCraftItem),"OnFailedToCraftItem");
            this.m_CallbackHandlers.Add(typeof(ITriggerCraftStop), "OnCraftStop");
        }
    }
}