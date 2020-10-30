using System;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DevionGames.UIWidgets;

namespace DevionGames.InventorySystem
{

	[System.Serializable]
	public class Item : ScriptableObject, INameable, IJsonSerializable
	{
		[SerializeField]
		[HideInInspector]
		private string m_Id;

		public string Id {
			get{ return this.m_Id; }
            set { this.m_Id = value; }
		}

		[SerializeField]
		private string m_ItemName = "New Item";

		public string Name {
			get{ return this.m_ItemName; }
			set{ this.m_ItemName = value; }
		}

        [SerializeField]
        private bool m_UseItemNameAsDisplayName = true;

        [SerializeField]
        private string m_DisplayName = "New Item";

        public string DisplayName
        {
            get {
                if (this.m_UseItemNameAsDisplayName)
                    return this.m_ItemName;
                return this.m_DisplayName; 
            
            }
            set {
                this.m_DisplayName = value; 
            }
        }

        [SerializeField]
		private Sprite m_Icon;

		public Sprite Icon {
			get{ return this.m_Icon; }
			set{ this.m_Icon = value; }
		}

		[SerializeField]
		private GameObject m_Prefab;

		public GameObject Prefab {
			get{ return m_Prefab; }
			set{ this.m_Prefab = value; }
		}

		[SerializeField]
		[Multiline (4)]
		private string m_Description = string.Empty;

		public string Description {
			get{ return this.m_Description; }
		}


		[Header ("Behaviour:")]
        [SerializeField]
        [CategoryPicker]
        private Category m_Category = null;

        public Category Category
        {
            get { return this.m_Category; }
            set { this.m_Category = value; }
        }

        private Rarity m_Rarity;
		public Rarity Rarity {
			get{
                if (this.m_Rarity == null) {
                    this.m_Rarity = SelectRarity(this.m_PossibleRarity);
                }
                return this.m_Rarity; 
            }
		}

        [SerializeField]
        private List<Rarity> m_PossibleRarity=new List<Rarity>();
        public List<Rarity> PossibleRarity {
            get { return this.m_PossibleRarity; }
            set { this.m_PossibleRarity = value; }
        }

        [SerializeField]
        private bool m_IsSellable = true;
        public bool IsSellable {
            get { return this.m_IsSellable; }
            set { this.m_IsSellable = true; }
        }

		[SerializeField]
		private int m_BuyPrice=0;

		public int BuyPrice {
			get{ return Mathf.RoundToInt(m_BuyPrice*Rarity.PriceMultiplier); }
            set { this.m_BuyPrice = value; }
		}

        [CurrencyPicker(true)]
        [SerializeField]
        private Currency m_BuyCurrency;

        public Currency BuyCurrency
        {
            get { return this.m_BuyCurrency; }
            set { this.m_BuyCurrency = value; }
        }

        [SerializeField]
		private int m_SellPrice=0;

		public int SellPrice {
			get{ return Mathf.RoundToInt(this.m_SellPrice*Rarity.PriceMultiplier); }
		}

        [CurrencyPicker(true)]
        [SerializeField]
        private Currency m_SellCurrency= null;

        public Currency SellCurrency
        {
            get { return this.m_SellCurrency; }
            set { this.m_SellCurrency = value; }
        }

        [SerializeField]
        [Range(1, 100)]
        private int m_Stack = 1;

        public virtual int Stack
        {
            get { return this.m_Stack; }
            set
            {
                this.m_Stack = value;
                if (Slot != null){
                    if (m_Stack <= 0 && !typeof(Currency).IsAssignableFrom(GetType())){
                        ItemContainer.RemoveItemCompletely(this);
                    }
                    Slot.Repaint();
                    for (int i = 0; i < ReferencedSlots.Count; i++){
                        ReferencedSlots[i].Repaint();
                    }
                }
            }
        }

        [SerializeField]
		[Range (0, 100)]
		private int m_MaxStack = 1;

		public virtual int MaxStack {
			get{
                if (this.m_MaxStack > 0){
                    return this.m_MaxStack;
                }
                return int.MaxValue;
            }
		}

		[SerializeField]
		private bool m_IsDroppable = true;

