using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
	[System.Serializable]
	public class CameraSettings
	{
		[SerializeField]
		private string m_Name = "Camera Preset";
		[SerializeField]
		private string m_InputName;
		[SerializeField]
		private ActivationType m_Activation = ActivationType.Automatic;
		[SerializeField]
		private Sprite m_Crosshair;
		[SerializeField]
		private Vector2 m_Offset = new Vector2 (0.25f, 1.5f);
		[SerializeField]
		private float m_Distance = 2.5f;
		[SerializeField]
		private bool m_InheritDistance = true;

		[HeaderLine ("Input")]
		[SerializeField]
		private string m_TurnButton;
		[SerializeField]
		private float m_TurnSpeed = 1.5f;
		[SerializeField]
		private float m_TurnSmoothing = 0.05f;
		[MinMaxSlider (-180, 180)]
		[SerializeField]
		private Vector2 m_YawLimit = new Vector2 (-180, 180);
		[MinMaxSlider (-90f, 90f)]
		[SerializeField]
		private Vector2 m_PitchLimit = new Vector2 (-60, 60);
        [SerializeField]
        private float m_VisibilityDelta = 0.3f;

		[SerializeField]
		private float m_ZoomSpeed = 5f;
		[MinMaxSlider (0f, 25f)]
		[SerializeField]
		private Vector2 m_ZoomLimit = new Vector2 (0f, 10f);
		[SerializeField]
		private float m_ZoomSmoothing = 0.1f;
		[SerializeField]
		private float m_MoveSmoothing = 0.07f;
		[SerializeField]
		private CursorLockMode m_CursorMode;
		[SerializeField]
		private bool m_ConsumeInputOverUI = true;

		[HeaderLine ("Collision")]
		[SerializeField]
		private LayerMask m_CollisionLayer = 1 << 0;
		[SerializeField]
		private float m_CollisionRadius = 0.4f;

		private bool m_IsActive;
		private float m_Zoom;

		public string Name {
			get { 
				return this.m_Name;
			}
			set { 
				this.m_Name = value;
			}
		}

		public string InputName {
			get { 
				return this.m_InputName;
			}
			set { 
				this.m_InputName = value;
			}
		}

		public ActivationType Activation {
			get {
				return this.m_Activation;
			}
			set { 
				this.m_Activation = value;
			}
		}

		public Vector2 Offset {
			get {
				return this.m_Offset;
			}
			set { 
				this.m_Offset = value;
			}
		}

		public float Distance {
			get { 
				return this.m_Distance;
			}
			set { 
				this.m_Distance = value;
			}
		}

		public bool InheritDistance
		{
			get
			{
				return this.m_InheritDistance;
			}
			set
			{
				this.m_InheritDistance = value;
			}
		}

		public Sprite Crosshair {
			get { 
				return this.m_Crosshair;
			}
			set { 
				this.m_Crosshair = value;
			}
		}

		public string TurnButton {
			get {
				return this.m_TurnButton;
			}
			set { 
				this.m_TurnButton = value;
			}
		}

		public float TurnSpeed {
			get { 
				return this.m_TurnSpeed;
			}
			set { 
				this.m_TurnSpeed = value;
			}
		}

		public float TurnSmoothing {
			get { 
				return this.m_TurnSmoothing;
			}
			set { 
				this.m_TurnSmoothing = value;
			}
		}

		public Vector2 YawLimit {
			get { 
				return this.m_YawLimit;
			}
			set { 
				this.m_YawLimit = value;
			}
		}

		public Vector2 PitchLimit {
			get { 
				return this.m_PitchLimit;
			}
			set { 
				this.m_PitchLimit = value;
			}
		}

        public float VisibilityDelta
        {
            get
            {
                return this.m_VisibilityDelta;
            }
            set
            {
                this.m_VisibilityDelta = value;
            }
        }

        public float ZoomSpeed {
			get { 
				return this.m_ZoomSpeed;
			}
			set { 
				this.m_ZoomSpeed = value;
			}
		}

		public Vector2 ZoomLimit {
			get { 
				return this.m_ZoomLimit;
			}
			set { 
				this.m_ZoomLimit = value;
			}
		}

		public float ZoomSmoothing {
			get { 
				return this.m_ZoomSmoothing;
			}
			set { 
				this.m_ZoomSmoothing = value;
			}
		}

		public float MoveSmoothing {
			get { 
				return this.m_MoveSmoothing;
			}
			set { 
				this.m_MoveSmoothing = value;
			}
		}

		public CursorLockMode CursorMode {
			get { 
				return this.m_CursorMode;
			}
			set { 
				this.m_CursorMode = value;
			}
		}

		public bool ConsumeInputOverUI {
			get {
				return this.m_ConsumeInputOverUI;
			}
			set { 
				this.m_ConsumeInputOverUI = value;
			}
		}

		public LayerMask CollisionLayer {
			get { 
				return this.m_CollisionLayer;
			}
			set { 
				this.m_CollisionLayer = value;
			}
		}

		public float CollisionRadius {
			get { 
				return this.m_CollisionRadius;
			}
			set { 
				this.m_CollisionRadius = value;
			}
		}

		public bool IsActive {
			get { 
				return this.m_IsActive;
			}
			set { 
				this.m_IsActive = value;
			}
		}

		public float Zoom {
			get { 
				return this.m_Zoom;
			}
			set { 
				this.m_Zoom = value;
			}
		}

		public enum ActivationType
		{
			Automatic,
			Manual,
			Button,
			Toggle
		}
	}
}