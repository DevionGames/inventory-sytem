using DevionGames.UIWidgets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DevionGames.InventorySystem
{
    public class SaveLoadMenu : UIWidget
    {
        /// <summary>
        /// The parent transform of slots. 
        /// </summary>
        [Header("Reference")]
        [Tooltip("The parent transform of slots.")]
        [SerializeField]
        protected Transform m_SlotParent;
        /// <summary>
        /// The slot prefab. This game object should contain the Slot component or a child class of Slot. 
        /// </summary>
        [Tooltip("The slot prefab. This game object should contain the SaveLoadSlot component.")]
        [SerializeField]
        protected SaveLoadSlot m_SlotPrefab;

        private void Start()
        {
            if (InventoryManager.current != null)
            {
                InventoryManager.current.onDataSaved.AddListener(UpdateStates);
            }

            UpdateStates();
        }

        private void UpdateStates()
        {
            List<SaveLoadSlot> slots = this.m_SlotParent.GetComponentsInChildren<SaveLoadSlot>().ToList();
            slots.Remove(this.m_SlotPrefab);
            for (int i = 0; i < slots.Count; i++)
            {
                DestroyImmediate(slots[i].gameObject);
            }

            List<string> keys = PlayerPrefs.GetString("InventorySystemSavedKeys").Split(';').ToList();
            keys.RemoveAll(x => string.IsNullOrEmpty(x));
            keys.Reverse();

            for (int i = keys.Count - 1; i >= 0; i--)
            {
                string key = keys[i];
                SaveLoadSlot slot = CreateSlot(key);
            }
        }

        public void Save()
        {
            InventoryManager.Save(DateTime.UtcNow.ToString());

        }

        public SaveLoadSlot CreateSlot(string name)
        {
            if (this.m_SlotPrefab != null && this.m_SlotParent != null)
            {
                GameObject go = (GameObject)Instantiate(this.m_SlotPrefab.gameObject);
                Text text = go.GetComponentInChildren<Text>();
                text.text = name;
                go.SetActive(true);
                go.transform.SetParent(this.m_SlotParent, false);
                return go.GetComponent<SaveLoadSlot>();
            }
            return null;
        }
    }
}