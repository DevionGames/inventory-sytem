using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.StatSystem
{
    [CreateAssetMenu(fileName = "DamageData", menuName = "Devion Games/Stat System/Damage Data")]
    [System.Serializable]
    public class DamageData : ScriptableObject
    {
        public string sendingStat = "Damage";
        public string receivingStat = "Health";

        public float maxDistance = 2f;
        public float maxAngle = 60f;
    }
}