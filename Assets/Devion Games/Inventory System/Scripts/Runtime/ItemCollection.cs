using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace DevionGames.InventorySystem
{
	public class ItemCollection : MonoBehaviour, IEnumerable<Item>, IJsonSerializable
    {
		[ItemPicker (true)]
		[SerializeField]
		protected List<Item> items = new List<Item> ();
        [SerializeField]
        protected List<int> amounts = new List<int>();
        [SerializeField]
        protected List<float> randomProperty = new List<float>();
        [HideInInspector]
		public UnityEvent onChange;

        private bool m_Initialized;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (this.m_Initialized) { return; }
            //  if (items.Select(x => x == null).Count() > 0) { Debug.LogError("ItemCollection has Nullreference items: "+gameObject.name); }
           
            items = InventoryManager.CreateInstances(items.ToArray(), amounts.ToArray(), randomProperty.ToArray()).ToList();
            for (int i = 0; i < items.Count; i++) {
                Item current = items[i];
                if (current.Stack > current.MaxStack) {
                    //Split in smaller stacks
                    int maxStacks = Mathf.FloorToInt((float)current.Stack / (float)current.MaxStack);
                    int restStack = current.Stack - (current.MaxStack * maxStacks);
                    for (int j = 0; j < maxStacks; j++) {
                        Item instance = InventoryManager.CreateInstance(current);
                        instance.Stack = instance.MaxStack;
                        Add(instance);
                    }
                    if (restStack > 0)
                    {
                        current.Stack = restStack;
                    }
                    else {
                        Remove(current);
                    }
                   
                }
            }
            

            //Stack same currencies
            CombineCurrencies();
            ItemCollectionPopulator populator = GetComponent<ItemCollectionPopulator>();
            if (populator != null) {
                Add(InventoryManager.CreateInstances(populator.m_ItemGroup));
            } 
            this.m_Initialized = true;

		}

		public Item this [int index] {
			get { return this.items [index]; }
			set {
				Insert (index, value);
                if (onChange != null)
                    onChange.Invoke();
            }
		}

		public int Count {
			get { 
				return items.Count;
			}
		}

		public bool IsEmpty {
			get { 
				return items.Count == 0;
			}
		}

		public IEnumerator<Item> GetEnumerator ()
		{
			return this.items.GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return this.GetEnumerator ();
		}

        public void RandomizeProperties(Item item) {
            if (item != null) {
                for (int i=0;i<items.Count;i++) {
                    if (item.Id == items[i].Id) {
                        item.RandomizeProperties(randomProperty[i]);
                    }
                }
            }
        }

        public void Add(Item[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                Add(items[i]);
            }
        }

        public void Add (Item item)
		{
			this.items.Add (item);
            this.amounts.Add(item.Stack);
            this.randomProperty.Add(0f);
            if(onChange != null)
			    onChange.Invoke ();

		}

		public bool Remove (Item item)
		{
            int index = items.IndexOf(item);
            
            bool result = this.items.Remove (item);
            if (result) {
                this.amounts.RemoveAt(index);
                this.randomProperty.RemoveAt(index);
                if (onChange != null)
                    onChange.Invoke();
            }
			return result;
		}

		public void Insert (int index, Item child)
		{
            this.amounts.Insert(index,child.Stack);
            this.randomProperty.Insert(index, 0f);
            this.items.Insert (index, child);
            if (onChange != null)
                onChange.Invoke();
        }

		public void RemoveAt (int index)
		{
            this.amounts.RemoveAt(index);
            this.randomProperty.RemoveAt(index);
			this.items.RemoveAt (index);
            if (onChange != null)
                onChange.Invoke();
        }

		public void Clear ()
		{
            this.randomProperty.Clear();
            this.amounts.Clear();
            Item[] currencies = this.items.Where(x => typeof(Currency).IsAssignableFrom(x.GetType())).ToArray();
            for (int i = 0; i < currencies.Length; i++) {
                currencies[i].Stack = 0;
                if(currencies[i].Slot != null)
                    currencies[i].Slot.ObservedItem = currencies[i];
            }
            this.items.Clear();

            Add(currencies);

            if (onChange != null)
                onChange.Invoke();
        }

        public void GetObjectData(Dictionary<string, object> data)
        {
            data.Add("Prefab", gameObject.name.Replace("(Clone)", ""));
            data.Add("Position", new List<float> { transform.position.x, transform.position.y, transform.position.z });
            data.Add("Rotation", new List<float> { transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z });
            data.Add("Type",(GetComponent<ItemContainer>() != null?"UI":"Trigger"));
            if (items.Count > 0)
            {
                List<object> mItems = new List<object>();
                for (int i = 0; i < items.Count; i++)
                {
                    Item item = items[i];
                    if (item != null)
                    {
                        Dictionary<string, object> itemData = new Dictionary<string, object>();
                        item.GetObjectData(itemData);
                        mItems.Add(itemData);
                    }
                    else
                    {
                        mItems.Add(null);
                    }

                }
                data.Add("Items", mItems);
            }
        }

        public void SetObjectData(Dictionary<string, object> data)
        {
            List<object> position = data["Position"] as List<object>;
            List<object> rotation = data["Rotation"] as List<object>;
            if ((string)data["Type"] != "UI")
            {
                transform.position = new Vector3(System.Convert.ToSingle(position[0]), System.Convert.ToSingle(position[1]), System.Convert.ToSingle(position[2]));
                transform.rotation = Quaternion.Euler(new Vector3(System.Convert.ToSingle(rotation[0]), System.Convert.ToSingle(rotation[1]), System.Convert.ToSingle(rotation[2])));
            }
            Clear();

            ItemContainer container = GetComponent<ItemContainer>();
            if (data.ContainsKey("Items"))
            {
                List<object> mItems = data["Items"] as List<object>;
                for (int i = 0; i < mItems.Count; i++)
                {
                    Dictionary<string, object> itemData = mItems[i] as Dictionary<string, object>;
                    if (itemData != null)
                    {
                        List<Item> items = new List<Item>(InventoryManager.Database.items);
                        items.AddRange(InventoryManager.Database.currencies);

                        Item item = items.Find(x => x.Name == (string)itemData["Name"]);
                        
                        if (item != null)
                        {
                            Item mItem = (Item)ScriptableObject.Instantiate(item);
                            mItem.SetObjectData(itemData);


                            Add(mItem);
                            amounts[i] = 0;
                            randomProperty[i] = 0f;

                            if (itemData.ContainsKey("Slots"))
                            {
                                List<object> slots = itemData["Slots"] as List<object>;
                                for (int j = 0; j < slots.Count; j++)
                                {
                                    int slot = System.Convert.ToInt32(slots[j]);
                                    if (container.Slots.Count > slot)
                                    {
                                        mItem.Slots.Add(container.Slots[slot]);
                                    }
                                }
                            }

                           /* if (itemData.ContainsKey("Index") && container != null)
                            {
                                int index = System.Convert.ToInt32(itemData["Index"]);
                                if (container.Slots.Count > index)
                                {
                                    mItem.Slot = container.Slots[System.Convert.ToInt32(itemData["Index"])];
                                }
                            }*/
                        }
                    }
                }
            }
            CombineCurrencies();
            if (container != null) {
                container.Collection=this;
            }
        }

        private void CombineCurrencies()
        {
            Currency[] currencies = items.Where(x => x != null && typeof(Currency).IsAssignableFrom(x.GetType())).Cast<Currency>().ToArray();
            Dictionary<string, Currency> currencyMap = new Dictionary<string, Currency>();
            for (int i = 0; i < currencies.Length; i++)
            {
                Currency current = currencies[i];
                Currency currency = null;
                if (!currencyMap.TryGetValue(current.Name, out currency))
                {
                    currencyMap.Add(current.Name, current);
                    continue;
                }
                currency.Stack += current.Stack;
                items.Remove(current);
            }
        }

    }
}