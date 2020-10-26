using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
	public class Push : MotionState
	{
		[SerializeField]
		private float m_Distance = 0.5f;
	
		private PushableObject m_PushableObject;
		private RaycastHit hitInfo;

		public override bool CanStart ()
		{
			return CheckPush (this.m_Distance);
		}

		public override bool CanStop ()
		{
			return !CheckPush (this.m_Distance + 0.3f) && this.m_InPosition;
		}

		public override void OnStart ()
		{
			Vector3 targetPosition = hitInfo.point + hitInfo.normal * this.m_Distance;
			targetPosition.y = this.m_Transform.position.y;
			this.MoveToTarget (this.m_Transform, targetPosition, Quaternion.LookRotation (-hitInfo.normal), 0.5f, delegate() {
				this.m_PushableObject.StartMove (this.m_Controller);
			});
		}

		public override void OnStop ()
		{
			this.m_PushableObject.StopMove ();
		}

		public override bool UpdateVelocity (ref Vector3 velocity)
		{
			if (!base.m_InPosition) {
				velocity = Vector3.zero;
				return false;
			}

			Vector3 movePosition = velocity * Time.deltaTime / 1.15f;
			if (!this.m_PushableObject.Move (this.m_PushableObject.transform.position + movePosition)) {
				velocity = Vector3.zero;
				return false;
			}
			return true;
		}

		public override bool UpdateAnimatorIK (int layer)
		{
			if (this.m_PushableObject != null) {
				Vector3 leftHandPosition = this.m_Animator.GetIKPosition (AvatarIKGoal.LeftHand) + this.m_PushableObject.leftHandOffset;
				this.m_Animator.SetIKPosition (AvatarIKGoal.LeftHand, leftHandPosition);
				this.m_Animator.SetIKPositionWeight (AvatarIKGoal.LeftHand, 1);

				Vector3 rightHandPosition = this.m_Animator.GetIKPosition (AvatarIKGoal.RightHand) + this.m_PushableObject.rightHandOffset;
				this.m_Animator.SetIKPosition (AvatarIKGoal.RightHand, rightHandPosition);
				this.m_Animator.SetIKPositionWeight (AvatarIKGoal.RightHand, 1);
			}
			return false;
		}

		public override bool UpdateRotation ()
		{
			return false;
		}

		private bool CheckPush (float distance)
		{
			Vector3 direction = this.m_Controller.LookRotation * (this.m_Controller.IsAiming ? this.m_Controller.RelativeInput : this.m_Controller.RawInput);
			Ray ray = new Ray (transform.position + new Vector3 (0f, this.m_CapsuleCollider.height * 0.5f, 0f), direction.normalized);
			if (Physics.Raycast (ray, out hitInfo, distance)) {

				this.m_PushableObject = hitInfo.transform.GetComponent<PushableObject> ();
				if (this.m_PushableObject != null) {
					return true;
				}
			}
			return false;
		}
	}
}