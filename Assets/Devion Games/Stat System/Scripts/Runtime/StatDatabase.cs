using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.StatSystem
{
    [System.Serializable]
    public class StatDatabase : ScriptableObject
    {
        public List<Stat> items = new List<Stat>();
        public List<StatEffect> effects = new List<StatEffect>();
        public List<Configuration.Settings> settings = new List<Configuration.Settings>();
    }
}