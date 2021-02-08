using DevionGames.UIWidgets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    public class SwapItems : MonoBehaviour
    {
        public ItemSlot first;
        public ItemSlot second;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R)) {
                first.Container.SwapItems(first, second);
            }
        }
    }
}