using DevionGames.UIWidgets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DevionGames.InventorySystem
{
    public class DisplayName : MonoBehaviour, ITriggerCameInRange, ITriggerUsedHandler, ITriggerUnUsedHandler, ITriggerWentOutOfRange, ITriggerPointerEnter, ITriggerPointerExit
    {
        //When to display name?
        [SerializeField]
        [EnumFlags]
        protected DisplayNameType m_DisplayType = DisplayNameType.InRange | DisplayNameType.OnMouseOver;
        //Color to display name
        [SerializeField]
        protected Color m_Color = Color.white;
        //Name label offset
        [SerializeField]
        protected Vector3 m_Offset = Vector3.zero;

        protected BaseTrigger m_Trigger;


        protected virtual void DoDisplayName(bool state)
        {
            if (state)
            {
                FloatingTextManager.Add(gameObject, gameObject.name.Replace("(Clone)", ""), this.m_Color, this.m_Offset);
            }
            else
            {
                FloatingTextManager.Remove(gameObject);
            }
        }

        private void Start()
        {
            m_Trigger = GetComponent<BaseTrigger>();
            EventHandler.Register<bool>(gameObject, "OnCameraRaycast", OnCameraRaycast);
            EventHandler.Register<bool>(gameObject, "OnMouseRaycast", OnMouseRaycast);

            if (this.m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.Always))
                DoDisplayName(true);
        }

        private void OnDestroy()
        {
            DoDisplayName(false);
        }

        public void OnCameraRaycast(bool state)
        {
            if (
                      this.m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.CameraRaycast) &&
                      !(this.m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.InRange) && this.m_Trigger.InRange ||
                      this.m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.Always) ||
                      this.m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.InUse) && this.m_Trigger.InUse))
            {
                DoDisplayName(state);
            }
        }

        public void OnMouseRaycast(bool state) {
            if (
                         this.m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.MouseRaycast) &&
                         !(this.m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.InRange) && this.m_Trigger.InRange ||
                         this.m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.Always) ||
                         this.m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.InUse) && this.m_Trigger.InUse))
            {
                DoDisplayName(state);
            }
        }

        public void OnCameInRange(GameObject player)
        {
            if (this.m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.InRange))
                DoDisplayName(true);
        }

        public void OnTriggerUsed(GameObject player)
        {
            if (this.m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.InUse))
                DoDisplayName(true);
        }

        public void OnTriggerUnUsed(GameObject player)
        {
            if (this.m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.InUse) &&
               !(this.m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.Always)))
            {
                DoDisplayName(false);
            }
        }

        public void OnWentOutOfRange(GameObject player)
        {
            if (this.m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.InRange) &&
                 !(this.m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.Always)))
            {
                DoDisplayName(false);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!UnityTools.IsPointerOverUI())
            {
                if (this.m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.OnMouseOver))
                {        
                    DoDisplayName(true);
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!UnityTools.IsPointerOverUI())
            {
                if (
                    this.m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.OnMouseOver) &&
                    !(this.m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.InRange) && this.m_Trigger.InRange ||
                    this.m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.Always) ||
                    this.m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.InUse) && this.m_Trigger.InUse))
                {
                    DoDisplayName(false);
                }
            }
        }

        [System.Flags]
        public enum DisplayNameType
        {
            Always = 1,
            InRange = 2,
            InUse = 4,
            OnMouseOver = 8,
            CameraRaycast = 16,
            MouseRaycast = 32
        }
    }
}