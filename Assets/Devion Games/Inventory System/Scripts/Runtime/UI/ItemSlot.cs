using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DevionGames.UIWidgets;
using System.Linq;
using UnityEngine.Events;

namespace DevionGames.InventorySystem
{
    public class ItemSlot : Slot, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        /// Key to use item.
		/// </summary>
		[SerializeField]
        protected KeyCode m_UseKey;
        /// <summary>
        /// Key text to display the use key
        /// </summary>
        [SerializeField]
        protected Text m_Key;
        /// <summary>
		/// The image to display cooldown.
		/// </summary>
		[SerializeField]
        protected Image m_CooldownOverlay;
        /// <summary>
        /// The image to display cooldown.
        /// </summary>
        [SerializeField]
        protected Text m_Cooldown;
        /// <summary>
		/// The text to display item description.
		/// </summary>
		[SerializeField]
        protected Text m_Description;
        /// <summary>
        /// Item container that will show ingredients of the item
        /// </summary>
        [SerializeField]
        protected ItemContainer m_Ingredients;
        /// <summary>
        /// Item container that will show the buy price of item.
        /// </summary>
        [SerializeField]
        protected ItemContainer m_BuyPrice;

        private bool m_IsCooldown;
        /// <summary>
        /// Gets a value indicating whether this slot is in cooldown.
        /// </summary>
        /// <value><c>true</c> if this slot is in cooldown; otherwise, <c>false</c>.</value>
        public bool IsCooldown
        {
            get
            {
                UpdateCooldown();
                return this.m_IsCooldown;
            }
        }
        protected float cooldownDuration;
        protected float cooldownInitTime;

        private static DragObject m_DragObject;
        public static DragObject dragObject {
            get {return m_DragObject;}
            set{
                m_DragObject = value;
                //Set the dragging icon
                if (m_DragObject != null && m_DragObject.item != null)
                {
                    UICursor.Set(m_DragObject.item.Icon);
                }
                else
                {
                    //if value is null, remove the dragging icon
                    UICursor.Clear();
                }
            }
        }
        protected Coroutine m_DelayTooltipCoroutine;
        protected ScrollRect m_ParentScrollRect;
        protected bool m_IsMouseKey;


        protected override void Start()
        {
            base.Start();
            if (this.m_CooldownOverlay != null)
                this.m_CooldownOverlay.raycastTarget = false;

            this.m_ParentScrollRect = GetComponentInParent<ScrollRect>();
            if (this.m_Key != null){
                this.m_Key.text = UnityTools.KeyToCaption(this.m_UseKey);
            }
            this.m_IsMouseKey = m_UseKey == KeyCode.Mouse0 || m_UseKey == KeyCode.Mouse1 || m_UseKey == KeyCode.Mouse2;
        }

        /// <summary>
		/// Update is called every frame, if the MonoBehaviour is enabled.
		/// </summary>
		protected virtual void Update()
        {
            if (Input.GetKeyDown(m_UseKey) && !UnityTools.IsPointerOverUI())
            {
                if(!(this.m_IsMouseKey && TriggerRaycaster.IsPointerOverTrigger()))
                    Use();
            }
            if (Container != null && Container.IsVisible)
            {
                UpdateCooldown();
            }
        }

