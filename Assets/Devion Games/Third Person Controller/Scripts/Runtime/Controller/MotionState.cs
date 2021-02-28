using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DevionGames
{
	public abstract class MotionState : MonoBehaviour
	{

		[HideInInspector]
		[SerializeField]
		private string m_FriendlyName = string.Empty;

		public string FriendlyName {
			get {
				return this.m_FriendlyName;
			}
		}

		[SerializeField]
		private string m_InputName = string.Empty;

		public string InputName {
			get { 
				return this.m_InputName;
			}
			set {
				this.m_InputName = value;
			}
		}

		[SerializeField]
		private StartType m_StartType= StartType.Automatic;

		public StartType StartType {
			get {
				return this.m_StartType;
			}
			set { this.m_StartType = value; }
		}

		[SerializeField]
		private StopType m_StopType = StopType.Automatic;

		public StopType StopType {
			get { 
				return this.m_StopType;
			}
			set { this.m_StopType = value; }
		}

        [SerializeField]
        private bool m_ConsumeInputOverUI=false;

        public bool ConsumeInputOverUI
        {
            get
            {
                return this.m_ConsumeInputOverUI;
            }
        }

		[SerializeField]
		private bool m_PauseItemUpdate = true;

		public bool PauseItemUpdate
		{
			get
			{
				return this.m_PauseItemUpdate;
			}
		}

		[SerializeField]
		private float m_TransitionDuration = 0.2f;

		public float TransitionDuration {
			get { 
				return this.m_TransitionDuration;
			}
		}

		[SerializeField]
		private string m_State;

		public string State {
			get { 
				return this.m_State;
			}
			set {
				this.m_State = value;
			}
		}

		[SerializeField]
		private string m_CameraPreset = "Default";
		public string CameraPreset {
			get { return this.m_CameraPreset; }
			set { this.m_CameraPreset = value; }
		}


		private bool m_IsActive;

		public bool IsActive {
			get {
				return this.m_IsActive;
			}
		}

		private int m_Index;

		public int Index {
			get { 
				return this.m_Index;
			}
			set {
				this.m_Index = value;
			}
		}

		private int m_Layer;

		public int Layer {
			get { 
				return this.m_Layer;
			}
			set { 
				this.m_Layer = value;
			}
		}

		public ThirdPersonController Controller {
			get { 
				return this.m_Controller;
			}
			set { 
				this.m_Controller = value;
			}
		}

		protected Animator m_Animator;
		protected ThirdPersonCamera m_Camera;
		protected Rigidbody m_Rigidbody;
		protected CapsuleCollider m_CapsuleCollider;
		protected ThirdPersonController m_Controller;
		protected Transform m_Transform;
		protected bool m_InPosition = true;


		private void Start ()
		{
			//this.m_Transform = transform.root;
			this.m_Transform = transform;
			this.m_Animator = this.m_Transform.GetComponent<Animator> ();
			this.m_Rigidbody = this.m_Transform.GetComponent<Rigidbody> ();
			this.m_CapsuleCollider = this.m_Transform.GetComponent<CapsuleCollider> ();
			this.m_Camera = Camera.main.GetComponent<ThirdPersonCamera>();

			ThirdPersonController[] controllers = this.m_Transform.GetComponents<ThirdPersonController> ();
			for (int i = 0; i < controllers.Length; i++) {
				if (controllers [i].enabled) {
					this.m_Controller = controllers [i];
				}
			}
		}

		private void StopMotion ()
		{
			if (IsActive) {
				StopMotion (true);
			}

		}

		public void StopMotion (bool force)
		{
			if (!this.m_IsActive || !force && !this.CanStop ()) {
				return;
			}

			if(PauseItemUpdate)
				SendMessage("PauseItemUpdate", false, SendMessageOptions.DontRequireReceiver);
			this.m_IsActive = false;
			OnStop ();
			if(!string.IsNullOrEmpty(GetDestinationState()))
				m_Controller.CheckDefaultAnimatorStates();

			this.m_Camera.Deactivate(CameraPreset);
			/*CameraSettings preset = this.m_Camera.Presets.Where(x => x.Name == CameraPreset).FirstOrDefault();
			if (preset != null && preset.Name != "Default")
			{
				preset.IsActive = false;
			}*/
			//Debug.Log("Stop Motion "+FriendlyName);

		}



		public void StartMotion ()
		{
			if (PauseItemUpdate)
				SendMessage("PauseItemUpdate", true, SendMessageOptions.DontRequireReceiver);
			this.m_IsActive = true;

			OnStart ();

			this.m_Camera.Activate(CameraPreset);
			/*CameraSettings preset = this.m_Camera.Presets.Where(x => x.Name == CameraPreset).FirstOrDefault();
			if (preset != null)
			{
				preset.IsActive = true;
			}*/


			string destinationState = GetDestinationState ();
			if (!string.IsNullOrEmpty (destinationState)) {
				m_Animator.CrossFadeInFixedTime (destinationState, TransitionDuration);
			}
	
			//Debug.Log("Start Motion " + FriendlyName);
		}

		public bool IsPlaying() {
			int layers = this.m_Animator.layerCount;
			string destinationState = GetDestinationState();
			for (int i = 0; i < layers; i++) {
				AnimatorStateInfo info = this.m_Animator.GetCurrentAnimatorStateInfo(i);
				if (info.IsName(destinationState))
					return true;
			}
			return false;
		}


		public virtual bool CanStart()
		{
			return true;
		}

		public virtual void OnStart()
		{

		}

		public virtual bool UpdateVelocity (ref Vector3 velocity)
		{
			return true;
		}

		public virtual bool UpdateRotation ()
		{
			return true;
		}

		public virtual bool UpdateAnimator ()
		{
			return true;
		}


		public virtual bool UpdateAnimatorIK (int layer)
		{
			return true;
		}

		public virtual bool CheckGround ()
		{
			return true;
		}

		public virtual bool CheckStep()
		{
			return true;
		}

		public virtual bool CanStop()
		{
			return true;
		}

		public virtual void OnStop ()
		{
		
		}

		public virtual string GetDestinationState ()
		{
			return this.m_State;
		}

		protected void MoveToTarget (Transform transform, Vector3 position, Quaternion rotation, float time, System.Action onComplete)
		{
			StartCoroutine (MoveToTargetInternal (transform, position, rotation, time, onComplete));

		}

		private IEnumerator MoveToTargetInternal (Transform transform, Vector3 position, Quaternion rotation, float time, System.Action onComplete)
		{
			this.m_InPosition = false;
			float elapsedTime = 0f;
			Vector3 startingPosition = transform.position;
			Quaternion startingRotation = transform.rotation;
			while (elapsedTime < time) {
				transform.position = Vector3.Lerp (startingPosition, position, (elapsedTime / time));
				transform.rotation = Quaternion.Slerp (startingRotation, rotation, (elapsedTime / time));
				elapsedTime += Time.fixedDeltaTime;
				yield return new WaitForEndOfFrame ();
			}
			this.m_InPosition = true;

			if (onComplete != null) {
				onComplete.Invoke ();
			}
		}
	
	}

	public enum StartType
	{
		Automatic,
		Down,
		Press
	}

	public enum StopType
	{
		Automatic,
		Manual,
		Up,
		Toggle
	}
}