using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [RequireComponent(typeof(ItemCollection))]
    public class ItemCollectionPopulator : MonoBehaviour
    {
        [ItemGroupPicker]
        [SerializeField]
        public ItemGroup m_ItemGroup;

        private void Start() {}

        /* private void Awake()
         {
             ItemCollection collection = GetComponent<ItemCollection>();
             collection.Initialize();
             collection.Add(InventoryManager.CreateInstances(this.m_ItemGroup));
         }*/


    }
}