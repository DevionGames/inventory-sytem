using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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
        [Range(0f,360f)]
        public float maxAngle = 60f;
        public bool displayDamage = false;
        [Compound("displayDamage")]
        public GameObject damagePrefab;
        [Compound("displayDamage")]
        public Color damageColor = Color.yellow;
        [Compound("displayDamage")]
        public Color criticalDamageColor = Color.red;
        [Compound("displayDamage")]
        public Vector3 intensity = new Vector3(3f, 2f, 0f);

        [HeaderLine("Particles")]
        public GameObject particleEffect;
        public Vector3 offset = Vector3.up;
        public Vector3 randomize = new Vector3(0.2f, 0.1f);
        public float lifeTime = 3f;

        [HeaderLine("Sounds")]
        public AudioMixerGroup audioMixerGroup;
        [Range(0f,1f)]
        public float volumeScale = 0.7f;
        public AudioClip[] hitSounds;

        [HeaderLine("Camera Shake")]
        [InspectorLabel("Enabled")]
        public bool enableShake;
        [Compound("enableShake")]
        public float duration = 0.4f;
        [Compound("enableShake")]
        public float speed = 5f;
        [Compound("enableShake")]
        public Vector3 amount = new Vector3(0.4f,0.4f);

        [HeaderLine("Knockback")]
        [InspectorLabel("Enabled")]
        public bool enableKnockback;
        [Compound("enableKnockback")]
        public float knockbackChance = 0.7f;
        [Compound("enableKnockback")]
        public float knockbackStrength = 30f;
        [Compound("enableKnockback")]
        public float knockbackAcceleration = 50f;
        [Compound("enableKnockback")]
        public float knockbackDuration = 1f;

        [System.NonSerialized]
        public GameObject sender;
    }
}