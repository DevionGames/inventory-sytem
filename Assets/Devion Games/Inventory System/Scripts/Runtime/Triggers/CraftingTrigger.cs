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
        protected string m_IngredientsWindow = "Inventory";
        [SerializeField]
        protected string m_StorageWindow = "Inventory";
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

        protected ItemContainer m_ResultStorageContainer;
        protected ItemContainer m_RequiredIngredientsContainer;
        protected bool m_IsCrafting;
        protected float m_ProgressDuration;
        protected float m_ProgressInitTime;
        protected Progressbar m_Progressbar;
        protected Spinner m_AmountSpinner;

        protected override void Start()
        {
            base.Start();
            this.m_ResultStorageContainer = WidgetUtility.Find<ItemContainer>(this.m_StorageWindow);
            this.m_RequiredIngredientsContainer = WidgetUtility.Find<ItemContainer>(this.m_IngredientsWindow);
            this.m_Progressbar = WidgetUtility.Find<Progressbar>(this.m_CraftingProgressbar);

            ItemContainer container = GetComponent<ItemContainer>();
            if (container != null) {
                container.RegisterListener("OnShow", (CallbackEventData ev) => { InUse = true;  });
                container.RegisterListener("OnClose", (CallbackEventData ev) => { InUse = false; });
            }
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
                NotifyAlreadyCrafting(item);
                return;
            }

            CraftingRecipe recipe = GetCraftingRecipe(item);
            if (recipe == null || !recipe.CheckConditions()) return;

            if (!HasIngredients(item))
            {
                NotifyMissingIngredients(item);
                return;
            }

            GameObject user = InventoryManager.current.PlayerInfo.gameObject;
            if (user != null)
            {
                user.SendMessage("SetControllerActive", false, SendMessageOptions.DontRequireReceiver);
                Animator animator = InventoryManager.current.PlayerInfo.animator;
                if (animator != null)
                    animator.CrossFadeInFixedTime(Animator.StringToHash(recipe.AnimatorState), 0.2f);

            }
            StartCoroutine(CraftItems(item, amount));
            ExecuteEvent<ITriggerCraftStart>(Execute, item);
        }

        private bool HasIngredients(Item item) {
            CraftingRecipe recipe = GetCraftingRecipe(item);
            for (int i = 0; i < recipe.Ingredients.Count; i++)
            {
                if (!this.m_RequiredIngredientsContainer.HasItem(recipe.Ingredients[i].item, recipe.Ingredients[i].amount))
                {
                    return false;
                }
            }
            return true;
        }

        protected virtual CraftingRecipe GetCraftingRecipe(Item item) {
            return item.CraftingRecipe;
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
                if (HasIngredients(item))
                {
                    yield return StartCoroutine(CraftItem(item));
                }
                else
                {
                    NotifyMissingIngredients(item);
                    break;
                }
            }
            StopCrafting(item);
            this.m_IsCrafting = false;
        }

        protected virtual IEnumerator CraftItem(Item item)
        {
            CraftingRecipe recipe = GetCraftingRecipe(item);
            this.m_ProgressDuration = recipe.Duration;
            this.m_ProgressInitTime = Time.time;
            yield return new WaitForSeconds(recipe.Duration);

            if (recipe.Skill != null)
            {
                Skill skill = ItemContainer.GetItem(recipe.Skill.Id) as Skill;
                if (skill != null && !skill.CheckSkill())
                {
                    NotifyFailedToCraft(item, FailureCause.Unknown);
                    if (recipe.RemoveIngredientsWhenFailed)
                    {
                        for (int j = 0; j < recipe.Ingredients.Count; j++)
                        {
                            this.m_RequiredIngredientsContainer.RemoveItem(recipe.Ingredients[j].item, recipe.Ingredients[j].amount);
                        }
                    }
                    yield break;
                }

            }

            Item craftedItem = Instantiate(item);
            craftedItem.Stack = 1;
            recipe.CraftingModifier.Modify(craftedItem);

            if (this.m_ResultStorageContainer.StackOrAdd(craftedItem))
            {
                for (int i = 0; i < recipe.Ingredients.Count; i++)
                {
                    this.m_RequiredIngredientsContainer.RemoveItem(recipe.Ingredients[i].item, recipe.Ingredients[i].amount);
                }
                NotifyItemCrafted(craftedItem);
            }
            else
            {
                NotifyFailedToCraft(item, FailureCause.ContainerFull);
                StopCrafting(item);
            }
        }


        protected virtual void NotifyAlreadyCrafting(Item item) {
            InventoryManager.Notifications.alreadyCrafting.Show();
            ExecuteEvent<ITriggerFailedCraftStart>(Execute, item, FailureCause.InUse);
        }

        protected virtual void NotifyMissingIngredients(Item item) {
            InventoryManager.Notifications.missingIngredient.Show();
            ExecuteEvent<ITriggerFailedCraftStart>(Execute, item, FailureCause.Requirement);
        }

        protected virtual void NotifyItemCrafted(Item item) {
            InventoryManager.Notifications.craftedItem.Show(UnityTools.ColorString(item.Name, item.Rarity.Color));
            ExecuteEvent<ITriggerCraftItem>(Execute, item);
        }

        protected virtual void NotifyFailedToCraft(Item item, FailureCause cause) {
            switch (cause) {
                case FailureCause.ContainerFull:
                    InventoryManager.Notifications.containerFull.Show(this.m_ResultStorageContainer.Name);
                    ExecuteEvent<ITriggerFailedToCraftItem>(Execute, item, FailureCause.ContainerFull);
                    break;
                case FailureCause.Unknown:
                    InventoryManager.Notifications.failedToCraft.Show(item.DisplayName);
                    ExecuteEvent<ITriggerFailedToCraftItem>(Execute, item, FailureCause.Unknown);
                    break;
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