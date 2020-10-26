﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    public abstract class Weapon : VisibleItem
    {
        [Header("Activation:")]
        [InspectorLabel("Input Name")]
        [SerializeField]
        protected string m_ActivationInputName;
        [SerializeField]
        protected ActivationType m_ActivationType;

        private bool m_IsActive;
        protected bool IsActive {
            get { return this.m_IsActive; }
            set {
                if (this.m_IsActive != value) {
                    this.m_IsActive=value;
                    OnItemActivated(this.m_IsActive);
                }
            }
        }

        [Header("Use:")]
        [InspectorLabel("Input Name")]
        [SerializeField]
        protected string m_UseInputName = "Fire1";
        [SerializeField]
        protected StartType m_StartType;
        [SerializeField]
        protected StopType m_StopType;

        [Header("Animator IK:")]
        [SerializeField]
        protected Transform m_RightHandIKTarget;
        [SerializeField]
        protected float m_RightHandIKWeight = 1f;
        [SerializeField]
        protected float m_RightHandIKSpeed = 1f;
        protected float m_RightHandIKLerp = 0f;

        [SerializeField]
        protected Transform m_LeftHandIKTarget;
        [SerializeField]
        protected float m_LeftHandIKWeight = 1f;
        [SerializeField]
        protected float m_LeftHandIKSpeed = 1f;
        protected float m_LeftHandIKLerp = 0f;



        [Header("Animator States:")]
        [SerializeField]
        public int m_ItemID = 0;
        [SerializeField]
        protected string m_IdleState="Movement";
        [SerializeField]
        protected string m_UseState="Sword Slash";

        

        protected bool m_InUse;
        protected float m_UseClipLength;

        private AnimatorStateInfo[] m_DefaultStates;


        public override void OnItemEquip(Item item)
        {
            base.OnItemEquip(item);
            if (this.m_ActivationType == ActivationType.Automatic)
            {
                IsActive = true;
            }
        }

        public override void OnItemUnEquip(Item item)
        {
            base.OnItemUnEquip(item);
           // if (this.m_ActivationType == ActivationType.Automatic)
            //{
                IsActive = false;
            //}
        }

        protected override void Update()
        {
            if (this.m_Pause || UnityTools.IsPointerOverUI() || !this.m_CharacterAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Default")) { return; }

 
            switch (this.m_ActivationType)
            {
                case ActivationType.Button:

                    IsActive = Input.GetButton(this.m_ActivationInputName);

                    break;
                case ActivationType.Toggle:
                    if (Input.GetButtonDown(this.m_ActivationInputName))
                    {
                        IsActive = !IsActive;
                    }
                    break;
            }
            if (!IsActive) { return; }


            if (this.m_StartType != StartType.Down || !Input.GetButtonDown(this.m_UseInputName))
            {
                if (this.m_StopType == StopType.Up && Input.GetButtonUp(this.m_UseInputName))
                {
                    this.TryStopUse();
                }
            }
            else if (!this.m_InUse && this.m_StartType == StartType.Down)
            {
                this.TryStartUse();
            }

            if (this.m_StartType == StartType.Press && Input.GetButton(this.m_UseInputName))
            {
                this.TryStartUse();
            }

           

            if (!IsActive  || !this.m_CharacterAnimator.GetCurrentAnimatorStateInfo(0).IsName(this.m_IdleState) || this.m_InUse)
            {
                m_RightHandIKLerp = 0f;
                m_LeftHandIKLerp = 0f;
                return;
            }
            m_RightHandIKLerp = Mathf.Lerp(m_RightHandIKLerp, 1f, Time.deltaTime*this.m_RightHandIKSpeed);
            m_LeftHandIKLerp = Mathf.Lerp(m_LeftHandIKLerp, 1f, Time.deltaTime*this.m_LeftHandIKSpeed);
        }

        private void TryStartUse() {
            if (!this.m_InUse && CanUse()) {
                StartUse();
            }
        }

        protected virtual bool CanUse() {
            Ray  ray = this.m_Camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                return hit.collider.GetComponent<Trigger>() ==null;
            }

            return true; 
        }

        private void TryStopUse() {
            if (m_InUse && CanUnuse()) {
                StopUse();
            }
        }

        protected virtual bool CanUnuse() { return true; }

        protected void StopUse() {
            if (IsActive)
            {
                OnStopUse();
                this.m_InUse = false;
                this.m_CharacterAnimator.CrossFadeInFixedTime(this.m_IdleState, 0.2f);
            }
        }

        protected virtual void OnStopUse() { }

        protected void StartUse()
        {
            OnStartUse();
            this.m_InUse = true;
            Use();
        }

        protected virtual void OnStartUse() { }

        protected virtual void Use() {
            this.m_CharacterAnimator.CrossFadeInFixedTime(this.m_UseState, 0.2f);
            this.m_CharacterAnimator.Update(0f);

        }

        private void OnEndUse() {
            if (this.m_InUse && this.m_StopType == StopType.EndUseEvent) {
                StopUse();
            }
        }

        protected bool m_Pause;
        private void PauseItemUpdate(bool state) {
            this.m_Pause = state;
        }
   
        protected virtual void OnItemActivated(bool activated) {
            if (activated)
            {
                this.m_CharacterAnimator.Update(1f);
                this.m_DefaultStates = new AnimatorStateInfo[this.m_CharacterAnimator.layerCount];
                 for (int j = 0; j < this.m_CharacterAnimator.layerCount; j++)
                 {
                     AnimatorStateInfo stateInfo = this.m_CharacterAnimator.GetCurrentAnimatorStateInfo(j);
                     this.m_DefaultStates[j] = stateInfo;
                 }
                this.m_CharacterAnimator.SetInteger("Item ID", this.m_ItemID);
                this.m_CharacterAnimator.CrossFadeInFixedTime(this.m_IdleState, 0.2f);

                this.m_InUse = false;
            } else {
                this.m_CharacterAnimator.SetInteger("Item ID",0);
                for (int j = 0; j < this.m_DefaultStates.Length; j++)
                {
                    this.m_CharacterAnimator.CrossFadeInFixedTime(this.m_DefaultStates[j].shortNameHash, 0.2f);
                    this.m_CharacterAnimator.Update(0f);
                }
            }
        }

        protected virtual void OnAnimatorIK(int layerIndex)
        {
            if (!IsActive || layerIndex == 0 || !this.m_CharacterAnimator.GetCurrentAnimatorStateInfo(0).IsName(this.m_IdleState) || this.m_InUse)
            {
                return;
            }

            if (this.m_RightHandIKTarget != null)
            {
                this.m_CharacterAnimator.SetIKPosition(AvatarIKGoal.RightHand, this.m_RightHandIKTarget.position);
                this.m_CharacterAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, this.m_RightHandIKWeight*this.m_RightHandIKLerp);

                this.m_CharacterAnimator.SetIKRotation(AvatarIKGoal.RightHand, this.m_RightHandIKTarget.rotation);
                this.m_CharacterAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, this.m_RightHandIKWeight*this.m_RightHandIKLerp);
            }

            if (this.m_LeftHandIKTarget != null)
            {
                this.m_CharacterAnimator.SetIKPosition(AvatarIKGoal.LeftHand, this.m_LeftHandIKTarget.position);
                this.m_CharacterAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, this.m_LeftHandIKWeight*this.m_LeftHandIKLerp);

                this.m_CharacterAnimator.SetIKRotation(AvatarIKGoal.LeftHand, this.m_LeftHandIKTarget.rotation);
                this.m_CharacterAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, this.m_LeftHandIKWeight*this.m_LeftHandIKLerp);
            }

        }

        public enum ActivationType {
            Automatic,
            Button,
            Toggle
        }

        public enum StartType
        {
            Automatic,
            Down,
            Press
        }

        public enum StopType
        {
            Up,
            EndUseEvent,
            Manual,
        }
    }
}