using DevionGames.UIWidgets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    public class SwapItems : MonoBehaviour
    {
        public KeyCode key = KeyCode.R;
        public ItemSlot first;
        public ItemSlot second;

        private void Update()
        {
            if (Input.GetKeyDown(key)) {
                first.Container.SwapItems(first, second);
            }
        }
    }
}