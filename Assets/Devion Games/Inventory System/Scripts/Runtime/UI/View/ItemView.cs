using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    public abstract class ItemView : MonoBehaviour
    {
        protected virtual void Start() { }
        public abstract void Repaint(Item item);
        public virtual bool RequiresConstantRepaint() { return false; }
    }
}