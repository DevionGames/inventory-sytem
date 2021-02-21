using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DevionGames.InventorySystem
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        private bool m_AutoDestruct = true;
        [Compound("m_AutoDestruct")]
        [SerializeField]
        private float m_DestructDelay = 15f;
        [SerializeField]
        private StartDirection m_StartDirection = StartDirection.Camera;
        [SerializeField]
        private Vector3 m_StartPositionOffset= Vector3.zero;
        [SerializeField]
        private float m_Speed = 5f;
        [SerializeField]
        private bool m_FollowTarget = true;
        [Compound("m_FollowTarget")]
        [SerializeField]
        private bool m_SelectBestTarget = true;
        [Compound("m_FollowTarget")]
        [SerializeField]
        private float m_MaxDistance = 20f;
        [Compound("m_FollowTarget")]
        [SerializeField]
        private float m_FieldOfView = 90f;
        [Compound("m_FollowTarget")]
        [SerializeField]
        private float m_TurnSpeed = -1f;
        [SerializeField]
        private bool m_DestroyOnCollision = true;
        [Compound("m_DestroyOnCollision")]
        [SerializeField]
        private float m_DestroyDelay = 0.1f;

        [SerializeField]
        private Object m_Data=null;

        private Rigidbody m_Rigidbody;
        private Collider m_Collider;
        private GameObject m_Target;

        private void Start()
        {
            this.transform.parent = null;
            this.m_Rigidbody = GetComponent<Rigidbody>();
            this.m_Collider = GetComponent<Collider>();
            if (this.m_AutoDestruct)
                Destroy(gameObject, this.m_DestructDelay);

            transform.position = transform.TransformPoint(this.m_StartPositionOffset);
            PlayerInfo player = InventoryManager.current.PlayerInfo;

            switch (this.m_StartDirection) {
                case StartDirection.Camera:
                    SetStartDirection(Camera.main.transform.forward);
                    break;
                case StartDirection.Player:
                    if (player.transform != null)
                    {
                        SetStartDirection(player.transform.forward);
                    }
                    break;
            }
            if (this.m_FollowTarget)
            {
                this.m_Target = GetTarget();
                this.m_FollowTarget = CheckFieldOfView(player.gameObject != null ? player.gameObject : Camera.main.gameObject, this.m_Target, this.m_FieldOfView);
            }
        }




        private void FixedUpdate()
        {
            this.m_Rigidbody.velocity = transform.forward * m_Speed;

            if (!this.m_FollowTarget) return;

            if (this.m_Target != null)
            {
                Vector3 targetPosition = UnityTools.GetBounds(this.m_Target.gameObject).center;
                if (this.m_TurnSpeed < 0f)
                {
                    transform.LookAt(targetPosition);

                }
                else
                {
                    Vector3 directionToTarget = targetPosition - transform.position;
                    Vector3 currentDirection = transform.forward;
                    Vector3 resultingDirection = Vector3.RotateTowards(currentDirection, directionToTarget, this.m_TurnSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime, 1f);
                    transform.rotation = Quaternion.LookRotation(resultingDirection);
                }
            }
        }

        private GameObject GetTarget() {
            if (SelectableObject.current != null)
            {
                if(Vector3.Distance(SelectableObject.current.position,transform.position) < this.m_MaxDistance)
                    return SelectableObject.current.gameObject;
            }

            if (this.m_SelectBestTarget) {
                Collider[] colliders = Physics.OverlapSphere(transform.position, this.m_MaxDistance);
                Collider[] selectables = colliders.Where(x => x.GetComponent<SelectableObject>() != null).ToArray();

                GameObject from = InventoryManager.current.PlayerInfo.gameObject;
                if (from == null) from =Camera.main.gameObject;
                float minDist = float.PositiveInfinity;
                GameObject bestTarget = null;

                for (int i = 0; i < selectables.Length; i++) {
                    Vector3 directionToTarget = selectables[i].transform.position - from.transform.position;
                    // Get angle between forward and look direction
                    float angle = Vector3.Angle(from.transform.forward, directionToTarget);
                    float dist = Vector3.Distance(from.transform.position, selectables[i].transform.position) * angle;
                    if (dist < minDist)
                    {
                        minDist = dist;
                        bestTarget = selectables[i].gameObject;
                    }
                }
                return bestTarget;
            }

            return null;
        }

        private void SetStartDirection(Vector3 direction) {
            if (direction.sqrMagnitude != 0.0f)
            {
                direction.Normalize();
                transform.LookAt(transform.position + direction);
            }
        }

        protected virtual bool CheckFieldOfView(GameObject from, GameObject target, float fieldOfView)
        {
            if (target == null) return false;

            // Get direction to target
            Vector3 directionToTarget = target.transform.position - from.transform.position;
            // Get angle between forward and look direction
            float angle = Vector3.Angle(from.transform.forward, directionToTarget);
            // Is target within field of view?
            if (angle <= fieldOfView * 0.5f)
                return true;
            // Not within view
            return false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!isActiveAndEnabled) return;
                OnHit(collision.transform,collision.GetContact(0).point);
        }

        private void OnHit(Transform hit, Vector3 position) {
            this.m_Collider.enabled = false;
            this.m_Rigidbody.velocity = Vector3.zero;
            this.m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            transform.position = position;
            transform.parent = hit;
            EventHandler.Execute(InventoryManager.current.PlayerInfo.gameObject, "SendDamage", hit.gameObject, this.m_Data); 
            if(this.m_DestroyOnCollision)
                Destroy(gameObject, this.m_DestroyDelay);
        }

        public enum StartDirection { 
            None,
            Camera,
            Player
        }

        public enum LookType { 
            TowardsTarget,
            TowardsTargetSmooth
        }
    }
}