		public bool IsDroppable {
			get{ return this.m_IsDroppable; }
		}


		[SerializeField]
		private AudioClip m_DropSound = null;

		public AudioClip DropSound {
			get{ return this.m_DropSound; }
		}

		[SerializeField]
		private GameObject m_OverridePrefab=null;

		public GameObject OverridePrefab {
			get{ return this.m_OverridePrefab; }
		}

        [SerializeField]
        private bool m_IsCraftable=false;

        public bool IsCraftable
        {
            get { return this.m_IsCraftable; }
        }

        [SerializeField]
        private float m_CraftingDuration = 2f;

        public float CraftingDuration
        {
            get { return this.m_CraftingDuration; }
        }

        [SerializeField]
        private string m_CraftingAnimatorState= "Blacksmithy";

        public string CraftingAnimatorState
        {
            get { return this.m_CraftingAnimatorState; }
        }

        public List<Ingredient> ingredients = new List<Ingredient>();


        public ItemContainer Container
        {
            get {
                if (Slot != null) {
                    return Slot.Container;
                }
                return null;
            }
        }

       // private Slot m_Slot;
		public Slot Slot {
			get{ 
                if(Slots.Count > 0)
                {
                    return this.Slots[0];
                }
                return null; 
            }
		}

        private List<Slot> m_Slots= new List<Slot>();
        public List<Slot> Slots
        {
            get { return this.m_Slots; }
            set { this.m_Slots = value; }
        }

        private List<Slot> m_ReferencedSlots = new List<Slot> ();

		public List<Slot> ReferencedSlots {
			get{ return this.m_ReferencedSlots; }
			set{ this.m_ReferencedSlots = value; }
		}

		[SerializeField]
		private List<ObjectProperty> properties = new List<ObjectProperty> ();

        private float m_PropertyPercentRange = 0f;
        public float PropertyPercentRange {
            get { return this.m_PropertyPercentRange; }
            set { this.m_PropertyPercentRange = value; }
        }

		public ObjectProperty FindProperty (string name)
		{
			return properties.Find (property => property.Name == name);
		}

		public ObjectProperty[] FindProperties (string name)
		{
			return properties.FindAll (property => property.Name == name).ToArray ();
		}

		public ObjectProperty[] GetProperties ()
		{
			return properties.ToArray ();
		}

		public void SetProperties (ObjectProperty[] properties)
		{
			this.properties = new List<ObjectProperty> (properties);
		}

        public void RandomizeProperties()
        {
            for (int i = 0; i < properties.Count; i++)
            {
                RandomizeProperty(properties[i].Name, this.m_PropertyPercentRange);
            }
        }

        public void RandomizeProperties (float percent)
		{
			for (int i = 0; i < properties.Count; i++) {
				RandomizeProperty (properties [i].Name, percent);
			}
		}

		public void RandomizeProperty (string name, float percent)
		{
			ObjectProperty property = FindProperty (name);

			if (property != null) {
				object value = property.GetValue ();
                if (UnityTools.IsNumeric(value) && property.show)
                {
                    float cur = System.Convert.ToSingle(property.GetValue())*this.Rarity.Multiplier;
                    float percentage = cur * percent;
                    float random = UnityEngine.Random.Range(cur - percentage, cur + percentage);
                    if (value is float)
                    {
                        property.SetValue(random );
                    }
                    else if (value is int)
                    {
                        property.SetValue(Mathf.RoundToInt(random));
                    }
                }
			}
		}

        private System.Random rnd = new System.Random();

        private Rarity SelectRarity(List<Rarity> items)
        {
            int poolSize = 0;
            for (int i = 0; i < items.Count; i++)
            {
                poolSize += items[i].Chance;
            }
            int randomNumber = rnd.Next(0, poolSize) + 1;

            int accumulatedProbability = 0;
            for (int i = 0; i < items.Count; i++)
            {
                accumulatedProbability += items[i].Chance;
                if (randomNumber <= accumulatedProbability)
                    return items[i];
            }
            Rarity empty = ScriptableObject.CreateInstance<Rarity>();
            empty.Color = Color.grey;
            empty.Chance = 100;
            empty.Multiplier = 1.0f;
            return empty;   
        }

