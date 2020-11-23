using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

        private static bool m_PointerOverTrigger = false;

        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            Camera camera = Camera.main;
            if (camera.GetComponent<TriggerRaycaster>() == null)
                camera.gameObject.AddComponent<TriggerRaycaster>();
        }


        private void Start()
        {
            this.m_Transform = transform;    
        }

        private void Update()
        {

            Ray ray = (Cursor.lockState == CursorLockMode.Locked? new Ray(this.m_Transform.position,this.m_Transform.forward) : Camera.main.ScreenPointToRay(Input.mousePosition));

            RaycastHit hit;
            if (TriggerRaycaster.Raycast(ray, out hit, float.PositiveInfinity, this.m_LayerMask))
            {
                if (m_LastCameraHit != hit.collider.gameObject)
                {
                    if(this.m_LastCameraHit != null)
                        EventHandler.Execute(this.m_LastCameraHit, "OnPointerExitTrigger");

                    m_LastCameraHit = hit.collider.gameObject;
                    EventHandler.Execute(m_LastCameraHit, "OnPointerEnterTrigger");
                }
                int button = -1;
                if (Input.GetMouseButtonDown(0))
                    button = 0;
                if (Input.GetMouseButtonDown(1))
                    button = 1;
                if (Input.GetMouseButtonDown(2))
                    button = 2;

                if (button != -1)
                {
                    m_LastCameraHit = hit.collider.gameObject;
                    EventHandler.Execute<int>(m_LastCameraHit, "OnPoinerClickTrigger", button);
                }
                
                TriggerRaycaster.m_PointerOverTrigger = true;
            }
            else
            {
                if (m_LastCameraHit != null)
                {
                    EventHandler.Execute(m_LastCameraHit, "OnPointerExitTrigger");
                    m_LastCameraHit = null;
                }
                TriggerRaycaster.m_PointerOverTrigger = false;
            }




            /*if (!UnityTools.IsPointerOverUI())
            {

            /*  RaycastHit hit;
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
              }*/
        }
        

        public static bool Raycast(Vector3 origin, Vector3 direction,out RaycastHit hit, float maxDistance, int layerMask ) {
            return Raycast(new Ray(origin, direction), out hit, maxDistance, layerMask);
        }

        public static bool Raycast(Ray ray, out RaycastHit hit, float maxDistance, int layerMask)
        {
            RaycastHit[] hits = Physics.RaycastAll(ray, maxDistance, layerMask, QueryTriggerInteraction.Collide);
            hit = new RaycastHit();
            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    RaycastHit current = hits[i];
                    if (current.collider.GetComponent<BaseTrigger>() == null)
                        continue;
                    hit = current;
                    return true;
                }
                return false;
            }
            return false;
        }

        public static bool IsPointerOverTrigger()
        {
            return TriggerRaycaster.m_PointerOverTrigger;
        }

    }
}