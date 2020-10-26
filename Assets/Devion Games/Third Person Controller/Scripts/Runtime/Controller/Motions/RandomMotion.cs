using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
	public class RandomMotion : SimpleMotion
	{
		[SerializeField]
		private string[] m_DestinationStates = null;

		public override string GetDestinationState ()
		{
			return this.m_DestinationStates [Random.Range (0, this.m_DestinationStates.Length)];
		}
	}
}