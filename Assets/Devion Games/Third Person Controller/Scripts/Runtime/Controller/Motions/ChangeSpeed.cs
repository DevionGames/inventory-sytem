using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
	public class ChangeSpeed : MotionState
	{
		[SerializeField]
		private float m_SpeedMultiplier = 2f;

		private float m_PrevSpeedMutiplier;

		public override void OnStart ()
		{
			this.m_PrevSpeedMutiplier = this.m_Controller.SpeedMultiplier;
			this.m_Controller.SpeedMultiplier = this.m_SpeedMultiplier;
		}

		public override void OnStop ()
		{
			this.m_Controller.SpeedMultiplier = this.m_PrevSpeedMutiplier;


		}

		public override bool CanStop ()
		{
			return this.StopType != StopType.Automatic;
		}
	}
}