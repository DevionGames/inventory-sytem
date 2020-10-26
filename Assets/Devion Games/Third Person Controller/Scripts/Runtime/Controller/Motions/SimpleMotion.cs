using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
	public class SimpleMotion: MotionState
	{
		[SerializeField]
		private float m_RecurrenceDelay = 0.2f;

		private float lastUsedTime;

		public override void OnStop ()
		{
			lastUsedTime = Time.time;
		}

		public override bool CanStart ()
		{
			return (Time.time > lastUsedTime + this.m_RecurrenceDelay);
		}

        public override bool UpdateAnimatorIK(int layer)
        {
            return false;
        }

        private void OnEndUse() {
            this.StopMotion(true);
        }
	}
}