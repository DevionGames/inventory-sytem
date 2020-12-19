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
                GameObject current = hit.collider.GetComponentInParent<BaseTrigger>().gameObject;

                if (m_LastCameraHit != current)
                {
                    if(this.m_LastCameraHit != null)
                        EventHandler.Execute(this.m_LastCameraHit, "OnPointerExitTrigger");

                    m_LastCameraHit = current;
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
                    m_LastCameraHit = current;
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
                    if (current.collider.GetComponentInParent<BaseTrigger>() == null)
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