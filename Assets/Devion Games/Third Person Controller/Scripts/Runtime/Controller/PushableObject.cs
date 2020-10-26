using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
	public class PushableObject : MonoBehaviour
	{
		
		public Vector3 leftHandOffset;
		public Vector3 rightHandOffset;
		private Rigidbody m_Rigidbody;
		private Transform m_Transform;

		private Vector3 m_MoveDirection;
		private bool m_CanMove = true;
		private bool m_Moving;
		private ThirdPersonController m_Controller;

		private void Start ()
		{
			this.m_Rigidbody = GetComponent<Rigidbody> ();	
			this.m_Transform = transform;
		}

		public void StartMove (ThirdPersonController controller)
		{
			this.m_CanMove = true;
			this.m_MoveDirection = controller.transform.forward;
			this.m_MoveDirection.y = 0f;
			this.m_Moving = true;
			this.m_Controller = controller;
		}

		public void StopMove ()
		{
			this.m_Moving = false;
		}

		public bool Move (Vector3 position)
		{
			this.m_Rigidbody.MovePosition (position);
			return m_CanMove;
		}

		private void OnCollisionStay (Collision collision)
		{
			if (this.m_Moving) {
				foreach (ContactPoint p in collision.contacts) {
					Vector3 direction = p.normal;
					if (p.otherCollider.transform != m_Controller.transform && p.point.y > this.m_Transform.position.y + 0.1f && Vector3.Dot (direction, this.m_MoveDirection) < -0.2) {
						
						this.m_CanMove = false;
						break;
					}
				}
			}
		}

	}
}