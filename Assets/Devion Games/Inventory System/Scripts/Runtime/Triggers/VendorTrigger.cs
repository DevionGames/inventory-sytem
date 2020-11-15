using System.Collections;
using System.Collections.Generic;
using DevionGames.UIWidgets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DevionGames.InventorySystem
{
    public class VendorTrigger : Trigger, ITriggerUnUsedHandler
    {
        public override string[] Callbacks
        {
            get
            {
                List<string> callbacks = new List<string>(base.Callbacks);
                callbacks.Add("OnSelectSellItem");
                callbacks.Add("OnSoldItem");
                callbacks.Add("OnFaildToSellItem");
                callbacks.Add("OnSelectBuyItem");
                callbacks.Add("OnBoughtItem");
                callbacks.Add("OnFaildToBuyItem");
                return callbacks.ToArray();
            }
        }

        [Header("Vendor")]
        [Range(0f,10f)]
        [SerializeField]
        protected float m_BuyPriceFactor = 1.0f;
        [Range(0f, 10f)]
        [SerializeField]
        protected float m_SellPriceFactor = 1.0f;
        [SerializeField]
        protected string m_PurchasedStorageWindow = "Inventory";
        [SerializeField]
        protected string m_PaymentWindow = "Inventory";
        [SerializeField]
        protected bool m_RemoveItemAfterPurchase = false;

        [Header("Buy & Sell Dialog")]
        [SerializeField]
        private string m_BuySellDialogName = "BuySellDialog";
        [SerializeField]
        private bool m_DisplaySpinner = true;

        [Header("Buy")]
        [SerializeField]
        private string m_BuyDialogTitle = "Buy";
        [SerializeField]
        private string m_BuyDialogText= "How many items do you want to buy?";
        [SerializeField]
        private string m_BuyDialogButton = "Buy";

        [Header("Sell")]
        [SerializeField]
        private string m_SellDialogTitle = "Sell";
        [SerializeField]
        private string m_SellSingleDialogText = "Are you sure you want to sell this item?";
        [SerializeField]
        private string m_SellMultipleDialogText = "How many items do you want to sell?";
        [SerializeField]
        private string m_SellDialogButton = "Sell";

        private DialogBox m_BuySellDialog;
        private Spinner m_AmountSpinner;
        private ItemContainer m_PriceInfo;

        private ItemContainer m_PurchasedStorageContainer;
        private ItemContainer m_PaymentContainer;

        protected static void Execute(ITriggerSelectSellItem handler, Item item, GameObject player)
        {
            handler.OnSelectSellItem(item, player);
        }

        protected static void Execute(ITriggerSoldItem handler, Item item, GameObject player)
        {
            handler.OnSoldItem(item, player);
        }

        protected static void Execute(ITriggerFailedToSellItem handler, Item item, GameObject player, FailureCause failureCause)
        {
            handler.OnFailedToSellItem(item, player, failureCause);
        }

        protected static void Execute(ITriggerSelectBuyItem handler, Item item, GameObject player)
        {
            handler.OnSelectBuyItem(item, player);
        }

        protected static void Execute(ITriggerBoughtItem handler, Item item, GameObject player)
        {
            handler.OnBoughtItem(item, player);
        }

        protected static void Execute(ITriggerFailedToBuyItem handler, Item item, GameObject player, FailureCause failureCause)
        {
            handler.OnFailedToBuyItem(item, player, failureCause);
        }

        protected override void Start()
        {
            base.Start();
            this.m_BuySellDialog = WidgetUtility.Find<DialogBox>(this.m_BuySellDialogName);
            if (this.m_BuySellDialog != null) {
               this.m_AmountSpinner = this.m_BuySellDialog.GetComponentInChildren<Spinner>();
               this.m_PriceInfo = this.m_BuySellDialog.GetComponentInChildren<ItemContainer>();
            }
            this.m_PurchasedStorageContainer= WidgetUtility.Find<ItemContainer>(this.m_PurchasedStorageWindow);
            this.m_PaymentContainer = WidgetUtility.Find<ItemContainer>(this.m_PaymentWindow);

          /*  ItemCollection collection = GetComponent<ItemCollection>();
            for (int i = 0; i < collection.Count; i++) {
                collection[i].BuyPrice = Mathf.RoundToInt(m_BuyPriceFactor*collection[i].BuyPrice);
            }*/
        }

        public override bool OverrideUse(Slot slot, Item item)
        {
            
            if (slot.Container.CanSellItems)
            {
                if (!item.IsSellable)
                {
                    InventoryManager.Notifications.cantSellItem.Show(item.DisplayName);
                    return true;
                }
                SellItem(item, item.Stack, true);
            }
            else if(Trigger.currentUsedWindow == slot.Container)
            {
              
                BuyItem(item, 1);
            }
            return true;
        }

        public void BuyItem(Item item,int amount, bool showDialog = true) {
           
            if (showDialog)
            {
                this.m_AmountSpinner.gameObject.SetActive(this.m_DisplaySpinner);

                this.m_AmountSpinner.onChange.RemoveAllListeners();
                this.m_AmountSpinner.current = 1;
                this.m_AmountSpinner.min = 1;
                ObjectProperty property = item.FindProperty("BuyBack");
                this.m_AmountSpinner.max = (this.m_RemoveItemAfterPurchase || property != null && property.boolValue)?item.Stack:int.MaxValue;
                this.m_AmountSpinner.onChange.AddListener(delegate (float value)
                {
                    Currency price = Instantiate(item.BuyCurrency);
                    price.Stack = Mathf.RoundToInt(this.m_BuyPriceFactor * item.BuyPrice * value);
                    this.m_PriceInfo.RemoveItems();
                    this.m_PriceInfo.StackOrAdd(price);
                });
                this.m_AmountSpinner.onChange.Invoke(this.m_AmountSpinner.current);

                ExecuteEvent<ITriggerSelectBuyItem>(Execute, item);
                this.m_BuySellDialog.Show(this.m_BuyDialogTitle, this.m_BuyDialogText, item.Icon, delegate (int result)
                {
                    if (result == 0){
                        BuyItem(item, Mathf.RoundToInt(this.m_AmountSpinner.current), false);
                    }
                }, this.m_BuyDialogButton, "Cancel");
            }else {
                if (this.m_PurchasedStorageContainer == null || this.m_PaymentContainer == null){
                    return;
                }
                Rarity rarity = item.Rarity;
                Item instance = Instantiate(item);

                instance.Rarity = rarity;
                instance.Stack = amount;
                Currency price = Instantiate(instance.BuyCurrency);
                price.Stack = Mathf.RoundToInt(this.m_BuyPriceFactor*instance.BuyPrice * amount);

                if ( this.m_PaymentContainer.RemoveItem(price,price.Stack))
                {

                    if (amount > instance.MaxStack)
                    {
                        int stack = instance.Stack;
                        Currency singlePrice = Instantiate(instance.BuyCurrency);
                        singlePrice.Stack = Mathf.RoundToInt(instance.BuyPrice*this.m_BuyPriceFactor);
                       // singlePrice.Stack = Mathf.RoundToInt(this.m_BuyPriceFactor * singlePrice.Stack);
                        int purchasedStack = 0;
                        for (int i = 0; i < stack; i++)
                        {
                            Item singleItem = Instantiate(instance);
                            singleItem.Rarity = instance.Rarity;
                            singleItem.Stack = 1;
                            if (!this.m_PurchasedStorageContainer.StackOrAdd(singleItem))
                            {
                                this.m_PaymentContainer.StackOrAdd(singlePrice);
                                InventoryManager.Notifications.containerFull.Show(this.m_PurchasedStorageWindow);
                                ExecuteEvent<ITriggerFailedToBuyItem>(Execute, instance, FailureCause.ContainerFull);
                                break;
                            }
                            purchasedStack += 1;
                            ExecuteEvent<ITriggerBoughtItem>(Execute, singleItem);
                        }
                        if (this.m_RemoveItemAfterPurchase)
                        {
                            item.Container.RemoveItem(item, purchasedStack);
                        }
                        InventoryManager.Notifications.boughtItem.Show(purchasedStack.ToString()+"x"+instance.DisplayName, singlePrice.Stack*purchasedStack + " " + price.Name);
                    }
                    else
                    {
                        Item itemInstance = Instantiate(instance);
                        itemInstance.Rarity = instance.Rarity;

                        if (!this.m_PurchasedStorageContainer.StackOrAdd(itemInstance))
                        {
                            this.m_PaymentContainer.StackOrAdd(price);
                            InventoryManager.Notifications.containerFull.Show(this.m_PurchasedStorageWindow);
                            ExecuteEvent<ITriggerFailedToBuyItem>(Execute, instance, FailureCause.ContainerFull);
                        }else {
                            ObjectProperty property = item.FindProperty("BuyBack");
    
                            if (this.m_RemoveItemAfterPurchase || property != null && property.boolValue)
                            {
                                item.RemoveProperty("BuyBack");
                                item.Container.RemoveItem(item, amount);
                            }
                            InventoryManager.Notifications.boughtItem.Show(itemInstance.Name, price.Stack+" "+price.Name);
                            ExecuteEvent<ITriggerBoughtItem>(Execute, itemInstance);
                        }
                    }
                }
                else
                {
                    InventoryManager.Notifications.noCurrencyToBuy.Show(item.DisplayName, price.Stack + " " + price.Name);
                    ExecuteEvent<ITriggerFailedToBuyItem>(Execute, item, FailureCause.NotEnoughCurrency);
                }
            }
        }

        public void SellItem(Item item, int amount, bool showDialog = true)
        {
            if (showDialog)
            {
                this.m_AmountSpinner.gameObject.SetActive(this.m_DisplaySpinner);
                 if (item.Stack > 1)
                    {

                        this.m_AmountSpinner.onChange.RemoveAllListeners();
                        this.m_AmountSpinner.current = amount;
                        this.m_AmountSpinner.min = 1;
                        this.m_AmountSpinner.max = item.Stack;
                        this.m_AmountSpinner.onChange.AddListener(delegate (float value)
                        {
                            Currency price = Instantiate(item.SellCurrency);
                            price.Stack = Mathf.RoundToInt(this.m_SellPriceFactor * item.SellPrice * value);
                            this.m_PriceInfo.RemoveItems();
                            this.m_PriceInfo.StackOrAdd(price);
                        });
                        this.m_AmountSpinner.onChange.Invoke(this.m_AmountSpinner.current);
                    }else {
                        this.m_AmountSpinner.current = 1;
                        this.m_AmountSpinner.gameObject.SetActive(false);
                        Currency price = Instantiate(item.SellCurrency);
                        price.Stack = Mathf.RoundToInt(this.m_SellPriceFactor * item.SellPrice);
                        this.m_PriceInfo.RemoveItems();
                        this.m_PriceInfo.StackOrAdd(price);
                    }

                ExecuteEvent<ITriggerSelectSellItem>(Execute, item);
                this.m_BuySellDialog.Show(this.m_SellDialogTitle, item.Stack>1?this.m_SellMultipleDialogText:this.m_SellSingleDialogText, item.Icon, delegate (int result)
                {
                    if (result == 0)
                    {
                        SellItem(item, Mathf.RoundToInt(this.m_AmountSpinner.current), false);
                    }
                   
                }, this.m_SellDialogButton, "Cancel");
            }
            else
            {
                Currency price = Instantiate(item.SellCurrency);
                price.Stack = Mathf.RoundToInt(this.m_SellPriceFactor * item.SellPrice * amount);
                
                if (item.Container.RemoveItem(item, amount))
                {
                    ExecuteEvent<ITriggerSoldItem>(Execute, item);
                    this.m_PaymentContainer.StackOrAdd(price);
                    if (item.CanBuyBack)
                    {
                        item.AddProperty("BuyBack", true);
                        Trigger.currentUsedWindow.AddItem(item);
                    }
                    InventoryManager.Notifications.soldItem.Show((amount>1?amount.ToString()+"x":"")+item.Name, price.Stack+" "+price.Name);
                }
                else {
                    ExecuteEvent<ITriggerFailedToSellItem>(Execute, item, FailureCause.Remove);
                }
            }
        }

        public void OnTriggerUnUsed(GameObject player)
        {
            if (Trigger.currentUsedTrigger == this && this.m_BuySellDialog.IsVisible) {
                this.m_BuySellDialog.Close();
            }
        }

        protected override void RegisterCallbacks()
        {
            base.RegisterCallbacks();
            this.m_CallbackHandlers.Add(typeof(ITriggerSelectSellItem), "OnSelectSellItem");
            this.m_CallbackHandlers.Add(typeof(ITriggerSoldItem), "OnSoldItem");
            this.m_CallbackHandlers.Add(typeof(ITriggerFailedToSellItem), "OnFailedToSellItem");
            this.m_CallbackHandlers.Add(typeof(ITriggerSelectBuyItem), "OnSelectBuyItem");
            this.m_CallbackHandlers.Add(typeof(ITriggerBoughtItem), "OnBoughtItem");
            this.m_CallbackHandlers.Add(typeof(ITriggerFailedToBuyItem), "OnFailedToBuyItem");
        }
    }
}