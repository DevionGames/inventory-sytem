using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

namespace DevionGames
{
	public class ThirdPersonCamera : MonoBehaviour
	{
		[SerializeField]
		private bool m_DontDestroyOnLoad = true;
		[SerializeField]
		private Transform m_Target = null;
        public Transform Target {
            get {
                GameObject target = GameObject.FindGameObjectWithTag("Player");
                if (target != null)
                {
                    m_Target = target.transform;
                }
                return this.m_Target;
            }
        }


		[SerializeField]
		private CameraSettings[] m_Presets = null;

		public CameraSettings[] Presets {
			get {
				return this.m_Presets;
			}
		}

		private Transform m_Transform;
		private CameraSettings m_ActivePreset;

		private float m_MouseX;
		private float m_MouseY;
		private float m_SmoothX;
		private float m_SmoothY;
		private float m_SmoothZoom;
		private float m_SmoothXVelocity;
		private float m_SmoothYVelocity;
		private float m_ZoomVelocity;
		private Vector3 m_SmoothMoveVelocity;
		private bool m_GUIClick;
		private bool m_ConsumeTurn;
		private bool m_ConsumeZoom;

		private Canvas m_CrosshairCanvas;
		private Image m_CrosshairImage;
        private bool m_CrosshairActive;
        private bool m_RotatedLastFrame;
        private bool m_CharacterControllerActive=true;
        private FocusTarget m_FocusTarget;
        private bool m_Focus;

        private void Awake()
        {
            this.m_FocusTarget = GetComponent<FocusTarget>();
            if (this.m_FocusTarget != null) 
                this.m_FocusTarget.OnFocusChange += OnFucusTarget;
        }

        private void Start ()
		{
			this.m_Transform = transform;
			if (this.m_Transform.parent != null) {
				this.m_Transform.parent = null;
			}

			if (this.m_DontDestroyOnLoad) {
				DontDestroyOnLoad (gameObject);
			}

			for (int i = 0; i < this.m_Presets.Length; i++) {
				if (this.m_Presets [i].Activation == CameraSettings.ActivationType.Automatic) {
					this.m_ActivePreset = this.m_Presets [i];
					this.m_ActivePreset.IsActive = true;
					break;
				}
			}

			ApplyCrosshair (this.m_ActivePreset.Crosshair);
            /*this.m_MouseY = this.Target.eulerAngles.x;
			this.m_MouseX = this.Target.eulerAngles.y;*/
            this.m_MouseY = this.m_Transform.eulerAngles.x;
            this.m_MouseX = this.m_Transform.eulerAngles.y;

            Vector3 targetPosition = this.Target.position + this.m_ActivePreset.Offset.x * this.m_Transform.right + this.m_ActivePreset.Offset.y * this.Target.up;
			this.m_SmoothZoom = this.m_ActivePreset.Distance + this.m_ActivePreset.Zoom;
			Vector3 direction = -this.m_SmoothZoom * this.m_Transform.forward;
			Vector3 desiredPosition = targetPosition + direction;
			this.m_Transform.position = desiredPosition;

			Cursor.lockState = this.m_ActivePreset.CursorMode;
			Cursor.visible = this.m_ActivePreset.CursorMode == CursorLockMode.Locked?false:true;
            EventHandler.Register<bool>(Target.gameObject, "OnSetControllerActive", OnSetControllerActive);
            
		}

        private void OnEnable()
        {
            //SendMessage("Focus", false, SendMessageOptions.DontRequireReceiver);
            if (this.m_CrosshairImage != null)
            {
                this.m_CrosshairImage.gameObject.SetActive(this.m_CrosshairActive);
            }
                
        }

        private void OnDisable()
        {
           // SendMessage("Focus", true, SendMessageOptions.DontRequireReceiver);
            if (this.m_CrosshairImage != null) {
                this.m_CrosshairActive = this.m_CrosshairImage.gameObject.activeSelf;
                this.m_CrosshairImage.gameObject.SetActive(false);
            }
        }

        private void OnFucusTarget(bool state) {
            if (state)
            {
                if (this.m_CrosshairImage != null)
                {
                    this.m_CrosshairActive = this.m_CrosshairImage.gameObject.activeSelf;
                    this.m_CrosshairImage.gameObject.SetActive(false);
                }
            }else {
                if (this.m_CrosshairImage != null)
                {
                    this.m_CrosshairImage.gameObject.SetActive(this.m_CrosshairActive);
                }
            }
            this.m_Focus = state;
        }

