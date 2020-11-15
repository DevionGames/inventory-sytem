using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
	public class Jump : MotionState, IControllerGrounded
	{
		[SerializeField]
		private float m_Force = 5f;
		[SerializeField]
		private float m_RecurrenceDelay = 0.2f;


		private float jumpTime;
		private float lastJumpTime;

		public override void OnStart ()
		{
			StartJump ();
		}


        public override bool UpdateAnimator ()
		{
            this.m_Animator.SetFloat ("Float Value", this.m_Rigidbody.velocity.y, 0.15f, Time.deltaTime);
			return true;
		}

		private void StartJump ()
		{
			if (this.IsActive) {
				this.jumpTime = Time.time;
				this.m_Controller.IsGrounded = false;
				Vector3 velocity = this.m_Rigidbody.velocity;
				velocity.y = m_Force;
				this.m_Rigidbody.velocity = velocity;
				this.m_Animator.SetFloat ("Float Value", this.m_Rigidbody.velocity.y);
                float cycle = 0f;
                if (this.m_Controller.IsMoving)
                {
                    float normalizedTime = this.m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;
                    cycle = Mathf.Sin(360f * normalizedTime);
                }
                this.m_Animator.SetFloat("Leg", cycle);
                if (this.m_Controller.RawInput.z < 0f)
                {
                    this.m_Animator.SetBool("Bool Value", true);
                }else {
                    this.m_Animator.SetBool("Bool Value", false);
                }
            }
		}

		public override bool CheckGround ()
		{
			if (Time.time > jumpTime + 0.2f) {
				return true;
			}
			return false;
		}

        public override bool CheckStep()
        {
			return false;
        }

        public void OnControllerGrounded (bool grounded)
		{
			if (grounded) {
				this.lastJumpTime = Time.time;
				this.StopMotion (true);
			}
		}

		public override bool CanStart ()
		{
			return this.m_Controller.IsGrounded && (Time.time > lastJumpTime + this.m_RecurrenceDelay);
		}

		public override bool CanStop ()
		{
		
			return !this.m_Controller.IsGrounded && this.m_Rigidbody.velocity.y < 0.01f && Time.time > jumpTime + 0.2f; 
		}
	}
}