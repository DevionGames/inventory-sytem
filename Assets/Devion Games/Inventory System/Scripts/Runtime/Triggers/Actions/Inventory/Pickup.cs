using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevionGames.UIWidgets;

namespace DevionGames.InventorySystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon("Item")]
    [ComponentMenu("Inventory System/Pickup Item")]
    [RequireComponent(typeof(ItemCollection))]
    public class Pickup : Action
    {
        [SerializeField]
        private string m_WindowName = "Inventory";
        [SerializeField]
        private bool m_DestroyWhenEmpty = true;
        [SerializeField]
        private int m_Amount = -1;

        private ItemCollection m_ItemCollection;

        public override void OnStart()
        {
            this.m_ItemCollection = gameObject.GetComponent<ItemCollection>();
            this.m_ItemCollection.onChange.AddListener(delegate () {
                if (this.m_ItemCollection.IsEmpty && this.m_DestroyWhenEmpty)
                {
                    DisplayTriggerTooltip tooltip = gameObject.GetComponent<DisplayTriggerTooltip>();
                    if (tooltip != null) {
                        GameObject.Destroy(tooltip);
                    }
                    GameObject.Destroy(gameObject,0.1f);
                }
            });

        }

        public override ActionStatus OnUpdate()
        {
            return PickupItems() ;
        }

        private ActionStatus  PickupItems()
        {
            if (this.m_ItemCollection.Count == 0) {
                InventoryManager.Notifications.empty.Show(gameObject.name.Replace("(Clone)", "").ToLower());
                return ActionStatus.Failure;
            }
            ItemContainer[] windows = WidgetUtility.FindAll<ItemContainer>(this.m_WindowName);
            List<Item> items = new List<Item>();
            if (this.m_Amount < 0)
            {
                items.AddRange(this.m_ItemCollection);
            }
            else
            {
                for (int i = 0; i < this.m_Amount; i++)
                {
                    Item item = this.m_ItemCollection[Random.Range(0, this.m_ItemCollection.Count)];
                    items.Add(item);
                }
            }

            for (int i = 0; i < items.Count; i++)
            {
                Item item = items[i];
                if (windows.Length > 0)
                {
                    for (int j = 0; j < windows.Length; j++)
                    {
                        ItemContainer current = windows[j];

                        if (current.StackOrAdd(item))
                        {
                            this.m_ItemCollection.Remove(item);
                            break;
                        }
                    }
                }
                else
                {
                    //Drop items to ground
                    DropItem(item);
                    this.m_ItemCollection.Remove(item);
                }
            }

            return ActionStatus.Success;
        }

        private void DropItem(Item item)
        {
            GameObject prefab = item.OverridePrefab != null ? item.OverridePrefab : item.Prefab;
            float angle = Random.Range(0f, 360f);
            float x = (float)(InventoryManager.DefaultSettings.maxDropDistance * Mathf.Cos(angle * Mathf.PI / 180f)) + gameObject.transform.position.x;
            float z = (float)(InventoryManager.DefaultSettings.maxDropDistance * Mathf.Sin(angle * Mathf.PI / 180f)) + gameObject.transform.position.z;
            Vector3 position = new Vector3(x, gameObject.transform.position.y, z);

            RaycastHit hit;
            if (Physics.Raycast(position, Vector3.down, out hit)) {
                position = hit.point+ Vector3.up;
            }

            GameObject go = InventoryManager.Instantiate(prefab, position, Random.rotation);
            ItemCollection collection = go.GetComponent<ItemCollection>();
            if (collection != null)
            {
                collection.Clear();
                collection.Add(item);
            }
        }
    }
}