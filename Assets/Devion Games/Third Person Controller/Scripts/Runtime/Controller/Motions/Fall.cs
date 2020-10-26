using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
	public class Fall : MotionState, IControllerGrounded
	{
		[SerializeField]
		private float m_GravityMultiplier = 2f;
		[SerializeField]
		private float m_FallMinHeight = 0.3f;

		public override void OnStart ()
		{
			this.m_Animator.SetInteger ("Int Value", 0);
            if (this.m_Controller.RawInput.z < 0f)
            {
                this.m_Animator.SetBool("Bool Value", true);
            }
            else
            {
                this.m_Animator.SetBool("Bool Value", false);
            }
        }

		public override bool UpdateVelocity (ref Vector3 velocity)
		{
			Vector3 extraGravityForce = (Physics.gravity * this.m_GravityMultiplier) - Physics.gravity;
			velocity += extraGravityForce * Time.deltaTime;
			return true;
		}

		public override bool UpdateAnimator ()
		{
			this.m_Animator.SetFloat ("Float Value", this.m_Rigidbody.velocity.y, 0.15f, Time.deltaTime);
			return true;
		}

		public override bool CanStart ()
		{
			RaycastHit hitInfo;
			if (this.m_FallMinHeight != 0f && Physics.Raycast (this.m_Transform.position + this.m_Transform.up, -this.m_Transform.up, out hitInfo, this.m_Transform.up.y + this.m_FallMinHeight) && hitInfo.distance < this.m_Transform.up.y + this.m_FallMinHeight) {
				return false;
			}

			return this.m_Rigidbody.velocity.y < 0f && !this.m_Controller.IsGrounded;
		}

		private void OnControllerLanded ()
		{
			this.StopMotion (true);
			//Debug.Log("OnControllerLanded Fall "+ Controller.GetComponent<Animator>().IsInTransition(0));
		}

        public void OnControllerGrounded(bool grounded)
        {
			if (this.IsActive)
            {
                this.m_Animator.SetInteger("Int Value", 1);
               // Invoke("OnControllerLanded", 2f);
            }
        }
    }
}