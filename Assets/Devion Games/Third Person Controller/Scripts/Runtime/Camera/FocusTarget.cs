using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    public class FocusTarget : MonoBehaviour
    {
        [SerializeField]
        private Vector3 m_OffsetPosition = new Vector3(0f, 1f, 0f);
        [SerializeField]
        private float m_Pitch = 22f;
        [SerializeField]
        private float m_Distance = 2f;
        [SerializeField]
        private float m_Speed = 5f;

        [SerializeField]
        private bool m_SpinTarget = true;
        [SerializeField]
        private string m_SpinButton = "Fire3";

        private bool m_Focus;
        private ThirdPersonCamera m_ThirdPersonCamera;
        private bool m_TargetRotationFinished = false;
        private bool m_GUIClick;

        public delegate void FucusDelegate(bool state);
        /// <summary>
        /// Called when an item is added to the container.
        /// </summary>
        public event FucusDelegate OnFocusChange;

        public bool Focused {
            get { return this.m_Focus; }
            set {
                Focus(value);
            }
        }

        private void Start()
        {
            this.m_ThirdPersonCamera = GetComponent<ThirdPersonCamera>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (this.m_Focus) {
                Transform target = this.m_ThirdPersonCamera.Target;
                Vector3 targetPosition = target.position + this.m_OffsetPosition.x * target.right + this.m_OffsetPosition.y* target.up;
                Vector3 direction = -m_Distance * transform.forward;
                Vector3 desiredPosition = targetPosition + direction;

                transform.position = Vector3.Lerp(transform.position, desiredPosition, this.m_Speed*Time.deltaTime );
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(this.m_Pitch,transform.rotation.eulerAngles.y,transform.rotation.eulerAngles.z), this.m_Speed * Time.deltaTime);

                if (!this.m_TargetRotationFinished)
                {
                    Vector3 cameraDirection = transform.position - this.m_ThirdPersonCamera.Target.position;
                    cameraDirection.y = 0f;
                    Quaternion targetRotation = Quaternion.LookRotation(cameraDirection, Vector3.up);
                    this.m_ThirdPersonCamera.Target.rotation = Quaternion.Lerp(target.rotation, targetRotation, Time.deltaTime * this.m_Speed);
                    if (Quaternion.Angle(target.rotation, targetRotation) < 0.1f)
                    {
                        this.m_TargetRotationFinished = true;
                    }
                }else if (this.m_SpinTarget) {

                    if (Input.GetMouseButtonDown(0) && UnityTools.IsPointerOverUI()) {
                        this.m_GUIClick = true;
                    }
                    if (Input.GetMouseButtonUp(0)) {
                        this.m_GUIClick = false;
                    }

                    if (Input.GetButton(this.m_SpinButton) && !this.m_GUIClick)
                    {
                        float input = Input.GetAxis("Mouse X") * -this.m_Speed; 
                        target.Rotate(0, input, 0, Space.World);
                    }
                }
            }
        }

        private void Focus(bool focus)
        {
            if (this.m_Focus == focus)
                return;

            this.m_Focus = focus;
            this.m_TargetRotationFinished = false;
            this.m_GUIClick = false;
            if (this.m_Focus) {
                this.m_ThirdPersonCamera.Target.SendMessage("Deselect", SendMessageOptions.DontRequireReceiver);
            }
            OnFocusChange?.Invoke(focus);
        }
    }
}