using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DevionGames.InventorySystem
{
    public class LoadSaveMenu : MonoBehaviour
    {
        public Transform grid;
        public Button slotPrefab;

        private void Start()
        {
            if (InventoryManager.current != null) {
                InventoryManager.current.onDataSaved.AddListener(UpdateLoadingStates);
            }
            UpdateLoadingStates();
        }

        private void UpdateLoadingStates() {
            List<Button> slots = grid.GetComponentsInChildren<Button>().ToList();
            slots.Remove(slotPrefab);
            for (int i = 0; i < slots.Count; i++) {
                DestroyImmediate(slots[i].gameObject);
            }

            List<string> keys = PlayerPrefs.GetString("InventorySystemSavedKeys").Split(';').ToList();
            keys.RemoveAll(x => string.IsNullOrEmpty(x));
            keys.Reverse();

            for (int i = 0; i < keys.Count; i++) {
                string key = keys[i];
                Button button = CreateSlot(key);
                button.onClick.AddListener(()=>{InventoryManager.Load(key); });

            }
        }

        public void Save() {
            InventoryManager.Save(DateTime.UtcNow.ToString());
           
        }

        public Button CreateSlot(string name)
        {
            if (slotPrefab != null && grid != null)
            {
                GameObject go = (GameObject)Instantiate(slotPrefab.gameObject);
                Text text = go.GetComponentInChildren<Text>();
                text.text = name;
                go.SetActive(true);
                go.transform.SetParent(grid, false);
                return go.GetComponent<Button>() ;
            }
            return null;
        }
    }
}