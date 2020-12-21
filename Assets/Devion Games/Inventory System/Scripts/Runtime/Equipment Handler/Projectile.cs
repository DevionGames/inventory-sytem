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

        [SerializeField]
        private Object m_Data=null;

        private Rigidbody m_Rigidbody;
        private Collider m_Collider;


        private void Start()
        {
            this.m_Rigidbody = GetComponent<Rigidbody>();
            this.m_Collider = GetComponent<Collider>();
        }


        //Changed to OnCollisionStay for close shooting.
       /* private void OnCollisionStay(Collision collision)
        {
            if (!isActiveAndEnabled) return;
                OnHit(collision.transform, collision.GetContact(0).point);
        }*/

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
            if (this.m_AutoDestruct)
                Destroy(gameObject, this.m_DestructDelay);
        }

    }
}