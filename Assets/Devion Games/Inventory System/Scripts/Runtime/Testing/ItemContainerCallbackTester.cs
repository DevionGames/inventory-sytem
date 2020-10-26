using System.Collections;
using System.Collections.Generic;
using DevionGames;
using DevionGames.InventorySystem;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemContainerCallbackTester : MonoBehaviour
{
    private void Awake()
    {
        ItemContainer container = GetComponent<ItemContainer>();
        container.OnRemoveItem += (Item item, int amount, Slot slot) =>
        {
            Debug.Log("[" + Time.time + "]" + "OnRemoveItem: " + item.Name + " Amount: " + amount + " Container: " + slot.Container.Name + " Slot: " + slot.Index);
        };

        container.OnAddItem += (Item item, Slot slot) =>
        {
            Debug.Log("[" + Time.time + "]" + "OnAddItem: " + item.Name + " Amount: " + item.Stack + "Container: " + slot.Container.Name + " Slot: " + slot.Index);
        };

        container.OnFailedToAddItem += (Item item) =>
        {
            Debug.Log("OnFailedToAddItem: " + item.Name);
        };

        container.OnFailedToRemoveItem += (Item item, int amount) =>
        {
            Debug.Log("OnFailedToRemoveItem: " + item.Name + " Amount: " + amount);
        };
    }
}
