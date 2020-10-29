using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem.Configuration
{
    [System.Serializable]
    public class Input : Settings
    {
        public override string Name
        {
            get
            {
                return "Input";
            }
        }

        [Header("Unstacking:")]
        [InspectorLabel("Event")]
        [EnumFlags]
        public UnstackInput unstackEvent = UnstackInput.OnClick | UnstackInput.OnDrag;
        [InspectorLabel("Key Code")]
        public KeyCode unstackKeyCode = KeyCode.LeftShift;

        [Flags]
        public enum UnstackInput {
            OnClick = 1,
            OnDrag = 2
        }
    }
}