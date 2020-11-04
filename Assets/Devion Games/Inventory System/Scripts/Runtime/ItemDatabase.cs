using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DevionGames.InventorySystem{
	[System.Serializable]
	public class ItemDatabase : ScriptableObject {
		public List<Item> items = new List<Item>();
        public List<Currency> currencies = new List<Currency>();
        public List<Rarity> raritys = new List<Rarity>();
		public List<Category> categories = new List<Category>();
		public List<EquipmentRegion> equipments = new List<EquipmentRegion>();
        public List<ItemGroup> itemGroups = new List<ItemGroup>();
        public List<Configuration.Settings> settings = new List<Configuration.Settings>();

		public List<string> folders = new List<string>();
		public List<Item> allItems {
			get {
				List<Item> all = new List<Item>(items);
				all.AddRange(currencies);
				return all;
			}
		}

		public GameObject GetItemPrefab(string name){
			for (int i=0; i< items.Count; i++) {
				Item item=items[i];
				if(item != null && item.Prefab != null && item.Prefab.name == name){
					return item.Prefab;
				}
			}
			return null;
		}

		public ItemGroup GetItemGroup(string name) {
			return itemGroups.First(x => x.Name == name);
		}
	}
}