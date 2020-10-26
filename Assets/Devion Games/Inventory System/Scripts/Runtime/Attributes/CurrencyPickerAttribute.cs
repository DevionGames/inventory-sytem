using UnityEngine;
using System;
using System.Collections;

namespace DevionGames.InventorySystem
{
    public class CurrencyPickerAttribute : PickerAttribute
    {

        public CurrencyPickerAttribute() : this(false) { }

        public CurrencyPickerAttribute(bool utility) : base(utility) { }
    }
}