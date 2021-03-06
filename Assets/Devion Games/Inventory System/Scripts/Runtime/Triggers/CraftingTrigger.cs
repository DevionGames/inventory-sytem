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

        private ItemContainer m_ResultStorageContainer;
        private ItemContainer m_RequiredIngredientsContainer;
        private bool m_IsCrafting;
        private float m_ProgressDuration;
        private float m_ProgressInitTime;
        private Progressbar m_Progressbar;
        private Spinner m_AmountSpinner;



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
                InventoryManager.Notifications.alreadyCrafting.Show();
                ExecuteEvent<ITriggerFailedCraftStart>(Execute, item, FailureCause.InUse);
                return;
            }

            if (item.CraftingRecipe == null || !item.CraftingRecipe.CheckConditions()) return;

            if (!HasIngredients(this.m_RequiredIngredientsContainer, item.CraftingRecipe))
            {
                InventoryManager.Notifications.missingIngredient.Show();
                ExecuteEvent<ITriggerFailedCraftStart>(Execute, item, FailureCause.Requirement);
                return;
            }

            GameObject user = InventoryManager.current.PlayerInfo.gameObject;
            if (user != null)
            {
                user.SendMessage("SetControllerActive", false, SendMessageOptions.DontRequireReceiver);

                Animator animator = InventoryManager.current.PlayerInfo.animator;
                if(animator != null)
                    animator.CrossFadeInFixedTime(Animator.StringToHash(item.CraftingRecipe.AnimatorState), 0.2f);

            }
            StartCoroutine(CraftItems(item, amount));
            ExecuteEvent<ITriggerCraftStart>(Execute, item);

        }

        private bool HasIngredients(ItemContainer container, CraftingRecipe recipe) {
            
            for (int i = 0; i < recipe.Ingredients.Count; i++)
            {
                if (!container.HasItem(recipe.Ingredients[i].item, recipe.Ingredients[i].amount))
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
                if (HasIngredients(this.m_RequiredIngredientsContainer,item.CraftingRecipe))
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
            this.m_ProgressDuration = item.CraftingRecipe.Duration;
            this.m_ProgressInitTime = Time.time;
            yield return new WaitForSeconds(item.CraftingRecipe.Duration);

            if (item.CraftingRecipe.Skill != null)
            {
                Skill skill = ItemContainer.GetItem(item.CraftingRecipe.Skill.Id) as Skill;
                if (skill != null && !skill.CheckSkill())
                {
                    InventoryManager.Notifications.failedToCraft.Show(item.DisplayName);
                    if (item.CraftingRecipe.RemoveIngredientsWhenFailed)
                    {
                        for (int j = 0; j < item.CraftingRecipe.Ingredients.Count; j++)
                        {
                            this.m_RequiredIngredientsContainer.RemoveItem(item.CraftingRecipe.Ingredients[j].item, item.CraftingRecipe.Ingredients[j].amount);
                        }
                    }
                    yield break;
                }

            }

            Item craftedItem = Instantiate(item);
            craftedItem.Stack = 1;
            craftedItem.CraftingRecipe.CraftingModifier.Modify(craftedItem);

            if (this.m_ResultStorageContainer.StackOrAdd(craftedItem))
            {
                for (int i = 0; i < item.CraftingRecipe.Ingredients.Count; i++)
                {
                    this.m_RequiredIngredientsContainer.RemoveItem(item.CraftingRecipe.Ingredients[i].item, item.CraftingRecipe.Ingredients[i].amount);
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