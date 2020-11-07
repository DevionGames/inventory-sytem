using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem.Configuration
{
    [System.Serializable]
    public class Default : Settings
    {
        public override string Name
        {
            get
            {
                return "Default";
            }
        }


        public string playerTag = "Player";
        public float maxDropDistance = 3f;

        [Header("Physics")]
        public bool queriesHitTriggers = false;

        [Header("Debug")]
        public bool debugMessages = true;
        public bool showAllComponents = false;
    }
}