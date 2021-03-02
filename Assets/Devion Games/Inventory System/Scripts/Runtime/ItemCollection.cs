using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace DevionGames.InventorySystem
{
	public class ItemCollection : MonoBehaviour, IEnumerable<Item>, IJsonSerializable
    {
        public bool saveable = true;
		[ItemPicker]
		[SerializeField]
        [FormerlySerializedAs("items")]
        protected List<Item> m_Items = new List<Item> ();
        [FormerlySerializedAs("amounts")]
        [SerializeField]
        protected List<int> m_Amounts = new List<int>();

        [SerializeField]
        protected List<ItemModifierList> m_Modifiers = new List<ItemModifierList>();

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

            //Used to sync old items
            if (this.m_Modifiers.Count < this.m_Items.Count)
            {
                for (int i = m_Modifiers.Count; i < this.m_Items.Count; i++)
                {
                    m_Modifiers.Add(new ItemModifierList());
                }
            }

            if (this.m_Amounts.Count < this.m_Items.Count) {
                for (int i = this.m_Amounts.Count; i < this.m_Items.Count; i++)
                {
                    this.m_Amounts.Add(1);
                }
            }

            m_Items = InventoryManager.CreateInstances(m_Items.ToArray(),this.m_Amounts.ToArray(), this.m_Modifiers.ToArray()).ToList();
          
            for (int i = 0; i < m_Items.Count; i++) {
                Item current = m_Items[i];
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
            if (populator != null && populator.enabled) {
                Item[] groupItems = InventoryManager.CreateInstances(populator.m_ItemGroup);
                Add(groupItems);
            } 
            this.m_Initialized = true;

		}

		public Item this [int index] {
			get { return this.m_Items [index]; }
			set {
				Insert (index, value);
                if (onChange != null)
                    onChange.Invoke();
            }
		}

		public int Count {
			get { 
				return m_Items.Count;
			}
		}

		public bool IsEmpty {
			get { 
				return m_Items.Count == 0;
			}
		}

		public IEnumerator<Item> GetEnumerator ()
		{
			return this.m_Items.GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return this.GetEnumerator ();
		}

        public void Add(Item[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                Add(items[i]);
            }
        }

        public void Add (Item item, bool allowStacking = false)
		{
            this.m_Items.Add (item);
            int index = m_Items.IndexOf(item);

            this.m_Amounts.Insert(index,item.Stack);
            this.m_Modifiers.Insert(index,new ItemModifierList());
            if(onChange != null)
			    onChange.Invoke ();

		}

     

        public bool Remove (Item item)
		{
            int index = m_Items.IndexOf(item);
            
            bool result = this.m_Items.Remove (item);
            if (result) {
                this.m_Amounts.RemoveAt(index);
                this.m_Modifiers.RemoveAt(index);
                if (onChange != null)
                    onChange.Invoke();
            }
			return result;
		}

		public void Insert (int index, Item child)
		{
            this.m_Items.Insert (index, child);
            this.m_Amounts.Insert(index, child.Stack);
            this.m_Modifiers.Insert(index, new ItemModifierList());
            if (onChange != null)
                onChange.Invoke();
        }

		public void RemoveAt (int index)
		{
			this.m_Items.RemoveAt (index);
            this.m_Amounts.RemoveAt(index);
            this.m_Modifiers.RemoveAt(index);
            if (onChange != null)
                onChange.Invoke();
        }

		public void Clear ()
		{
            
            Item[] currencies = this.m_Items.Where(x => typeof(Currency).IsAssignableFrom(x.GetType())).ToArray();
            for (int i = 0; i < currencies.Length; i++) {
                currencies[i].Stack = 0;
                if(currencies[i].Slot != null)
                    currencies[i].Slot.ObservedItem = currencies[i];
            }
            this.m_Items.Clear();
            this.m_Amounts.Clear();
            this.m_Modifiers.Clear();
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
            if (m_Items.Count > 0)
            {
                List<object> mItems = new List<object>();
                for (int i = 0; i < m_Items.Count; i++)
                {
                    Item item = m_Items[i];
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

                            this.m_Amounts[i] = 0;
                            this.m_Modifiers[i].modifiers.Clear();
                        
                            if (itemData.ContainsKey("Slots") && container != null)
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
            Currency[] currencies = m_Items.Where(x => x != null && typeof(Currency).IsAssignableFrom(x.GetType())).Cast<Currency>().ToArray();
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
                m_Items.Remove(current);
            }
        }

    }
}