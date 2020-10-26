using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    public class MoveForward : MonoBehaviour
    {

        [SerializeField]
        private float m_Speed = 5f;
        [SerializeField]
        private bool m_AutoDestruct = true;
        [SerializeField]
        private float m_DestructDelay = 10f;
        [SerializeField]
        private GameObject hitEffect= null;


        private Rigidbody m_Rigidbody;
        private Collider m_Collider;


        private void Start()
        {
            this.m_Rigidbody = GetComponent<Rigidbody>();
            this.m_Collider = GetComponent<Collider>();
            this.transform.parent = null;
            if (this.m_AutoDestruct)
                Destroy(gameObject, this.m_DestructDelay);

            Vector3 forward = Camera.main.transform.forward;
            if (forward.sqrMagnitude != 0.0f)
            {
                forward.Normalize();
                transform.LookAt(transform.position + forward);
            }
        }

        private void FixedUpdate()
        {
            this.m_Rigidbody.velocity = transform.forward * m_Speed; 
        }

        private void OnCollisionEnter(Collision collision)
        {
            this.m_Collider.enabled = false;
            this.m_Rigidbody.velocity = Vector3.zero;
            this.m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            transform.position = collision.GetContact(0).point;
            if (hitEffect != null)
                Instantiate(hitEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}