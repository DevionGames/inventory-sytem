using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [RequireComponent(typeof(ItemCollection))]
    public class ItemGenerator : MonoBehaviour, IGenerator
    {
        [SerializeField]
        private List<ItemGeneratorData> m_ItemGeneratorData=new List<ItemGeneratorData>();
        [SerializeField]
        private int m_MaxAmount = 1;
        [Tooltip("Refill time in seconds.")]
        [SerializeField]
        private float m_Refill = -1;


        private void Start()
        {
            ItemCollection collection = GetComponent<ItemCollection>();
            collection.Add(GenerateItems().ToArray());
            if (m_Refill > 0) {
                StartCoroutine(Refill(collection));
            }
        }

        private IEnumerator Refill(ItemCollection collection) {
            while (true) {
                if (collection.IsEmpty) {
                    yield return new WaitForSeconds(m_Refill);
                    collection.Add(GenerateItems().ToArray());
                }
                yield return new WaitForSeconds(3f);
            }
        }

        private List<Item> GenerateItems() {
            List<Item> generatedItems = new List<Item>();
            IEnumerable<int> indices = Enumerable.Range(0, this.m_ItemGeneratorData.Count).OrderBy(x=> rng.Next());

            foreach (int index in indices) {
                if (generatedItems.Count >= this.m_MaxAmount){
                    break;
                }
                ItemGeneratorData data = this.m_ItemGeneratorData[index];
                if (Random.value > data.chance){
                    continue;
                }
                Item item = data.item;
                int stack = Random.Range(data.minStack, data.maxStack + 1);
                stack = Mathf.Clamp(stack, item.Stack, item.MaxStack);

                item = InventoryManager.CreateInstance(item);
                item.Stack = stack;
                data.modifiers.Modify(item);
                generatedItems.Add(item);
            }
            return generatedItems;
        }

        private System.Random rng = new System.Random();

     
    }


}