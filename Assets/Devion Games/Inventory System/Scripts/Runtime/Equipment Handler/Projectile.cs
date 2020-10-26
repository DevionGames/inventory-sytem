using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        private bool m_AutoDestruct = true;
        [SerializeField]
        private float m_DestructDelay = 10f;

        private Rigidbody m_Rigidbody;
        private Collider m_Collider;


        private void Start()
        {
            this.m_Rigidbody = GetComponent<Rigidbody>();
            this.m_Collider = GetComponent<Collider>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            this.m_Collider.enabled = false;
            this.m_Rigidbody.velocity = Vector3.zero;
            this.m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            transform.position = collision.GetContact(0).point;
            if (this.m_AutoDestruct)
                Destroy(gameObject, this.m_DestructDelay);
        }

    }
}