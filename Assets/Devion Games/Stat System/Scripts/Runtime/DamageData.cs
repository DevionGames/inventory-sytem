using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.StatSystem
{
    [CreateAssetMenu(fileName = "DamageData", menuName = "Devion Games/Stat System/Damage Data")]
    [System.Serializable]
    public class DamageData : ScriptableObject
    {
        [HeaderLine("Default")]
        public string sendingStat = "Damage";
        public string criticalStrikeStat = "Critical Strike";
        public string receivingStat = "Health";

        public float maxDistance = 2f;
        public float maxAngle = 60f;

        [HeaderLine("Particles")]
        public GameObject particleEffect;
        public Vector3 offset = Vector3.up;
        public Vector3 randomize = new Vector3(0.2f, 0.1f);
        public float lifeTime = 3f;

        [HeaderLine("Sounds")]
        public float volume = 0.7f;
        public AudioClip[] hitSounds;

        [HeaderLine("Camera Shake")]
        public float duration = 0.4f;
        public float speed = 5f;
        public Vector3 amount = new Vector3(0.4f,0.4f);
    }
}