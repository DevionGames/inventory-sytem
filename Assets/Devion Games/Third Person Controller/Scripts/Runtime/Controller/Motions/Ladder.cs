using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    public class Ladder : MotionState
    {
		[InspectorLabel("Trigger")]
		[SerializeField]
		private string m_TriggerName = "Ladder";
		private MotionTrigger m_Trigger;
		private bool m_StartMove = false;
        public override void OnStart()
        {
			GetComponent<CharacterIK>().enabled = false;
			this.m_Rigidbody.useGravity = false;
			this.m_Rigidbody.velocity = Vector3.zero;
			this.m_Controller.Velocity = Vector3.zero;
			this.m_Controller.IsGrounded = false;
			this.m_StartMove = false;
			Vector3 startPosition = this.m_Trigger.transform.position;
			startPosition.y = 0.186f;
			this.m_Animator.SetFloat("Forward Input", 0f);
			MoveToTarget(this.m_Transform, startPosition, this.m_Trigger.transform.rotation, 0.3f, delegate {this.m_StartMove = true; });
	
		}

        public override void OnStop()
        {
			this.m_Rigidbody.useGravity = true;
			GetComponent<CharacterIK>().enabled = true;
		}

        public override bool CanStart()
		{
			return this.m_Trigger != null && this.m_Controller.RawInput.z > 0f;
		}

        public override bool CanStop()
        {
            return (this.m_Trigger== null || this.m_Controller.IsGrounded && this.m_Controller.RawInput.z < 0f);
        }

		public override bool UpdateVelocity(ref Vector3 velocity)
		{
			if(this.m_StartMove)
				velocity = this.m_Controller.RootMotionForce;
			return false;
		}

        public override bool UpdateAnimator()
        {
			if (this.m_StartMove)
			{
				this.m_Animator.SetFloat("Forward Input", this.m_Controller.RawInput.z);
			}
            return false;
        }

        public override bool CheckGround()
        {
            return this.m_Controller.RawInput.z < 0f;
        }

        public override bool UpdateRotation()
        {
            return false;
        }


		private void OnTriggerEnter(Collider other)
		{
			MotionTrigger trigger = other.GetComponent<MotionTrigger>();
			if (StartType == StartType.Automatic && trigger != null && trigger.triggerName == this.m_TriggerName )
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