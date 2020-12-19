using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DevionGames.InventorySystem
{
    [RequireComponent(typeof(ItemCollection))]
    public class ItemGroupGenerator : MonoBehaviour, IGenerator
    {
        [ItemGroupPicker]
        [SerializeField]
        private ItemGroup m_From = null;
        [SerializeField]
        private List<ScriptableObject> m_Filters = new List<ScriptableObject>();
        [SerializeField]
        private int m_MinStack = 1;
        [SerializeField]
        private int m_MaxStack = 1;
        [SerializeField]
        private int m_MinAmount = 1;
        [SerializeField]
        private int m_MaxAmount = 1;

        [Range(0f,1f)]
        [SerializeField]
        private float m_Chance = 1.0f;

        [SerializeField]
        private ItemModifierList m_Modifiers = new ItemModifierList();


        private void Start()
        {
            List<Item> items = GenerateItems();
            ItemCollection collection = GetComponent<ItemCollection>();
            collection.Add(items.ToArray());
        }


        public List<Item> GenerateItems() {
            List<Item> items = InventoryManager.Database.items;
            if (this.m_From != null) {
                items = new List<Item>(this.m_From.Items);
            }


            for (int i = 0; i < this.m_Filters.Count; i++) {
                if (this.m_Filters[i] is Category) {
                    items = items.Where(x => x.Category != null &&( x.Category.Name == (this.m_Filters[i] as Category).Name)).ToList();
                }
                if (this.m_Filters[i] is Rarity)
                {
                    items = items.Where(x => x.Rarity.Name == (this.m_Filters[i] as Rarity).Name).ToList();
                }
            }
            

            List<Item> generatedItems = new List<Item>();
            if (items.Count < 1) { return generatedItems; }

            int amount = Random.Range(this.m_MinAmount, this.m_MaxAmount+1);

            for (int i = 0; i < amount; i++) {
                if (Random.value > this.m_Chance) {
                    continue;
                }

                Item item = items[Random.Range(0, items.Count)];
                int stack = Random.Range(this.m_MinStack,this.m_MaxStack);
                item = Instantiate(item);
                item.Stack = stack;

                this.m_Modifiers.Modify(item);

                if (item.IsCraftable)
                {
                    for (int j = 0; j < item.ingredients.Count; j++)
                    {
                        item.ingredients[j].item = Instantiate(item.ingredients[j].item);
                        item.ingredients[j].item.Stack = item.ingredients[j].amount;
                    }
                }
                generatedItems.Add(item);


            }
            return generatedItems;
        }
    }
}