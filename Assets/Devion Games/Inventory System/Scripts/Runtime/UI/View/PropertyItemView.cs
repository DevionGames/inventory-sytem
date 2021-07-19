using DevionGames.UIWidgets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    public class PropertyItemView : ItemView
    {
        /// <summary>
        /// StringPairSlot prefab to display a string - string pair 
        /// </summary>
        [SerializeField]
        protected StringPairSlot m_SlotPrefab;

        protected List<StringPairSlot> m_SlotCache = new List<StringPairSlot>();

        public override void Repaint(Item item)
        {
            if (this.m_SlotPrefab != null)
            {
                for (int i = 0; i < this.m_SlotCache.Count; i++)
                {
                    this.m_SlotCache[i].gameObject.SetActive(false);
                }
                if (item != null)
                {
                    List<KeyValuePair<string, string>> pairs = item.GetPropertyInfo();

                    if (pairs != null && pairs.Count > 0)
                    {
                        while (pairs.Count > this.m_SlotCache.Count)
                        {
                            CreateSlot();
                        }

                        for (int i = 0; i < pairs.Count; i++)
                        {
                            StringPairSlot slot = this.m_SlotCache[i];
                            slot.gameObject.SetActive(true);
                            slot.Target = pairs[i];
                        }
                        this.m_SlotPrefab.transform.parent.gameObject.SetActive(true);
                    }
                }
            }
        }
        protected virtual StringPairSlot CreateSlot()
        {
            if (this.m_SlotPrefab != null)
            {
                GameObject go = (GameObject)Instantiate(this.m_SlotPrefab.gameObject);
                go.SetActive(true);
                go.transform.SetParent(this.m_SlotPrefab.transform.parent, false);
                StringPairSlot slot = go.GetComponent<StringPairSlot>();
                this.m_SlotCache.Add(slot);

                return slot;
            }
            Debug.LogWarning("[ItemSlot] Please ensure that the slot prefab is set in the inspector.");
            return null;
        }
    }
}