using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    public class Climb : MotionState
    {
		[SerializeField]
		private LayerMask m_ClimbLayer= Physics.DefaultRaycastLayers;
		[SerializeField]
		private float m_Distance = 1.3f;
		[SerializeField]
		private float m_MinDistance = 0.9f;
		[SerializeField]
		private float m_MinHeight = 0.5f;
		[SerializeField]
		private float m_MaxHeight = 1f;

		private float jumpTime;
		public override void OnStart()
        {
			StartJump();
        }

        public override void OnStop()
        {
			this.m_Rigidbody.velocity = Vector3.zero;
        }

        public override bool CanStart()
		{
			if (Physics.Raycast(transform.position + Vector3.up * (this.m_MaxHeight+0.01f), transform.forward, this.m_Distance,this.m_ClimbLayer)) {
				return false;
			}
			RaycastHit hit;
			if (Physics.Raycast(transform.position + Vector3.up * this.m_MinHeight, transform.forward,out hit, this.m_Distance, this.m_ClimbLayer)) {
				
				return Vector3.Distance(transform.position + Vector3.up * this.m_MinHeight, hit.point) > this.m_MinDistance && this.m_Controller.RelativeInput.z > 0.7f;
			}
			return false;
		}

		private void StartJump()
		{
			if (this.IsActive)
			{
				this.jumpTime = Time.time;
				this.m_Controller.IsGrounded = false;
				this.m_Rigidbody.velocity = this.m_Controller.RootMotionForce;
			}
		}

        public override bool UpdateVelocity(ref Vector3 velocity)
        {
			velocity = this.m_Controller.RootMotionForce;
            return false;
        }

        public override bool CheckGround()
		{
			if (Time.time > jumpTime + 0.3f)
			{
				return true;
			}
			return false;
		}

		public override bool CanStop()
		{
			return this.m_Controller.IsGrounded;
		}
	}
}