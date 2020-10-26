using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
	public class ChangeHeight : MotionState
	{
		[SerializeField]
		private float m_HeightAdjustment = -0.8f;

		public override void OnStart ()
		{
			this.m_CapsuleCollider.height += this.m_HeightAdjustment;
			this.m_CapsuleCollider.center = new Vector3 (this.m_CapsuleCollider.center.x, this.m_CapsuleCollider.center.y + this.m_HeightAdjustment * 0.5f, this.m_CapsuleCollider.center.z);
		}

		public override void OnStop ()
		{
			this.m_CapsuleCollider.height -= this.m_HeightAdjustment;
			this.m_CapsuleCollider.center = new Vector3 (this.m_CapsuleCollider.center.x, this.m_CapsuleCollider.center.y - this.m_HeightAdjustment * 0.5f, this.m_CapsuleCollider.center.z);
		}
	}
}