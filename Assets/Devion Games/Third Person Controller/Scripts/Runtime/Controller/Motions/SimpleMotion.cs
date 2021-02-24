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


        public override void OnStart()
        {
			this.m_Animator.CrossFade("Empty",0.15f);
        }

        public override bool CanStart ()
		{
			
			return (Time.time > lastUsedTime + this.m_RecurrenceDelay);
		}



       /* public override bool UpdateAnimatorIK(int layer)
        {
            return true;
        }*/

       /* private void OnEndUse() {
			Debug.Log("OnEndUse!");
            this.StopMotion(true);
        }*/
	}
}