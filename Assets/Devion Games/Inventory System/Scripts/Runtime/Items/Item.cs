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

        [Tooltip("Unique name of the item. It can be used to display the item name in UI.")]
		[SerializeField]
		private string m_ItemName = "New Item";

		public string Name {
			get{ return this.m_ItemName; }
			set{ this.m_ItemName = value; }
		}

        [Tooltip("If set to true the Name setting will be used to display the items name in UI.")]
        [SerializeField]
        private bool m_UseItemNameAsDisplayName = true;
        [Tooltip("Items name to display in UI.")]
        [SerializeField]
        private string m_DisplayName = "New Item";

        public string DisplayName
        {
            get {
                string displayName = m_UseItemNameAsDisplayName ? this.m_ItemName : this.m_DisplayName;
                if (Rarity.UseAsNamePrefix)
                    displayName = Rarity.Name + " " + displayName;
                return displayName; 
            
            }
            set {
                this.m_DisplayName = value; 
            }
        }

        [Tooltip("The icon that can be shown in various places of the UI. Tooltip, vendor and many more. ")]
        [SerializeField]
		private Sprite m_Icon;

		public Sprite Icon {
			get{ return this.m_Icon; }
			set{ this.m_Icon = value; }
		}

        [Tooltip("The prefab to instantiate when an item is draged out of a container. This prefab is also used to place the item in scene, so the player can pickup the item.")]
		[SerializeField]
		private GameObject m_Prefab;

		public GameObject Prefab {
			get{ return m_Prefab; }
			set{ this.m_Prefab = value; }
		}

        [Tooltip("Item description is used in the UI. Tooltip, vendor, spells, crafting...")]
		[SerializeField]
		[Multiline (4)]
		private string m_Description = string.Empty;

		public string Description {
			get{ return this.m_Description; }
		}


        [Tooltip("The category the item belongs to. Used to sort the items collection in editor or also at runtime in the UI.")]
		[Header ("Behaviour:")]
        [SerializeField]
        [CategoryPicker]
        private Category m_Category = null;

        public Category Category
        {
            get { return this.m_Category; }
            set { this.m_Category = value; }
        }

        private static Rarity m_DefaultRarity;
        private static Rarity DefaultRarity {
            get {
                if (Item.m_DefaultRarity is null) {
                    //TODO make not deletable in editor
                    Item.m_DefaultRarity = ScriptableObject.CreateInstance<Rarity>();
                    Item.m_DefaultRarity.Name = "None";
                    Item.m_DefaultRarity.Color = Color.grey;
                    Item.m_DefaultRarity.Chance = 100;
                    Item.m_DefaultRarity.Multiplier = 1.0f;
                }
                return Item.m_DefaultRarity;
             }
        }

        private Rarity m_Rarity;
		public Rarity Rarity {
			get{
                if (this.m_Rarity == null ) {
                    this.m_Rarity = DefaultRarity;
                }
                return this.m_Rarity; 
            }
            set { this.m_Rarity = value; }
		}

        /*[SerializeField]
        private List<Rarity> m_PossibleRarity=new List<Rarity>();
        public List<Rarity> PossibleRarity {
            get { return this.m_PossibleRarity; }
            set { this.m_PossibleRarity = value; }
        }*/

        [Tooltip("Is this item sellable to a vendor? More options will appear if it is sellable.")]
        [SerializeField]
        private bool m_IsSellable = true;
        public bool IsSellable {
            get { return this.m_IsSellable; }
            set { this.m_IsSellable = true; }
        }

        [Tooltip("Items buy price. This value will be multiplied with the rarities price multiplier.")]
		[SerializeField]
		private int m_BuyPrice=0;

		public int BuyPrice {
			get{ return Mathf.RoundToInt(m_BuyPrice*Rarity.PriceMultiplier); }
           // set { this.m_BuyPrice = value; }
		}

        [Tooltip("If set to true, this item will be added to the vendors inventory and the player can buy it back.")]
        [SerializeField]
        private bool m_CanBuyBack = true;
        public bool CanBuyBack { get { return this.m_CanBuyBack; } }

        [Tooltip("The buy currency. You can also use a lower currency, it will be auto converted. 120 Copper will be converted to 1 Silver and 20 Copper.")]
        [CurrencyPicker(true)]
        [SerializeField]
        private Currency m_BuyCurrency=null;

        public Currency BuyCurrency
        {
            get { return this.m_BuyCurrency; }
            set { this.m_BuyCurrency = value; }
        }
        [Tooltip("Items sell price. This value will be multiplied with the rarities price multiplier.")]
        [SerializeField]
		private int m_SellPrice=0;

		public int SellPrice {
			get{ return Mathf.RoundToInt(this.m_SellPrice*Rarity.PriceMultiplier); }
		}

        [Tooltip("The sell currency. You can also use a lower currency, it will be auto converted. 120 Copper will be converted to 1 Silver and 20 Copper.")]
        [CurrencyPicker(true)]
        [SerializeField]
        private Currency m_SellCurrency= null;

        public Currency SellCurrency
        {
            get { return this.m_SellCurrency; }
            set { this.m_SellCurrency = value; }
        }

        [Tooltip("Items stack definition. New created items will have this stack. Use a stack modifier to randomize the stack.")]
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

        [Tooltip("Maximum stack amount. Items stack can't be higher then this value. If the stack is bigger then the maximum stack, the item will be splitted into multiple stacks.")]
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

        [Tooltip("If set to true, the item is droppable from a container to the scene.")]
		[SerializeField]
		private bool m_IsDroppable = true;

		public bool IsDroppable {
			get{ return this.m_IsDroppable; }
		}

        [Tooltip("Sound that should be played when the item is dropped to the scene.")]
		[SerializeField]
		private AudioClip m_DropSound = null;

		public AudioClip DropSound {
			get{ return this.m_DropSound; }
		}

        [Tooltip("By default items prefab will be instantiated when dropped to the scene, you can override this option.")]
        [SerializeField]
		private GameObject m_OverridePrefab=null;

		public GameObject OverridePrefab {
			get{ return this.m_OverridePrefab; }
		}

        [Tooltip("Defines if the item is craftable.")]
        //TODO Move all to CraftingData class
        [SerializeField]
        private bool m_IsCraftable=false;

        public bool IsCraftable
        {
            get { return this.m_IsCraftable; }
        }

        [Tooltip("How long does it take to craft this item. This value is also used to display the progressbar in crafting UI.")]
        [SerializeField]
        private float m_CraftingDuration = 2f;

        public float CraftingDuration
        {
            get { return this.m_CraftingDuration; }
        }

        [Tooltip("Should a skill be used when item is crafted?")]
        [SerializeField]
        private bool m_UseCraftingSkill = false;

        public bool UseCraftingSkill {
            get { return this.m_UseCraftingSkill; }
        }

        [Tooltip("Name of the skill window. It is required if use crafting skill is set to true to be able to find the skill. ")]
        [SerializeField]
        private string m_SkillWindow = "Skills";
        public string SkillWindow {
            get { return this.m_SkillWindow; }
        }

        [Tooltip("What skill should be used when crafting? The current players skill will be searched in skill window set above.")]
        [ItemPicker(true)]
        [SerializeField]
        private Skill m_CraftingSkill = null;
        public Skill CraftingSkill {
            get { return this.m_CraftingSkill; }
        }

        [Tooltip("Remove the ingredients when crafting fails.")]
        [SerializeField]
        private bool m_RemoveIngredientsWhenFailed = false;
        public bool RemoveIngredientsWhenFailed {
            get { return this.m_RemoveIngredientsWhenFailed; }
        }

        [Tooltip("Minimum required skill to craft this item. The player can only craft this item if his skill is high enough.")]
        [Range(0f,100f)]
        [SerializeField]
        private float m_MinCraftingSkillValue=0f;

        public float MinCraftingSkillValue {
            get { return this.m_MinCraftingSkillValue; }
        }

        [Tooltip("Animation to play when this item is crafted. If you don't want to play any animation, delete this value.")]
        [SerializeField]
        private string m_CraftingAnimatorState= "Blacksmithy";

        public string CraftingAnimatorState
        {
            get { return this.m_CraftingAnimatorState; }
        }

        [SerializeField]
        private ItemModifierList m_CraftingModifier= new ItemModifierList();
        public ItemModifierList CraftingModifier {
            get { return this.m_CraftingModifier; }
            set { this.m_CraftingModifier = value; }
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

        public void AddProperty(string name, object value) {
            ObjectProperty property = new ObjectProperty();
            property.Name = name;
            property.SetValue(value);
            properties.Add(property);
        }

        public void RemoveProperty(string name)
        {
            properties.RemoveAll(x => x.Name == name);
        }

        public ObjectProperty FindProperty (string name)
		{
			return properties.FirstOrDefault (property => property.Name == name);
		}


		public ObjectProperty[] GetProperties ()
		{
			return properties.ToArray ();
		}

		public void SetProperties (ObjectProperty[] properties)
		{
			this.properties = new List<ObjectProperty> (properties);
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
            else if (property.SerializedType== typeof(string)) {
                propertyValue = property.stringValue;
            }
            else {
                propertyValue = ((UnityTools.IsNumeric(property.GetValue()) && System.Convert.ToSingle(property.GetValue()) > 0f) ? "+" : "-");
                propertyValue += (UnityTools.IsNumeric(property.GetValue()) ? Mathf.Abs(System.Convert.ToSingle(property.GetValue())) : property.GetValue()).ToString();
            }
            propertyValue = UnityTools.ColorString(propertyValue, property.color);
            return propertyValue;
        }

        public virtual void Use() { }

        public virtual void GetObjectData(Dictionary<string, object> data)
        {
            data.Add("Name", this.Name);
            data.Add("Stack", this.Stack);
            data.Add("RarityIndex", InventoryManager.Database.raritys.IndexOf(Rarity));

            if (Slot != null)
            {
                data.Add("Index", this.Slot.Index);
            }

            List<object> objectProperties = new List<object>();

            foreach (ObjectProperty property in properties)
            {
                Dictionary<string, object> propertyData = new Dictionary<string, object>();
                if (!typeof(UnityEngine.Object).IsAssignableFrom(property.SerializedType))
                {
                    propertyData.Add("Name", property.Name);
                    propertyData.Add("Value", property.GetValue());
                    objectProperties.Add(propertyData);
                }
            }
            data.Add("Properties",objectProperties);

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

        public virtual void SetObjectData(Dictionary<string, object> data)
        {
            this.Stack = System.Convert.ToInt32(data["Stack"]);
            if (data.ContainsKey("RarityIndex")) {
                int rarityIndex =System.Convert.ToInt32(data["RarityIndex"]);
                if (rarityIndex > -1 && rarityIndex < InventoryManager.Database.raritys.Count) {
                    this.m_Rarity = InventoryManager.Database.raritys[rarityIndex];
                }
            }

            if (data.ContainsKey("Properties"))
            {
                List<object> objectProperties = data["Properties"] as List<object>;
                for (int i = 0; i < objectProperties.Count; i++)
                {
                    Dictionary<string, object> propertyData = objectProperties[i] as Dictionary<string, object>;
                    string propertyName = (string)propertyData["Name"];
                    object propertyValue = propertyData["Value"];
                    ObjectProperty property = FindProperty(propertyName);
                    if (property == null) {
                        property = new ObjectProperty();
                        property.Name = propertyName;
                        properties.Add(property);
                    }
                    property.SetValue(propertyValue);

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