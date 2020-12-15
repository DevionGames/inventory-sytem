using DevionGames.UIWidgets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    public class ItemContainerPopulator : MonoBehaviour
    {
        [SerializeField]
        protected List<Entry> m_Entries = new List<Entry>();


        protected virtual void Start() {
            if (!InventoryManager.HasSavedData()) {
                for (int i = 0; i < this.m_Entries.Count; i++) {
                    ItemContainer container = WidgetUtility.Find<ItemContainer>(this.m_Entries[i].name);
                    if (container != null) {
                        Item[] groupItems = InventoryManager.CreateInstances(this.m_Entries[i].group);
                        for (int j = 0; j < groupItems.Length; j++) {
                            container.StackOrAdd(groupItems[j]);
                        }
                    }
                }
            }
        }

        [System.Serializable]
        public class Entry {
            public string name = "Inventory";
            [ItemGroupPicker]
            public ItemGroup group;
        }
    }
}