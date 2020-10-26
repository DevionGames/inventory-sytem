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
        private string m_BuySellDialogName = "BuySellDialog";

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

            ItemCollection collection = GetComponent<ItemCollection>();
            for (int i = 0; i < collection.Count; i++) {
                collection[i].BuyPrice = Mathf.RoundToInt(m_BuyPriceFactor*collection[i].BuyPrice);
            }
        }

        public override bool OverrideUse(Slot slot, Item item)
        {
            if (slot.Container.CanSellItems)
            {
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
                this.m_AmountSpinner.gameObject.SetActive(true);

                this.m_AmountSpinner.onChange.RemoveAllListeners();
                this.m_AmountSpinner.current = 1;
                this.m_AmountSpinner.min = 1;
                this.m_AmountSpinner.max = int.MaxValue;
                this.m_AmountSpinner.onChange.AddListener(delegate (float value)
                {
                    Currency price = Instantiate(item.BuyCurrency);
                    price.Stack = Mathf.RoundToInt(item.BuyPrice * value);
                    this.m_PriceInfo.RemoveItems();
                    this.m_PriceInfo.StackOrAdd(price);
                });
                this.m_AmountSpinner.onChange.Invoke(this.m_AmountSpinner.current);

                ExecuteEvent<ITriggerSelectBuyItem>(Execute, item);
                this.m_BuySellDialog.Show("Buy", "How many items do you want to buy?", item.Icon, delegate (int result)
                {
                    if (result == 0){
                        BuyItem(item, Mathf.RoundToInt(this.m_AmountSpinner.current), false);
                    }
                }, "Buy", "Cancel");
            }else {
                if (this.m_PurchasedStorageContainer == null || this.m_PaymentContainer == null){
                    return;
                }
                item = Instantiate(item);
                item.Stack = amount;
                Currency price = Instantiate(item.BuyCurrency);
                price.Stack = Mathf.RoundToInt(item.BuyPrice * amount);

                if ( this.m_PaymentContainer.RemoveItem(price,price.Stack))
                {

                    if (amount > item.MaxStack)
                    {
                        int stack = item.Stack;
                        Currency singlePrice = Instantiate(item.BuyCurrency);
                        singlePrice.Stack = item.BuyPrice;
                       // singlePrice.Stack = Mathf.RoundToInt(this.m_BuyPriceFactor * singlePrice.Stack);
                        int purchasedStack = 0;
                        for (int i = 0; i < stack; i++)
                        {
                            Item singleItem = Instantiate(item);
                            singleItem.Stack = 1;
                            if (!this.m_PurchasedStorageContainer.StackOrAdd(singleItem))
                            {
                                this.m_PaymentContainer.StackOrAdd(singlePrice);
                                InventoryManager.Notifications.containerFull.Show(this.m_PurchasedStorageWindow);
                                ExecuteEvent<ITriggerFailedToBuyItem>(Execute, item, FailureCause.ContainerFull);
                                break;
                            }
                            purchasedStack += 1;
                            ExecuteEvent<ITriggerBoughtItem>(Execute, singleItem);
                        }
                        InventoryManager.Notifications.boughtItem.Show(purchasedStack.ToString()+"x"+item.Name, singlePrice.Stack*purchasedStack + " " + price.Name);
                    }
                    else
                    {
                        Item itemInstance = Instantiate(item);
                        if (!this.m_PurchasedStorageContainer.StackOrAdd(itemInstance))
                        {
                            this.m_PaymentContainer.StackOrAdd(price);
                            InventoryManager.Notifications.containerFull.Show(this.m_PurchasedStorageWindow);
                            ExecuteEvent<ITriggerFailedToBuyItem>(Execute, item, FailureCause.ContainerFull);
                        }
                        else {
                            InventoryManager.Notifications.boughtItem.Show(itemInstance.Name, price.Stack+" "+price.Name);
                            ExecuteEvent<ITriggerBoughtItem>(Execute, itemInstance);
                        }
                    }
                }
                else
                {
                    InventoryManager.Notifications.noCurrencyToBuy.Show(item.Name, price.Stack + " " + price.Name);
                    ExecuteEvent<ITriggerFailedToBuyItem>(Execute, item, FailureCause.NotEnoughCurrency);
                }
            }
        }


        public void SellItem(Item item, int amount, bool showDialog = true)
        {
            if (showDialog)
            {
                this.m_AmountSpinner.gameObject.SetActive(true);
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
                this.m_BuySellDialog.Show("Sell", item.Stack>1? "How many items do you want to sell?":"Are you sure you want to sell this item?", item.Icon, delegate (int result)
                {
                    if (result == 0)
                    {
                        SellItem(item, Mathf.RoundToInt(this.m_AmountSpinner.current), false);
                    }
                   
                }, "Sell", "Cancel");
            }
            else
            {
                Currency price = Instantiate(item.SellCurrency);
                price.Stack = Mathf.RoundToInt(this.m_SellPriceFactor * item.SellPrice * amount);
                
                if (item.Container.RemoveItem(item, amount))
                {
                    ExecuteEvent<ITriggerSoldItem>(Execute, item);
                    this.m_PaymentContainer.StackOrAdd(price);
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