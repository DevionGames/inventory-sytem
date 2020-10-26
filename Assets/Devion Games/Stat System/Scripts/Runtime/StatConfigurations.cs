using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace DevionGames.StatSystem
{
	[System.Serializable]
	public class StatConfigurations : ScriptableObject
	{
		public List<Configuration.Settings> settings = new List<Configuration.Settings>();
	}
}