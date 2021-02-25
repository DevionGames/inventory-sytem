using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    public class Climb : MotionState
	{
		[SerializeField]
		private LayerMask m_ClimbMask = Physics.DefaultRaycastLayers;
		[SerializeField]
		private float m_MinForwardInput = 0f;
		[SerializeField]
		private float m_MinHeight = 0.8f;
		[SerializeField]
		private float m_MaxHeight = 1.2f;
		[SerializeField]
		private float m_MaxDistance = 0.5f;
		[SerializeField]
		private Vector3 m_ExtraForce= Vector3.zero;
		[SerializeField]
		private Vector3 m_IKLeftHandOffset=new Vector3(0.266f,0.01f,0.05f);
		[SerializeField]
		private Vector3 m_IKRightHandOffset = new Vector3(0.266f, 0.01f, 0.05f);
		[SerializeField]
		private float m_IKWeightSpeed = 8f;
		[SerializeField]
		private bool m_UpdateLookAt = false;


		private float m_FinalHeight;
		private float m_IKWeight = 0f;
		private float m_IKCurrentWeight = 0f;

		private Vector3 m_IKLeftHand;
		private Quaternion m_IKLeftHandRotation;

		private Vector3 m_IKRightHand;
		private Quaternion m_IKRightHandRotation;

		public override void OnStart()
        {
			this.m_Controller.IsGrounded = false;
			
			this.m_Controller.Velocity = Vector3.zero;
			this.m_Rigidbody.velocity = Vector3.zero;
			this.m_Rigidbody.useGravity = false;
		
			this.m_CapsuleCollider.isTrigger = true;
			Ray ray = new Ray(transform.position + transform.forward * this.m_MaxDistance + Vector3.up * this.m_MaxHeight, Vector3.down);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, this.m_MaxHeight - this.m_MinHeight, this.m_ClimbMask))
			{
				Vector3 position = transform.position - transform.forward * 0.01f; ;
				position.y = hit.point.y;
				Ray edgeRay = new Ray(position, transform.forward);
				RaycastHit edgeHit;
				if (Physics.Raycast(edgeRay, out edgeHit, this.m_MaxDistance, this.m_ClimbMask))
				{
					Ray rightHeightRay = new Ray(transform.position + transform.forward * edgeHit.distance + Vector3.up * this.m_MaxHeight + transform.right.normalized * this.m_IKLeftHandOffset.x + transform.forward * this.m_IKLeftHandOffset.z, Vector3.down);
					RaycastHit rightHeightHit;
					Ray leftHeightRay = new Ray(transform.position + transform.forward * edgeHit.distance + Vector3.up * this.m_MaxHeight - transform.right.normalized * this.m_IKRightHandOffset.x + transform.forward * this.m_IKLeftHandOffset.z, Vector3.down);
					RaycastHit leftHeightHit;
					if (Physics.Raycast(rightHeightRay, out rightHeightHit, this.m_MaxHeight - this.m_MinHeight, this.m_ClimbMask) && Physics.Raycast(leftHeightRay, out leftHeightHit, this.m_MaxHeight - this.m_MinHeight, this.m_ClimbMask))
					{
						//Climb
						this.m_IKLeftHand = leftHeightHit.point+transform.up.normalized*this.m_IKLeftHandOffset.y;
						this.m_IKRightHand = rightHeightHit.point+transform.up.normalized * this.m_IKRightHandOffset.y;


						this.m_IKLeftHandRotation = Quaternion.LookRotation(Vector3.forward, leftHeightHit.normal)*transform.rotation;
						this.m_IKRightHandRotation = Quaternion.LookRotation(Vector3.forward, rightHeightHit.normal)*transform.rotation;

						this.m_FinalHeight = leftHeightHit.point.y;
					}

				}
			}
		}

        public override void OnStop()
        {
			this.m_CapsuleCollider.isTrigger = false;
			this.m_Rigidbody.useGravity = true;
		}

        public override bool CanStart()
		{
			if (this.m_Controller.RelativeInput.z < this.m_MinForwardInput) return false;

			Ray ray = new Ray(transform.position+transform.forward * this.m_MaxDistance + Vector3.up * this.m_MaxHeight, Vector3.down);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, this.m_MaxHeight - this.m_MinHeight, this.m_ClimbMask))
			{
				Vector3 position = transform.position-transform.forward*0.01f;
				position.y = hit.point.y;
				Ray edgeRay = new Ray(position, transform.forward);
				RaycastHit edgeHit;
				if (Physics.Raycast(edgeRay, out edgeHit, this.m_MaxDistance, this.m_ClimbMask))
				{
					Ray rightHeightRay = new Ray(transform.position+transform.forward * edgeHit.distance + Vector3.up * this.m_MaxHeight + transform.right.normalized * this.m_IKLeftHandOffset.x + transform.forward * this.m_IKLeftHandOffset.z, Vector3.down);
					RaycastHit rightHeightHit;
					Ray leftHeightRay = new Ray(transform.position+transform.forward * edgeHit.distance + Vector3.up * this.m_MaxHeight - transform.right.normalized * this.m_IKRightHandOffset.x + transform.forward * this.m_IKLeftHandOffset.z, Vector3.down);
					RaycastHit leftHeightHit;
					if (Physics.Raycast(rightHeightRay, out rightHeightHit, this.m_MaxHeight - this.m_MinHeight, this.m_ClimbMask) && Physics.Raycast(leftHeightRay, out leftHeightHit, this.m_MaxHeight - this.m_MinHeight, this.m_ClimbMask))
					{
						//Climb
						/*this.m_IKLeftHand = leftHeightHit.point;
						this.m_IKRightHand = rightHeightHit.point;

						Vector3 lookAt = Vector3.Cross(-leftHeightHit.normal, transform.right);
						lookAt = lookAt.y < 0 ? -lookAt : lookAt;
						this.m_IKLeftHandRotation = Quaternion.LookRotation(leftHeightHit.point + lookAt, leftHeightHit.normal);
						this.m_IKRightHandRotation = Quaternion.LookRotation(leftHeightHit.point + lookAt, leftHeightHit.normal);

						this.m_FinalPosition = leftHeightHit.point;*/
						return true;
					}

				}
			}
			return false;
		}

        public override bool UpdateAnimatorIK(int layer)
        {

			this.m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, this.m_IKCurrentWeight);
			this.m_Animator.SetIKPosition(AvatarIKGoal.LeftHand, this.m_IKLeftHand);

			this.m_Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, this.m_IKCurrentWeight);
			this.m_Animator.SetIKRotation(AvatarIKGoal.LeftHand, this.m_IKLeftHandRotation);

			this.m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, this.m_IKCurrentWeight);
			this.m_Animator.SetIKPosition(AvatarIKGoal.RightHand, this.m_IKRightHand);

			this.m_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, this.m_IKCurrentWeight);
			this.m_Animator.SetIKRotation(AvatarIKGoal.RightHand, this.m_IKRightHandRotation);
			return this.m_UpdateLookAt;
        }

        private void OnDrawGizmosSelected()
        {
			if (!this.enabled) return;
			Ray ray = new Ray(transform.position+transform.forward * this.m_MaxDistance + Vector3.up * this.m_MaxHeight, Vector3.down);
			RaycastHit hit;

			if (DebugRay(ray, out hit, this.m_MaxHeight - this.m_MinHeight))
			{
				
				Vector3 position = transform.position;
				position.y = hit.point.y;
				Ray edgeRay = new Ray(position, transform.forward);
				RaycastHit edgeHit;
				if (Physics.Raycast(edgeRay, out edgeHit, this.m_MaxDistance))
				{
					Debug.DrawLine(edgeRay.origin, edgeHit.point, Color.green);


					Ray rightHeightRay = new Ray(transform.position + transform.forward * edgeHit.distance + Vector3.up * this.m_MaxHeight + transform.right.normalized * this.m_IKRightHandOffset.x + transform.forward * this.m_IKLeftHandOffset.z, Vector3.down);
					RaycastHit rightHeightHit;
					if (Physics.Raycast(rightHeightRay, out rightHeightHit, this.m_MaxHeight - this.m_MinHeight))
					{
						Debug.DrawLine(rightHeightRay.origin, rightHeightHit.point, Color.green);
					}
					else
					{
						Debug.DrawLine(rightHeightRay.origin, rightHeightRay.origin + Vector3.down * (this.m_MaxHeight - this.m_MinHeight), Color.red);
					}


					Ray leftHeightRay = new Ray(transform.position + transform.forward * edgeHit.distance + Vector3.up * this.m_MaxHeight - transform.right.normalized * this.m_IKLeftHandOffset.x + transform.forward * this.m_IKLeftHandOffset.z, Vector3.down);
					RaycastHit leftHeightHit;
					if (Physics.Raycast(leftHeightRay, out leftHeightHit, this.m_MaxHeight - this.m_MinHeight))
					{
						Debug.DrawLine(leftHeightRay.origin, leftHeightHit.point, Color.green);
					}
					else
					{
						Debug.DrawLine(leftHeightRay.origin, leftHeightRay.origin + Vector3.down * (this.m_MaxHeight - this.m_MinHeight), Color.red);
					}

				}
				else {
					Debug.DrawLine(ray.origin, ray.origin + Vector3.down * (this.m_MaxHeight - this.m_MinHeight), Color.red);
				}
			}

			
		}

		private bool DebugRay(Ray ray, out RaycastHit hit, float distance) {
			bool result = Physics.Raycast(ray, out hit, distance);
			Debug.DrawLine(ray.origin, ray.origin+ ray.direction * distance, result ? Color.green : Color.red);
			return result;
		}

        public override bool UpdateVelocity(ref Vector3 velocity)
        {
			if (!IsPlaying() ) {
				return false;
			}
			Vector3 rootMotion = this.m_Controller.RootMotionForce;
			rootMotion += transform.TransformDirection(this.m_ExtraForce);
			float force = this.m_Animator.GetFloat("Force");
			rootMotion += transform.forward * force;

			if (Mathf.Abs(this.transform.position.y - this.m_FinalHeight) < 0.05f)
			{
				rootMotion.y = 0f;
				Vector3 position = transform.position;
				position.y = Mathf.Lerp(position.y, this.m_FinalHeight, Time.fixedDeltaTime * 15f);

				transform.position = position;
			}

			this.m_IKCurrentWeight = Mathf.Lerp(this.m_IKCurrentWeight, this.m_IKWeight, Time.fixedDeltaTime * this.m_IKWeightSpeed);
			velocity = rootMotion;
			return false;
        }

        private void SetIKWeight(float weight)
		{
			this.m_IKWeight = weight;
		}

        public override bool UpdateRotation()
        {
            return false;
        }

        public override bool CheckGround()
		{
			return false;
		}

        public override bool CheckStep()
        {
            return false;
        }
    }
}