        private void OnSetControllerActive(bool active) {
            this.m_CharacterControllerActive = active;
        }


        private void LateUpdate ()
		{
            if (this.m_Focus)
                return;

            UpdateInput();
            if(!this.m_CharacterControllerActive)
                UpdateTransform();
        }

		public void FixedUpdate ()
		{
            if (this.m_Focus)
                return;

            if (this.m_CharacterControllerActive)
               UpdateTransform();
		}

        private void UpdateTransform()
        {
            this.m_SmoothX = Mathf.SmoothDamp(this.m_SmoothX, this.m_MouseX, ref this.m_SmoothXVelocity, this.m_ActivePreset.TurnSmoothing);
            this.m_SmoothY = Mathf.SmoothDamp(this.m_SmoothY, this.m_MouseY, ref this.m_SmoothYVelocity, this.m_ActivePreset.TurnSmoothing);
 
            this.m_Transform.rotation = Quaternion.Euler(this.m_SmoothY, this.m_SmoothX, 0f);

            this.m_SmoothZoom = Mathf.SmoothDamp(this.m_SmoothZoom, this.m_ActivePreset.Distance + this.m_ActivePreset.Zoom, ref this.m_ZoomVelocity, this.m_ActivePreset.ZoomSmoothing);

            Vector3 targetPosition = this.Target.position + this.m_ActivePreset.Offset.x * this.m_Transform.right + this.m_ActivePreset.Offset.y * this.Target.up;
            Vector3 direction = -this.m_SmoothZoom * this.m_Transform.forward;
            Vector3 desiredPosition = targetPosition + direction;

            RaycastHit hit;
            if (Physics.SphereCast(targetPosition - direction.normalized * this.m_ActivePreset.CollisionRadius, this.m_ActivePreset.CollisionRadius, direction.normalized, out hit, direction.magnitude, this.m_ActivePreset.CollisionLayer, QueryTriggerInteraction.Ignore))
            {
                desiredPosition = hit.point + (hit.normal * 0.1f);
            }
            this.m_Transform.position = Vector3.SmoothDamp(this.m_Transform.position, desiredPosition, ref this.m_SmoothMoveVelocity, this.m_ActivePreset.MoveSmoothing);
        }

        private void UpdateInput() {
            this.m_ConsumeTurn = false;
            this.m_ConsumeZoom = this.m_ActivePreset.ConsumeInputOverUI?UnityTools.IsPointerOverUI():false;

            if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && EventSystem.current != null && UnityTools.IsPointerOverUI())
            {
                this.m_GUIClick = true;
            }



            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                this.m_GUIClick = false;
            }

            if (!this.m_GUIClick)
            {
                for (int i = 0; i < this.m_Presets.Length; i++)
                {
                    CameraSettings preset = this.m_Presets[i];
                    switch (preset.Activation)
                    {
                        case CameraSettings.ActivationType.Button:
                            if (Input.GetButtonDown(preset.InputName))
                            {
                                preset.IsActive = true;

                            }
                            if (Input.GetButtonUp(preset.InputName))
                            {
                                preset.IsActive = false;
                            }
                            break;
                        case CameraSettings.ActivationType.Toggle:
                            if (Input.GetButtonDown(preset.InputName))
                            {
                                preset.IsActive = !preset.IsActive;
                            }
                            break;
                    }
                }
            }
            for (int i = 0; i < this.m_Presets.Length; i++)
            {
                CameraSettings preset = this.m_Presets[i];
                if (preset.IsActive)
                {
                    if (this.m_ActivePreset != preset)
                    {
                        float currentZoom = this.m_ActivePreset.Zoom;

                        this.m_ActivePreset = preset;
                        Cursor.visible = this.m_ActivePreset.CursorMode == CursorLockMode.Locked ? false : true;
                        if (this.m_ActivePreset.InheritDistance)
                            this.m_ActivePreset.Zoom = currentZoom;
                        ApplyCrosshair(this.m_ActivePreset.Crosshair);
                    }
                    break;
                }
            }

