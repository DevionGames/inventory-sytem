using DevionGames.UIWidgets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    public abstract class Restriction : MonoBehaviour
    {
        protected Slot slot;
        protected ItemContainer container;

        private void Start()
        {
            slot = GetComponent<Slot>();
            if (slot != null){
                container = slot.Container;
            }else {
                container = GetComponent<ItemContainer>();
            }
        }

        public abstract bool CanAddItem(Item item);

    }
}