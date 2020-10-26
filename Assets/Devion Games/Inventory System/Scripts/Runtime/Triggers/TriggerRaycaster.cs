using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    /// <summary>
    /// This component raycast from camera and sends executes OnCameraRaycast in DisplayName component. 
    /// The DisplayName component will then check if CameraRaycast is included in Display Type and execute DoDisplayName.
    /// </summary>
    public class TriggerRaycaster : MonoBehaviour
    {
        [SerializeField]
        private LayerMask m_LayerMask = Physics.DefaultRaycastLayers;

        private Transform m_Transform;
        private GameObject m_LastCameraHit;
        private GameObject m_LastMouseHit;

        private void Start()
        {
            this.m_Transform = transform;    
        }

        private void Update()
        {
            if (!UnityTools.IsPointerOverUI())
            {
                RaycastHit hit;
                if (Physics.Raycast(this.m_Transform.position, this.m_Transform.forward, out hit, float.PositiveInfinity, this.m_LayerMask, QueryTriggerInteraction.Ignore))
                {
                    if (m_LastCameraHit != null && m_LastCameraHit != hit.collider.gameObject)
                    {
                        EventHandler.Execute<bool>(this.m_LastCameraHit, "OnCameraRaycast", false);
                    }
                    m_LastCameraHit = hit.collider.gameObject;
                    EventHandler.Execute<bool>(m_LastCameraHit, "OnCameraRaycast", true);
                }
                else
                {
                    if (m_LastCameraHit != null)
                    {
                        EventHandler.Execute<bool>(m_LastCameraHit, "OnCameraRaycast", false);
                        m_LastCameraHit = null;
                    }
                }

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.PositiveInfinity, this.m_LayerMask, QueryTriggerInteraction.Ignore))
                {
                    if (m_LastMouseHit != null && m_LastMouseHit != hit.collider.gameObject)
                    {
                        EventHandler.Execute<bool>(this.m_LastMouseHit, "OnMouseRaycast", false);
                    }
                    m_LastMouseHit = hit.collider.gameObject;
                    EventHandler.Execute<bool>(m_LastMouseHit, "OnMouseRaycast", true);
                }
                else
                {
                    if (m_LastMouseHit != null)
                    {
                        EventHandler.Execute<bool>(m_LastMouseHit, "OnMouseRaycast", false);
                        m_LastMouseHit = null;
                    }
                }
            }
        }
    }
}