        public override void Repaint()
        {
            base.Repaint();

            if (this.m_Description != null)
            {
                this.m_Description.text = (ObservedItem != null ? ObservedItem.Description : string.Empty);
            }

            if (this.m_Ingredients != null)
            {
                this.m_Ingredients.RemoveItems();
                if (!IsEmpty) { 
                    for (int i = 0; i < ObservedItem.ingredients.Count; i++)
                    {
                        Item ingredient = Instantiate(ObservedItem.ingredients[i].item);
                        ingredient.Stack = ObservedItem.ingredients[i].amount;
                        this.m_Ingredients.StackOrAdd(ingredient);
                    }
                }
            }

            if (this.m_BuyPrice != null)
            {
                this.m_BuyPrice.RemoveItems();
                if (!IsEmpty)
                {
                    Currency price = Instantiate(ObservedItem.BuyCurrency);
                    price.Stack = Mathf.RoundToInt(ObservedItem.BuyPrice);
                    this.m_BuyPrice.StackOrAdd(price);
                    //Debug.Log(" Price Update for "+ObservedItem.Name+" "+price.Name+" "+price.Stack);
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ShowTooltip();
        }

        protected IEnumerator DelayTooltip(float delay)
        {
            float time = 0.0f;
            yield return true;
            while (time < delay)
            {
                time += Container.IgnoreTimeScale?Time.unscaledDeltaTime: Time.deltaTime;
                yield return true;
            }
            if (InventoryManager.UI.tooltip != null && ObservedItem != null)
            {
                InventoryManager.UI.tooltip.Show(UnityTools.ColorString(ObservedItem.DisplayName, ObservedItem.Rarity.Color), ObservedItem.Description, ObservedItem.Icon, ObservedItem.GetPropertyInfo());
                if (InventoryManager.UI.sellPriceTooltip != null && ObservedItem.IsSellable && ObservedItem.SellPrice > 0)
                {
                    InventoryManager.UI.sellPriceTooltip.RemoveItems();
                    Currency currency = Instantiate(ObservedItem.SellCurrency);
                    currency.Stack = ObservedItem.SellPrice*ObservedItem.Stack;

                    InventoryManager.UI.sellPriceTooltip.StackOrAdd(currency);
                    InventoryManager.UI.sellPriceTooltip.Show();
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CloseTooltip();
        }

        private void ShowTooltip() {
            if (Container.ShowTooltips && this.isActiveAndEnabled && dragObject == null && ObservedItem != null)
            {
                if (this.m_DelayTooltipCoroutine != null)
                {
                    StopCoroutine(this.m_DelayTooltipCoroutine);
                }
                this.m_DelayTooltipCoroutine = StartCoroutine(DelayTooltip(0.3f));
            }
        }

        private void CloseTooltip() {
            if (Container.ShowTooltips && InventoryManager.UI.tooltip != null)
            {
                InventoryManager.UI.tooltip.Close();
                if (InventoryManager.UI.sellPriceTooltip != null)
                {
                    InventoryManager.UI.sellPriceTooltip.RemoveItems();
                    InventoryManager.UI.sellPriceTooltip.Close();
                }
            }
            if (this.m_DelayTooltipCoroutine != null)
            {
                StopCoroutine(this.m_DelayTooltipCoroutine);
            }
        }

        // In order to receive OnPointerUp callbacks, we need implement the IPointerDownHandler interface
        public virtual void OnPointerDown(PointerEventData eventData) {}

        //Detects the release of the mouse button
        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (!eventData.dragging)
            {
                Stack stack = InventoryManager.UI.stack;

                bool isUnstacking = stack != null && stack.item != null;

                if (!isUnstacking && InventoryManager.Input.unstackEvent.HasFlag<Configuration.Input.UnstackInput>(Configuration.Input.UnstackInput.OnClick) && Input.GetKey(InventoryManager.Input.unstackKeyCode) && ObservedItem.Stack > 1)
                {
                    Unstack();
                    return;
                }
               
                //Check if we are currently unstacking the item
                if (isUnstacking && Container.StackOrAdd(this, stack.item) ){
                    
                    stack.item = null;
                    UICursor.Clear();
         
                }

                if (isUnstacking){
                    return;
                }
            
                if (Container.useButton.HasFlag((InputButton)Mathf.Clamp(((int)eventData.button * 2), 1, int.MaxValue)) && ObservedItem != null)
                {
                    if (Container.UseContextMenu)
                    {
                        UIWidgets.ContextMenu menu = InventoryManager.UI.contextMenu;
                        if (menu == null) { return; }
                        menu.Clear();

                        if (Trigger.currentUsedTrigger != null && Trigger.currentUsedTrigger is VendorTrigger && Container.CanSellItems)
                        {
                            menu.AddMenuItem("Sell", Use);
                        }
                        else if (ObservedItem is UsableItem)
                        {
                            menu.AddMenuItem("Use", Use);

                        }
                        if (ObservedItem.MaxStack > 1 || ObservedItem.MaxStack == 0)
                        {
                            menu.AddMenuItem("Unstack", Unstack);
                        }
                        
                        menu.AddMenuItem("Drop", DropItem);

                        menu.Show();
                    }
                    else
                    {
                        Use();
                    }
                }
            }
        }

        //Called by a BaseInputModule before a drag is started.
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (Container.IsLocked) {
                InventoryManager.Notifications.inUse.Show();
                return;
            }   

            //Check if we can start dragging
            if (ObservedItem != null && !IsCooldown && Container.CanDragOut)
            {
                //If key for unstacking items is pressed and if the stack is greater then 1, show the unstack ui.
                if (InventoryManager.Input.unstackEvent.HasFlag<Configuration.Input.UnstackInput>(Configuration.Input.UnstackInput.OnDrag) && Input.GetKey(InventoryManager.Input.unstackKeyCode) && ObservedItem.Stack > 1){
                    Unstack();
                }else{
                    //Set the dragging slot
                    // draggedSlot = this;
                    if(base.m_Ícon == null || !base.m_Ícon.raycastTarget || eventData.pointerCurrentRaycast.gameObject == base.m_Ícon.gameObject)
                        dragObject = new DragObject(this);
    
                }
            }
            if (this.m_ParentScrollRect != null && dragObject == null)
            {
                this.m_ParentScrollRect.OnBeginDrag(eventData);
            }
        }

        //When draging is occuring this will be called every time the cursor is moved.
        public virtual void OnDrag(PointerEventData eventData) {
            if (this.m_ParentScrollRect != null) {
                this.m_ParentScrollRect.OnDrag(eventData);
            }
        }

        //Called by a BaseInputModule when a drag is ended.
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            RaycastHit hit;
            if (!UnityTools.IsPointerOverUI() && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (Container.CanDropItems)
                {
                    DropItem();
                }
                else if (Container.UseReferences && Container.CanDragOut){
                    Container.RemoveItem(Index);

                }
            }
  
            dragObject = null;
           
            if (this.m_ParentScrollRect != null)
            {
                this.m_ParentScrollRect.OnEndDrag(eventData);
            }
            //Repaint the slot
            Repaint();
        }

        //Called by a BaseInputModule on a target that can accept a drop.
        public virtual void OnDrop(PointerEventData data)
        {
            if (dragObject != null && Container.CanDragIn){
                Container.StackOrSwap(this, dragObject.slot);
            }
        }

        //Try to drop the item to ground
        private void DropItem()
        {
            if (Container.IsLocked)
            {
                InventoryManager.Notifications.inUse.Show();
                return;
            }

            if (IsCooldown)
                return;

            //Get the item to drop
            Item item = dragObject != null ? dragObject.item : ObservedItem;

            //Check if the item is droppable
            if (item != null && item.IsDroppable)
            {
                //Get item prefab
                GameObject prefab = item.OverridePrefab != null ? item.OverridePrefab : item.Prefab;
                RaycastHit hit;
                Vector3 position = Vector3.zero;
                Vector3 forward = Vector3.zero;
                if (InventoryManager.current.PlayerInfo.transform != null)
                {
                    position = InventoryManager.current.PlayerInfo.transform.position;
                    forward = InventoryManager.current.PlayerInfo.transform.forward;
                }

                //Cast a ray from mouse postion to ground
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit) && !UnityTools.IsPointerOverUI())
                {
                    //Clamp the drop distance to max drop distance defined in setting.
                    Vector3 worldPos = hit.point;
                    Vector3 diff = worldPos - position;
                    float distance = diff.magnitude;
                    //if player is null this does not work!
                    if (distance > (InventoryManager.DefaultSettings.maxDropDistance - (transform.localScale.x / 2)))
                    {
                        position = position + (diff / distance) * InventoryManager.DefaultSettings.maxDropDistance;
                    }
                    else
                    {
                        position = worldPos;
                    }
                }
                else
                {
                    position = position + forward;
                }

                //Instantiate the prefab at position
                GameObject go = InventoryManager.Instantiate(prefab, position + Vector3.up * 0.3f, Quaternion.identity);
                go.name = go.name.Replace("(Clone)","");
                //Reset the item collection of the prefab with this item
                ItemCollection collection = go.GetComponent<ItemCollection>();
                if (collection != null)
                {
                    collection.Clear();
                    collection.Add(item);
                }
                PlaceItem placeItem = go.GetComponentInChildren<PlaceItem>(true);
                if (placeItem != null)
                    placeItem.enabled = true;

                ItemContainer.RemoveItemCompletely(item);
                Container.NotifyDropItem(item, go);
            }
        }

        //Unstack items
        private void Unstack() {
            
            if (InventoryManager.UI.stack != null)
            {
                InventoryManager.UI.stack.SetItem(ObservedItem);
            }
        }

        /// <summary>
        /// Set the slot in cooldown
        /// </summary>
        /// <param name="cooldown">In seconds</param>
        public void Cooldown(float cooldown)
        {
            if (!m_IsCooldown && cooldown > 0f)
            {
                cooldownDuration = cooldown;
                cooldownInitTime = Time.time;
                this.m_IsCooldown = true;
            }
        }

        /// <summary>
        /// Updates the cooldown image and sets if the slot is in cooldown.
        /// </summary>
        private void UpdateCooldown()
        {
            if (this.m_IsCooldown && this.m_CooldownOverlay != null)
            {
                if (Time.time - cooldownInitTime < cooldownDuration)
                {
                    if (this.m_Cooldown != null) {
                        this.m_Cooldown.text = (cooldownDuration - (Time.time - cooldownInitTime)).ToString("f1");
                    }
                    this.m_CooldownOverlay.fillAmount = Mathf.Clamp01(1f - ((Time.time - cooldownInitTime) / cooldownDuration));
                }else{
                    if(this.m_Cooldown != null)
                        this.m_Cooldown.text = string.Empty;

                    this.m_CooldownOverlay.fillAmount = 0f;
                }


            }
            this.m_IsCooldown = (cooldownDuration - (Time.time - cooldownInitTime)) > 0f;
        }

        /// <summary>
        /// Use the item in slot
        /// </summary>
        public override void Use()
        {
            if (Container.IsLocked)
            {
                InventoryManager.Notifications.inUse.Show();
                return;
            }

            Container.NotifyTryUseItem(ObservedItem, this);
            //Check if the item can be used.
            if (CanUse())
            {
                //Check if there is an override item behavior on trigger.
                if ((Trigger.currentUsedTrigger as Trigger) != null && (Trigger.currentUsedTrigger as Trigger).OverrideUse(this, ObservedItem))
                {
                    return;
                }
                if (Container.UseReferences)
                {
                    ObservedItem.Slot.Use();
                    return;
                }
                //Try to move item
                if (!MoveItem())
                {
                    CloseTooltip();
                    ObservedItem.Use();
                    Container.NotifyUseItem(ObservedItem, this);
                } else {
                    CloseTooltip();
                    ShowTooltip();
                }
              
            } else if(IsCooldown && !IsEmpty){
                InventoryManager.Notifications.inCooldown.Show(ObservedItem.DisplayName, (cooldownDuration - (Time.time - cooldownInitTime)).ToString("f2"));
            }
        }

        //Can we use the item
        public override bool CanUse()
        {
            return !IsCooldown && ObservedItem != null;
        }

        public class DragObject {
            public ItemContainer container;
            public Slot slot;
            public Item item;
            
            public DragObject(Slot slot) {
                this.slot = slot;
                this.container = slot.Container;
                this.item = slot.ObservedItem;
            }
        }
    }
}