        protected virtual void OnEnable ()
		{
			if (string.IsNullOrEmpty (this.m_Id)) {
				this.m_Id = System.Guid.NewGuid ().ToString ();
			}
		}

        public List<KeyValuePair<string, string>> GetPropertyInfo()
        {
            List<KeyValuePair<string, string>> propertyInfo = new List<KeyValuePair<string, string>>();
            foreach (ObjectProperty property in properties)
            {
                if (property.show)
                {
                    propertyInfo.Add(new KeyValuePair<string, string>(UnityTools.ColorString(property.Name,property.color),FormatPropertyValue(property)));
                }
            }
            return propertyInfo;
        }

        private string FormatPropertyValue(ObjectProperty property) {
            string propertyValue = string.Empty;

            if (property.SerializedType == typeof(Vector2))
            {
                propertyValue = property.vector2Value.x + "-" + property.vector2Value.y;
            }
            else
            {
                propertyValue = ((UnityTools.IsNumeric(property.GetValue()) && System.Convert.ToSingle(property.GetValue()) > 0f) ? "+" : "-");
                propertyValue += (UnityTools.IsNumeric(property.GetValue()) ? Mathf.Abs(System.Convert.ToSingle(property.GetValue())) : property.GetValue()).ToString();
            }
            propertyValue = UnityTools.ColorString(propertyValue, property.color);
            return propertyValue;
        }

        public virtual void Use() { }

        public void GetObjectData(Dictionary<string, object> data)
        {
            data.Add("Name", this.Name);
            data.Add("Stack", this.Stack);
            data.Add("RarityIndex", InventoryManager.Database.raritys.IndexOf(Rarity));

            if (Slot != null)
            {
                data.Add("Index", this.Slot.Index);
            }
            foreach (ObjectProperty property in properties)
            {
                if (!typeof(UnityEngine.Object).IsAssignableFrom(property.SerializedType))
                {
                    data.Add(property.Name, property.GetValue());
                }
            }

            if (Slots.Count > 0)
            {
                List<object> slots = new List<object>();
                for (int i = 0; i < Slots.Count; i++)
                {
                    slots.Add(Slots[i].Index);
                }
                data.Add("Slots", slots);
            }

            if (ReferencedSlots.Count > 0)
            {
                List<object> references = new List<object>();
                for (int i = 0; i < ReferencedSlots.Count; i++)
                {
                    Dictionary<string, object> referenceData = new Dictionary<string, object>();
                    referenceData.Add("Container", ReferencedSlots[i].Container.Name);
                    referenceData.Add("Slot", ReferencedSlots[i].Index);
                    references.Add(referenceData);
                }
                data.Add("Reference", references);
            }
        }

        public void SetObjectData(Dictionary<string, object> data)
        {
            this.Stack = System.Convert.ToInt32(data["Stack"]);
            if (data.ContainsKey("RarityIndex")) {
                int rarityIndex =System.Convert.ToInt32(data["RarityIndex"]);
                if (rarityIndex > -1 && rarityIndex < InventoryManager.Database.raritys.Count) {
                    this.m_Rarity = InventoryManager.Database.raritys[rarityIndex];
                }
            }

            foreach (ObjectProperty property in this.properties)
            {
                if (data.ContainsKey(property.Name))
                {
                    object obj = data[property.Name];
                    property.SetValue(obj);
                }
            }

            if (data.ContainsKey("Reference"))
            {
                List<object> references = data["Reference"] as List<object>;
                for (int i = 0; i < references.Count; i++)
                {
                    Dictionary<string, object> referenceData = references[i] as Dictionary<string, object>;
                    string container = (string)referenceData["Container"];
                    int slot = System.Convert.ToInt32(referenceData["Slot"]);
                    ItemContainer referenceContainer = WidgetUtility.Find<ItemContainer>(container);
                    if (referenceContainer != null)
                    {
                        referenceContainer.ReplaceItem(slot, this);
                    }
                }
            }
        }

        [System.Serializable]
        public class Ingredient
        {
            [ItemPicker(true)]
            public Item item;
            public int amount = 1;
        }
    }
}