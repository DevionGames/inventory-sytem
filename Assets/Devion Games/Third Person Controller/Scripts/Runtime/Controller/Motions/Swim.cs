using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
	/// <summary>
	/// Note: Swimming will only work if the MotionTrigger/SwimTrigger size is set to 1, collider size can be any.
	/// </summary>
	public class Swim : MotionState
	{
		[SerializeField]
		private float m_HeightOffset=-1.57f;
		[SerializeField]
		private float m_OffsetSmoothing = 0.1f;
		[InspectorLabel("Trigger")]
		[SerializeField]
		private string m_TriggerName = "Ladder";

		private MotionTrigger m_Trigger;
		private float m_SmoothOffset;
		private float m_SmoothVelocity;

        private float m_HeightAdjustment = 0f;//-0.3f;
   
		public override void OnStart ()
		{
			this.m_Rigidbody.useGravity = false;
			this.m_Rigidbody.velocity = Vector3.zero;
			this.m_Controller.Velocity = Vector3.zero;
			Vector3 position = transform.position;
			position.y = m_Trigger.transform.position.y + this.m_HeightOffset;
			transform.position = position;
			this.m_Controller.IsGrounded = true;
			this.m_CapsuleCollider.height += this.m_HeightAdjustment;
			this.m_CapsuleCollider.center = new Vector3 (this.m_CapsuleCollider.center.x, this.m_CapsuleCollider.center.y + this.m_HeightAdjustment * 0.5f, this.m_CapsuleCollider.center.z);
		}

		public override void OnStop ()
		{
			this.m_Rigidbody.useGravity = true;
			this.m_CapsuleCollider.height -= this.m_HeightAdjustment;
			this.m_CapsuleCollider.center = new Vector3 (this.m_CapsuleCollider.center.x, this.m_CapsuleCollider.center.y - this.m_HeightAdjustment * 0.5f, this.m_CapsuleCollider.center.z);
	
		}

		public override bool CanStart ()
		{
			if (this.m_Trigger != null && transform.position.y < this.m_Trigger.transform.position.y + this.m_HeightOffset-0.1f) {
				return true;
			}
			return false;
		}

		public override bool CanStop ()
		{
			if (this.m_Trigger == null || transform.position.y > this.m_Trigger.transform.position.y + this.m_HeightOffset + 0.1f) {
				return true;
			}
			return false;
		}

		public override bool UpdateVelocity (ref Vector3 velocity)
		{

			if (!this.m_Controller.IsStepping) {
				Vector3 position = transform.position;
				this.m_SmoothOffset = Mathf.SmoothDamp (position.y, m_Trigger.transform.position.y + this.m_HeightOffset, ref this.m_SmoothVelocity, this.m_OffsetSmoothing);
				position.y = this.m_SmoothOffset;
				transform.position = position;
			}
			return true;
		}


		public override bool CheckGround ()
		{
			this.m_Controller.CheckStep ();
			return false;
		}

		public override bool UpdateAnimatorIK (int layer)
		{
			return false;
		}

		private void OnTriggerEnter(Collider other)
		{
			MotionTrigger trigger = other.GetComponent<MotionTrigger>();
			if (StartType == StartType.Automatic && trigger != null && (trigger.triggerName == this.m_TriggerName || trigger is SwimTrigger))
			{
				this.m_Trigger = trigger;

			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (this.m_Trigger == other.GetComponent<MotionTrigger>())
				this.m_Trigger = null;
		}
	}
}