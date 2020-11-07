using System.Collections;
using System.Collections.Generic;
using DevionGames.UIWidgets;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace DevionGames.InventorySystem
{
    public class ShootableWeapon : Weapon
    {
        [SerializeField]
        private string m_ReloadState="Bow Reload";
        [Header("Behaviour:")]
        [SerializeField]
        private int m_ReloadClipSize = 1;
        [SerializeField]
        private bool m_ResetClipSize = false;

        [Header("Projectile:")]

        [SerializeField]
        protected GameObject m_Projectile;
        [SerializeField]
        protected ProjectileVisibility m_ProjectileVisibility = ProjectileVisibility.Always;
        [SerializeField]
        protected float m_ProjectileSpeed = 40f;
        [SerializeField]
        protected Transform m_FirePoint;
        [SerializeField]
        protected Transform m_ReloadPoint;
        [ItemPicker(true)]
        [SerializeField]
        protected Item m_ProjectileItem;
        [SerializeField]
        protected string m_ProjectileItemWindow = "Inventory";

        protected GameObject m_CurrentProjectile;
        private int m_CurrentClipSize = 0;
        private bool m_IsReloading;
        private ItemContainer[] m_ItemContainers;

        protected override void OnItemActivated(bool activated)
        {
            base.OnItemActivated(activated);
            if (activated)
            {
                this.m_IsReloading = false;
               
                if (this.m_CurrentClipSize > 0) {
                    CreateCurrentProjectile();
                }
            }
            else {
                if (this.m_ResetClipSize)
                {
                    if (this.m_CurrentClipSize > 0)
                    {
                        Item item = Instantiate(m_ProjectileItem);
                        item.Stack = this.m_CurrentClipSize;
                        ItemContainer.AddItem(this.m_ProjectileItemWindow, item);
                    }
                    this.m_CurrentClipSize = 0;
                }
                if (this.m_CurrentProjectile != null) {
                    Destroy(this.m_CurrentProjectile);
                }
            }
        }

        protected override void Update()
        {
            base.Update();
            if(IsActive)
                TryReload();
        }


        protected override void Use()
        {
            if (this.m_CurrentClipSize == 0)
            {
                TryReload();
            }else {
                base.Use();
                if (this.m_ProjectileVisibility == ProjectileVisibility.OnFire || this.m_CurrentProjectile == null)
                {
                    CreateCurrentProjectile();
                }

                this.m_CurrentProjectile.transform.position = this.m_FirePoint.transform.position;
                this.m_CurrentProjectile.transform.parent = null;
                Rigidbody projectileRigidbody = this.m_CurrentProjectile.GetComponent<Rigidbody>();
                this.m_CurrentProjectile.GetComponent<Projectile>().enabled = true;
                projectileRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                RaycastHit hit;
                if (Physics.Raycast(this.m_CameraTransform.position, this.m_CameraTransform.forward, out hit, float.PositiveInfinity))
                {
                    if(Vector3.Distance(m_CurrentProjectile.transform.position,hit.point)>1f)
                        projectileRigidbody.transform.LookAt(hit.point);

                }
                else
                {
                    projectileRigidbody.transform.forward = this.m_CameraTransform.forward;
                }

                projectileRigidbody.velocity = projectileRigidbody.transform.forward * this.m_ProjectileSpeed;
              
                this.m_CurrentProjectile = null;
            }
        }


        protected override bool CanUse()
        {
            return this.m_CurrentClipSize > 0;
        }

        private bool TryReload() {
            if (!this.m_IsReloading && this.m_CurrentClipSize == 0 ) {
                if (ItemContainer.HasItem(this.m_ProjectileItemWindow, this.m_ProjectileItem, this.m_ReloadClipSize))
                {
                    ItemContainer.RemoveItem(this.m_ProjectileItemWindow, m_ProjectileItem, this.m_ReloadClipSize);
                    this.m_IsReloading = true;
                    this.m_CharacterAnimator.CrossFadeInFixedTime(this.m_ReloadState, 0.2f);
                    if (this.m_ProjectileVisibility == ProjectileVisibility.Always)
                    {
                        CreateCurrentProjectile();
                    }
                    return true;
                }
                if(Input.GetButtonDown(this.m_ActivationInputName))
                    InventoryManager.Notifications.missingItem.Show(this.m_ProjectileItem.Name);
            }
            return false;
        }

        protected override void OnStopUse()
        {
            this.m_CurrentClipSize -= 1;     
        }

        private void OnEndReload()
        {
            if (this.m_IsReloading)
            {
                this.m_IsReloading = false;
                this.m_CurrentClipSize = this.m_ReloadClipSize;
                this.m_CharacterAnimator.CrossFadeInFixedTime(this.m_IdleState, 0.2f);
                this.m_CurrentProjectile.transform.SetParent(this.m_FirePoint, false);
            }
        }

        protected virtual void CreateCurrentProjectile()
        {
            this.m_CurrentProjectile = Instantiate(this.m_Projectile);
            IgnoreCollision(this.m_CurrentProjectile);
            this.m_CurrentProjectile.transform.SetParent(this.m_ReloadPoint, false);
            this.m_CurrentProjectile.SetActive(true);
        }

        public enum ProjectileVisibility
        {
            OnFire,
            Always
        }
    }
}