using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    public interface IModifier<T> 
    {
        void Modify(T item);
    }
}