            if (this.m_ActivePreset.ConsumeInputOverUI && this.m_GUIClick)
            {
                this.m_ConsumeTurn = true;
                this.m_ConsumeZoom = true;
            }
            this.m_ConsumeTurn = this.m_ActivePreset.TurnButton == "None" ? true : this.m_ConsumeTurn;

            if (!this.m_ConsumeTurn && (string.IsNullOrEmpty(this.m_ActivePreset.TurnButton) || Input.GetButton(this.m_ActivePreset.TurnButton)))
            {
                float x = Input.GetAxis("Mouse X") * this.m_ActivePreset.TurnSpeed;
                float y = Input.GetAxis("Mouse Y") * this.m_ActivePreset.TurnSpeed;

                if (this.m_ActivePreset.VisibilityDelta == 0f || Mathf.Abs(x) > this.m_ActivePreset.VisibilityDelta || Mathf.Abs(y) > this.m_ActivePreset.VisibilityDelta)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    this.m_RotatedLastFrame = true;
                }
                this.m_MouseX += x;
                this.m_MouseY -= y;


                if (Mathf.Abs(this.m_ActivePreset.YawLimit.x) + Mathf.Abs(this.m_ActivePreset.YawLimit.y) < 360)
                {
                    this.m_MouseX = ClampAngle(this.m_MouseX, this.m_ActivePreset.YawLimit.x, this.m_ActivePreset.YawLimit.y);
                }
                this.m_MouseY = ClampAngle(this.m_MouseY, this.m_ActivePreset.PitchLimit.x, this.m_ActivePreset.PitchLimit.y);
            }
            else if (this.m_RotatedLastFrame)
            {
                Cursor.lockState = this.m_ActivePreset.CursorMode;
                Cursor.visible = this.m_ActivePreset.CursorMode == CursorLockMode.Locked ? false : true;
                this.m_RotatedLastFrame = false;
            }

            if (!this.m_ConsumeZoom)
            {
                this.m_ActivePreset.Zoom -= Input.GetAxis("Mouse ScrollWheel") * this.m_ActivePreset.ZoomSpeed;
                this.m_ActivePreset.Zoom = Mathf.Clamp(this.m_ActivePreset.Zoom, this.m_ActivePreset.ZoomLimit.x - this.m_ActivePreset.Distance, this.m_ActivePreset.ZoomLimit.y - this.m_ActivePreset.Distance);
            }
        }

        public void Activate(string preset) {
            CameraSettings settings = Presets.Where(x => x.Name == preset).FirstOrDefault();
            if (settings != null)
            {
                settings.IsActive = true;
            }
        }

        public void Deactivate(string preset) {
            CameraSettings settings = Presets.Where(x => x.Name == preset).FirstOrDefault();
            if (settings != null && settings.Name != "Default")
            {
                settings.IsActive = false;
            }
        }

        private float ClampAngle (float angle, float min, float max)
		{
			do {
				if (angle < -360f)
					angle += 360f;
				if (angle > 360f)
					angle -= 360f;
			} while (angle < -360f || angle > 360f);

			return Mathf.Clamp (angle, min, max);
		}

		private void ApplyCrosshair (Sprite crosshair)
		{
			if (this.m_CrosshairImage == null) {
				CreateCrosshairUI ();
			}
			if (crosshair != null) {
				this.m_CrosshairImage.sprite = crosshair;
				this.m_CrosshairImage.SetNativeSize ();
				this.m_CrosshairImage.gameObject.SetActive (true);
			} else {
				this.m_CrosshairImage.gameObject.SetActive (false);
			}
		}

		private void CreateCrosshairUI ()
		{
			GameObject canvasGameObject = new GameObject ("Crosshair Canvas");
			this.m_CrosshairCanvas = canvasGameObject.AddComponent<Canvas> ();
			this.m_CrosshairCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            this.m_CrosshairCanvas.pixelPerfect = true;
            this.m_CrosshairCanvas.overrideSorting = true;
            this.m_CrosshairCanvas.sortingOrder = 100;
			GameObject crosshairGameObject = new GameObject ("Crosshair");
			this.m_CrosshairImage = crosshairGameObject.AddComponent<Image> ();
			crosshairGameObject.transform.SetParent (canvasGameObject.transform, false);
			crosshairGameObject.SetActive (false);
            canvasGameObject.AddComponent<DontDestroyOnLoad>();
		}
	}
}
