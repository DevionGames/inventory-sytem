using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using UnityEngine.Audio;

namespace DevionGames
{
	public class ThirdPersonController : MonoBehaviour
	{
        [SerializeField]
		private bool m_DontDestroyOnLoad = true;
		[HeaderLine ("Input")]
		[SerializeField]
		private string m_ForwardInput = "Vertical";
		[SerializeField]
		private string m_HorizontalInput = "Horizontal";
		[SerializeField]
		private float m_SpeedMultiplier = 1f;
		[EnumFlags]
		[SerializeField]
		private AimType m_AimType = AimType.Button | AimType.Selectable;
		[SerializeField]
		private string m_AimInput = "Fire2";

		[HeaderLine ("Movement")]
		[SerializeField]
		private float m_AimRotation = 20f;
		[SerializeField]
		private float m_RotationSpeed = 10f;
		[SerializeField]
		private Vector3 m_AirSpeed = new Vector3 (0.3f, 0f, 0.3f);
		[SerializeField]
		private float m_AirDampening = 0.15f;
		[SerializeField]
		private float m_GroundDampening = 0f;
		[SerializeField]
		private float m_StepOffset = 0.2f;
		[SerializeField]
		private float m_SlopeLimit = 45f;

		[HeaderLine ("Physics")]
		[SerializeField]
		private LayerMask m_GroundLayer = 1 << 0;
		[SerializeField]
		private float m_SkinWidth = 0.08f;
		[SerializeField]
		private PhysicMaterial m_IdleFriction;
		[SerializeField]
		private PhysicMaterial m_MovementFriction;
		[SerializeField]
		private PhysicMaterial m_StepFriction;
		[SerializeField]
		private PhysicMaterial m_AirFriction;


		[HeaderLine("Footsteps")]
		[SerializeField]
		private AudioMixerGroup m_AudioMixerGroup=null;
		[SerializeField]
		private List<AudioClip> m_FootstepClips=new List<AudioClip>();

		[HeaderLine("Animator")]
		[SerializeField]
		private bool m_UseChildAnimator = false;
		[SerializeField]
		private float m_ForwardDampTime = 0.15f;
		[SerializeField]
		private float m_HorizontalDampTime = 0.15f;

		[SerializeField]
		private List<MotionState> m_Motions;


		private Dictionary<MotionState,bool> m_ToggleState;
		private Animator m_Animator;
		private Rigidbody m_Rigidbody;
		private CapsuleCollider m_CapsuleCollider;
		private Transform m_CameraTransform;
		private Transform m_Transform;

		private bool m_IsGrounded = false;
		private Vector3 m_RawInput;
		private bool m_IsAiming;
		private bool m_IsMoving;
		private Vector3 m_Velocity;
		private Quaternion m_LookRotation;
        private float m_MouseInput;

		private Vector3 m_AirVelocity;
		private Vector3 m_PrevAirVelocity;
		private Vector3 m_RootMotionForce;
        private RaycastHit m_GroundHit;

        private CharacterIK m_CharacterIK;
        private Vector3 m_AimPosition;
		private bool m_Stepping;
		private float m_Slope;
		private AnimatorStateInfo[] m_LayerStateMap;
		private Dictionary<int,MotionState[]> m_MotionStateMap;
		private bool m_ControllerActive = true;
		private bool m_GUIClick;
        private IControllerEventHandler[] m_ControllerEvents;

        private delegate void EventFunction<T>(T handler, object arg);

		private AudioSource m_AudioSource;

        protected static void Execute(IControllerGrounded handler, object grounded)
        {
            handler.OnControllerGrounded((bool)grounded);
        }

        protected static void Execute(IControllerAim handler, object aim)
        {
            handler.OnControllerAim((bool)aim);
        }

        private void ExecuteEvent<T>(EventFunction<T> func,object arg, bool includeDisabled = false) where T : IControllerEventHandler
        {
            for (int i = 0; i < this.m_ControllerEvents.Length; i++)
            {
                IControllerEventHandler handler = this.m_ControllerEvents[i];
                if (ShouldSendEvent<T>(handler, includeDisabled))
                {
                    func.Invoke((T)handler, arg);
                }
            }
        }

        //Check if we should execute the event on that handler
        protected bool ShouldSendEvent<T>(IControllerEventHandler handler, bool includeDisabled)
        {
            var valid = handler is T;
            if (!valid)
                return false;
            var behaviour = handler as Behaviour;
            if (behaviour != null && !includeDisabled)
                return behaviour.isActiveAndEnabled;

            return true;
        }


        public List<MotionState> Motions {
			get {
				return this.m_Motions;
			}
			set { 
				this.m_Motions = value;
			}
		}

		public string ForwardInput {
			get { 
				return this.m_ForwardInput;
			}
			set { 
				this.m_ForwardInput = value;
			}
		}

		public string HorizontalInput {
			get { 
				return this.m_HorizontalInput;
			}
			set { 
				this.m_HorizontalInput = value;
			}
		}

		public float SpeedMultiplier {
			get { 
				return this.m_SpeedMultiplier;
			}
			set { 
				this.m_SpeedMultiplier = value;
			}
		}

		public AimType AimType {
			get { 
				return this.m_AimType;
			}
			set { 
				this.m_AimType = value;
			}
		}

		public string AimInput {
			get { 
				return this.m_AimInput;
			}
			set { 
				this.m_AimInput = value;
			}
		}

		public float AimRotation {
			get { 
				return this.m_AimRotation;
			}
			set { 
				this.m_AimRotation = value;
			}
		}

		public float RotationSpeed {
			get { 
				return this.m_RotationSpeed;
			}
			set { 
				this.m_RotationSpeed = value;
			}
		}

		public Vector3 AirSpeed {
			get { 
				return this.m_AirSpeed;
			}
			set { 
				this.m_AirSpeed = value;
			}
		}

		public float AirDampening {
			get { 
				return this.m_AirDampening;
			}
			set { 
				this.m_AirDampening = value;
			}
		}

		public float GroundDampening {
			get { 
				return this.m_GroundDampening;
			}
			set { 
				this.m_GroundDampening = value;
			}
		}

		public float StepOffset {
			get { 
				return this.m_StepOffset;
			}
			set { 
				this.m_StepOffset = value;
			}
		}

		public float SlopeLimit {
			get { 
				return this.m_SlopeLimit;
			}
			set { 
				this.m_SlopeLimit = value;
			}
		}

		public LayerMask GroundLayer {
			get { 
				return this.m_GroundLayer;
			}
			set { 
				this.m_GroundLayer = value;
			}
		}

		public float SkinWidth {
			get { 
				return this.m_SkinWidth;
			}
			set { 
				this.m_SkinWidth = value;
			}
		}

		public PhysicMaterial IdleFriction {
			get { 
				return this.m_IdleFriction;
			}
			set { 
				this.m_IdleFriction = value;
			}
		}

		public PhysicMaterial MovementFriction {
			get { 
				return this.m_MovementFriction;
			}
			set { 
				this.m_MovementFriction = value;
			}
		}

		public PhysicMaterial StepFriction {
			get { 
				return this.m_StepFriction;
			}
			set { 
				this.m_StepFriction = value;
			}
		}

		public PhysicMaterial AirFriction {
			get { 
				return this.m_AirFriction;
			}
			set { 
				this.m_AirFriction = value;
			}
		}

		public float ForwardDampTime {
			get { 
				return this.m_ForwardDampTime;
			}
			set { 
				this.m_ForwardDampTime = value;
			}
		}

		public float HorizontalDampTime {
			get { 
				return this.m_HorizontalDampTime;
			}
			set { 
				this.m_HorizontalDampTime = value;
			}
		}

		public bool IsGrounded {
			get {
				return this.m_IsGrounded;
			}
			set {
				if (this.m_IsGrounded != value) {
					this.m_IsGrounded = value;
                    ExecuteEvent<IControllerGrounded>(Execute,this.m_IsGrounded);
				}
			}
		}

		public bool IsStepping {
			get { 
				return this.m_Stepping;
			}
		}

		public Vector3 RawInput {
			get { 
				return this.m_RawInput;
			}
			set { 
				this.m_RawInput = value;
			}
		}

		public Vector3 RelativeInput {
			get {
				Vector3 input = Vector3.zero;
				if (IsAiming) {
					input.x = this.m_RawInput.x;
					input.z = this.m_RawInput.z;
				} else {
					float forward = Mathf.Max (Mathf.Abs (this.m_RawInput.x), Mathf.Max (Mathf.Abs (this.m_RawInput.z), 1f));
					input.z = Mathf.Clamp (this.m_RawInput.magnitude, -forward, forward);
				}
				return input;
			}
		}


		public bool IsMoving {
			get { 
				return this.m_IsMoving;
			}
            private set { this.m_IsMoving = value;}
		}

		public bool IsAiming {
			get { 
				return this.m_IsAiming;
			}
            set {
                if (this.m_IsAiming != value) {
                    this.m_IsAiming = value;
                    this.m_Animator.SetFloat("Yaw Input",0f);
                    ExecuteEvent<IControllerAim>(Execute, this.m_IsAiming);
                }
            }
		}

		public Vector3 Velocity {
			get { 
				return this.m_Velocity;
			}
			set { 
				this.m_Velocity = value;
			}
		}

		public Quaternion LookRotation {
			get { 
				return this.m_LookRotation;
			}
		}

		public Vector3 RootMotionForce
		{
			get
			{
				return this.m_RootMotionForce;
			}
		}

		private void Awake()
        {
			//Physics.queriesHitTriggers = false;
			if (this.m_DontDestroyOnLoad)
			{
				DontDestroyOnLoad(gameObject);
			}
			this.m_ControllerEvents = GetComponentsInChildren<IControllerEventHandler>(true);
			this.m_Rigidbody = GetComponent<Rigidbody>();
			this.m_Transform = transform;
			this.m_Animator = GetComponent<Animator>();
			Animator childAnimator = gameObject.GetComponentsInChildren<Animator>().Where(x => x != this.m_Animator).FirstOrDefault();
			if (childAnimator != null && this.m_UseChildAnimator)
			{
				this.m_Animator.runtimeAnimatorController = childAnimator.runtimeAnimatorController;
				this.m_Animator.avatar = childAnimator.avatar;
				Destroy(childAnimator);
			}

			this.m_CapsuleCollider = GetComponent<CapsuleCollider>();
			this.m_CameraTransform = Camera.main.transform;
			this.m_CharacterIK = GetComponent<CharacterIK>();
			this.m_ToggleState = new Dictionary<MotionState, bool>();

			for (int i = 0; i < this.m_Motions.Count; i++)
			{
				this.m_Motions[i].Index = i;
				this.m_ToggleState.Add(this.m_Motions[i], false);
			}
			this.m_LayerStateMap = new AnimatorStateInfo[this.m_Animator.layerCount];
			this.m_MotionStateMap = new Dictionary<int, MotionState[]>();
			for (int j = 0; j < this.m_Animator.layerCount; j++)
			{
				AnimatorStateInfo stateInfo = this.m_Animator.GetCurrentAnimatorStateInfo(j);
				List<MotionState> states = new List<MotionState>();
				for (int k = 0; k < this.m_Motions.Count; k++)
				{
					if (m_Animator.HasState(j, Animator.StringToHash(this.m_Motions[k].State)))
					{
						this.m_Motions[k].Layer = j;
						states.Add(this.m_Motions[k]);
					}
				}
				this.m_MotionStateMap.Add(j, states.ToArray());
				this.m_LayerStateMap[j] = stateInfo;
			}
		}

        private void OnEnable()
        {
            SetControllerActive(true);
        }

        private void OnDisable()
        {
            SetControllerActive(false);
        }

        private void Update ()
		{
            
			if (!this.m_ControllerActive) {
				return;
			}

			if ((Input.GetMouseButtonDown (0) || Input.GetMouseButtonDown (1)) && EventSystem.current != null && UnityTools.IsPointerOverUI ()) {
				this.m_GUIClick = true;
			}

			if (Input.GetMouseButtonUp (0) || Input.GetMouseButtonUp (1)) {
				this.m_GUIClick = false;
			}

			this.m_RawInput = new Vector3 (Input.GetAxis (this.m_HorizontalInput), 0, Input.GetAxis (this.m_ForwardInput));

			bool aimState = false;

			if (this.m_AimType.HasFlag<AimType>(AimType.Button) && !this.m_GUIClick) {
				aimState = Input.GetButton(this.m_AimInput);
			} 
			if (this.m_AimType.HasFlag<AimType>(AimType.Axis) && !aimState) {
				float aim = Input.GetAxis(this.m_AimInput);
				if (Mathf.Abs(aim) > 0.01f)
				{
					aimState = true;
					this.m_RawInput.x = aim;
				}
			} 
			if (this.m_AimType.HasFlag<AimType>(AimType.Toggle) && Input.GetButtonDown(this.m_AimInput) && !aimState) {
				aimState = !IsAiming;
			}
			if (this.m_AimType.HasFlag<AimType>(AimType.Selectable) && !aimState) {
				aimState = SelectableObject.current != null;
			}
			IsAiming = aimState;


			/*switch (this.m_AimType) {
			case AimType.Button:
				IsAiming = Input.GetButton (this.m_AimInput) && !this.m_GUIClick;
				break;
			case AimType.Axis:
				float aim = Input.GetAxis (this.m_AimInput);
				if (Mathf.Abs (aim) > 0.01f) {
					IsAiming = true;
					this.m_RawInput.x = aim;
				} else {
					IsAiming = false;

				}
				break;
			case AimType.Toggle:
				if (Input.GetButtonDown (this.m_AimInput)) {
					IsAiming = !IsAiming;
				}
				break;
				case AimType.Selectable:
					IsAiming = SelectableObject.current != null;
					break;
			}*/


            for (int j = 0; j < this.m_Motions.Count; j++) {
				MotionState motion = this.m_Motions [j];
				if (!motion.isActiveAndEnabled || motion.ConsumeInputOverUI && this.m_GUIClick) {
					continue;
				}
				if (motion.StartType != StartType.Down && motion.StopType != StopType.Toggle || !Input.GetButtonDown (motion.InputName)) {
					if (motion.StopType == StopType.Up && Input.GetButtonUp (motion.InputName)) {
						this.TryStopMotion (motion);
						this.m_ToggleState [motion] = motion.StopType == StopType.Up && motion.IsActive;
					}
				} else if (!motion.IsActive && motion.StartType == StartType.Down) {
					this.TryStartMotion (motion);
					this.m_ToggleState [motion] = (motion.StopType == StopType.Toggle || motion.StopType == StopType.Up) && motion.IsActive;
				} else if (motion.StopType == StopType.Toggle) {
					this.TryStopMotion (motion);
					this.m_ToggleState [motion] = motion.StopType == StopType.Toggle && motion.IsActive;
					break;
				} 
				if (motion.StartType == StartType.Press && Input.GetButton (motion.InputName)) {
					this.TryStartMotion (motion);
				}
			}
        }

		private void FixedUpdate ()
		{
			if (!this.m_ControllerActive) {
				return;
			}
			for (int i = 0; i < this.m_Motions.Count; i++) {
				MotionState motion = this.m_Motions [i];
				if (!motion.isActiveAndEnabled) {
					continue;
				}
				if (!motion.IsActive && (motion.StartType == StartType.Automatic || this.m_ToggleState [motion])) {
					this.TryStartMotion (motion);
				}
				if (motion.IsActive) {
					if (motion.StopType == StopType.Automatic && motion.CanStop ()) {
						this.TryStopMotion (motion);
					}
				}
			}
			this.m_LookRotation = Quaternion.Euler (this.m_Transform.eulerAngles.x, this.m_CameraTransform.eulerAngles.y, this.m_Transform.eulerAngles.z);

			this.m_Velocity = this.m_Rigidbody.velocity;
			if (this.IsGrounded) {
				this.m_Velocity.x = this.m_RootMotionForce.x;
				this.m_Velocity.z = this.m_RootMotionForce.z;
				float force = this.m_Animator.GetFloat ("Force");
				this.m_Velocity += this.m_Transform.TransformDirection (this.RelativeInput * force);
			}
			this.CheckGround ();
			this.CheckStep ();
			this.UpdateVelocity ();
			this.UpdateFrictionMaterial ();
			this.UpdateRotation ();
			this.UpdateAnimator ();
			this.m_Rigidbody.velocity = this.m_Velocity;
		}

		private void OnAnimatorMove ()
		{
            if (!this.m_ControllerActive)
            {
                return;
            }
            this.m_RootMotionForce = this.m_Animator.deltaPosition / Time.deltaTime;
        }

		public void UpdateVelocity ()
		{

			for (int i = 0; i < this.m_Motions.Count; i++) {
				MotionState motion = this.m_Motions [i];
				if (motion.IsActive && !motion.UpdateVelocity (ref this.m_Velocity)) {
					return;
				}
			}

			if (IsGrounded) {
				this.m_Velocity.x = this.m_Velocity.x / (1 + this.m_GroundDampening);
				this.m_Velocity.z = this.m_Velocity.z / (1 + this.m_GroundDampening);
			} else {
				this.m_AirVelocity.y = 0f;
				this.m_AirVelocity += (this.m_Transform.TransformDirection (Vector3.Scale (RelativeInput, this.m_AirSpeed)) - this.m_PrevAirVelocity); 
				this.m_Velocity += this.m_AirVelocity;
				this.m_PrevAirVelocity = this.m_AirVelocity;
				this.m_Velocity.x = this.m_Velocity.x / (1f + this.m_AirDampening);
				this.m_Velocity.z = this.m_Velocity.z / (1f + this.m_AirDampening);
			}
		}

		private void UpdateAnimator ()
		{
			for (int i = 0; i < this.m_Motions.Count; i++) {
				MotionState motion = this.m_Motions [i];
				if (motion.IsActive && !motion.UpdateAnimator ()) {
					return;
				}
			}
	
			this.m_Animator.SetFloat ("Forward Input", RelativeInput.z * this.m_SpeedMultiplier, this.m_ForwardDampTime, Time.deltaTime);
			this.m_Animator.SetFloat ("Horizontal Input", RelativeInput.x * this.m_SpeedMultiplier, this.m_HorizontalDampTime, Time.deltaTime);
            this.IsMoving = RelativeInput.sqrMagnitude > 0.01f;
            this.m_Animator.SetBool("Moving",this.m_IsMoving);
            if(this.m_IsAiming)
               this.m_Animator.SetFloat("Yaw Input",Normalize(GetSignedAngle(this.m_Transform.rotation,this.m_LookRotation, Vector3.up)*this.m_AimRotation,-180,180),0.15f,Time.deltaTime);
        }

        float Normalize(float input, float min, float max)
        {
            float average = (min + max) / 2;
            float range = (max - min) / 2;
            float normalized_x = (input - average) / range;
            return normalized_x;
        }

        private float GetSignedAngle(Quaternion a, Quaternion b, Vector3 axis)
        {
            float angle = 0f;
            Vector3 angleAxis = Vector3.zero;
            Quaternion q = (b * Quaternion.Inverse(a));
            q.ToAngleAxis(out angle, out angleAxis);
            if (Vector3.Angle(axis, angleAxis) > 90f)
            {
                angle = -angle;
            }
            return Mathf.DeltaAngle(0f, angle);
        }

        private void UpdateRotation ()
		{
			for (int i = 0; i < this.m_Motions.Count; i++) {
				MotionState motion = this.m_Motions [i];
				if (motion.IsActive && !motion.UpdateRotation ()) {
					return;
				}
			}

			Quaternion rotation = this.m_Transform.rotation;
			if (IsAiming) {
				rotation = this.m_LookRotation;
			} else if (this.m_RawInput.sqrMagnitude > 0.01f) {
				rotation = Quaternion.LookRotation (this.m_LookRotation * this.m_RawInput);
             
			}
			this.m_Transform.rotation = Quaternion.Slerp (this.m_Transform.rotation, rotation, (IsAiming ? this.m_AimRotation : this.m_RotationSpeed) * Time.fixedDeltaTime);
		}

		public void UpdateFrictionMaterial ()
		{
			if (this.IsGrounded) {
				if (this.m_Stepping) {
					this.m_CapsuleCollider.material = this.m_StepFriction;
				} else if (this.IsMoving) {
					this.m_CapsuleCollider.material = this.m_MovementFriction;
				} else {
					this.m_CapsuleCollider.material = this.m_IdleFriction;
				}
			} else {
				this.m_CapsuleCollider.material = this.m_AirFriction;
			}
		}

        public void DeterminanteDefaultStates() {
            for (int j = 0; j < this.m_Animator.layerCount; j++)
            {
                AnimatorStateInfo stateInfo = this.m_Animator.GetCurrentAnimatorStateInfo(j);
                if (!stateInfo.IsTag("Default")){ continue; }

                MotionState[] states;
                if (m_MotionStateMap.TryGetValue(j, out states)){
                    if (Array.Exists(states, motion => stateInfo.IsName(motion.GetDestinationState()))) {
                        continue;
                    }
                }
                this.m_LayerStateMap[j] = stateInfo;
            }
        }

        public void CheckDefaultAnimatorStates ()
		{
			for (int j = 0; j < this.m_LayerStateMap.Length; j++) {
				if (this.m_MotionStateMap [j].Length > 0) {
					bool active = false;
					for (int k = 0; k < this.m_MotionStateMap [j].Length; k++) {

						if (this.m_MotionStateMap [j] [k].IsActive) {
							active = true;
						}
					}
					if (!active && this.m_Animator.GetCurrentAnimatorStateInfo (j).shortNameHash != this.m_LayerStateMap [j].shortNameHash && !this.m_Animator.IsInTransition (j)) {
						//Debug.Log("Current: "+this.m_Animator.GetCurrentAnimatorClipInfo(j)[0].clip.name);
                        this.m_Animator.CrossFadeInFixedTime (this.m_LayerStateMap [j].shortNameHash, 0.3f);
						//this.m_Animator.Update(0f);
						//Debug.Log("Next: " + this.m_Animator.GetNextAnimatorClipInfo(j)[0].clip.name);
					}
				}
			}
		}

		public void CheckGround ()
		{
			for (int i = 0; i < this.m_Motions.Count; i++) {
				MotionState motion = this.m_Motions [i];
				if (motion.IsActive && !motion.CheckGround ()) {
					return;
				}
			}
				
			if (Physics.SphereCast (this.m_Transform.position + this.m_Transform.up * this.m_CapsuleCollider.radius * 2f, this.m_CapsuleCollider.radius, -this.m_Transform.up, out this.m_GroundHit, this.m_CapsuleCollider.radius * 2f + this.m_SkinWidth, this.m_GroundLayer)) {
				if (!this.m_Stepping && Physics.Raycast (this.m_Transform.position + this.m_CapsuleCollider.center, -this.m_Transform.up, out this.m_GroundHit, this.m_CapsuleCollider.height * 0.5f + this.m_SkinWidth * 0.9f, this.m_GroundLayer, QueryTriggerInteraction.Ignore)) {
					this.m_Velocity = Vector3.ProjectOnPlane (this.m_Velocity, this.m_GroundHit.normal);
				} else {
					this.m_Velocity.y = this.m_Velocity.y - 6f * Time.fixedDeltaTime;
				}
				this.IsGrounded = true;
			} else {
				this.IsGrounded = false;
			}

		}

		public void CheckStep ()
		{
			for (int i = 0; i < this.m_Motions.Count; i++)
			{
				MotionState motion = this.m_Motions[i];
				if (motion.IsActive && !motion.CheckStep())
				{
					this.m_Slope = -1f;
					return;
				}
			}
			Vector3 velocity = this.m_Velocity;
			velocity.y = 0f;
			if (this.RelativeInput.sqrMagnitude > velocity.sqrMagnitude) {
				velocity = this.m_Transform.TransformDirection (RelativeInput);
			}
			RaycastHit hitInfo;
			bool prevSlope = this.m_Slope != -1f;
			this.m_Slope = -1f;
			this.m_Stepping = false;

			if (velocity.sqrMagnitude > 0.001f && Physics.Raycast (this.m_Transform.position + this.m_Transform.up * 0.1f, velocity.normalized, out hitInfo, this.m_CapsuleCollider.radius + 0.2f,this.m_GroundLayer, QueryTriggerInteraction.Ignore)) {
				
				float slope = Mathf.Acos (Mathf.Clamp (hitInfo.normal.y, -1f, 1f)) * Mathf.Rad2Deg;
				if (slope > this.m_SlopeLimit) {
					Vector3 direction = hitInfo.point - this.m_Transform.position;
					direction.y = 0f;
					Physics.Raycast ((hitInfo.point + (Vector3.up * this.m_StepOffset)) + (direction.normalized * 0.1f), Vector3.down, out hitInfo, this.m_StepOffset + 0.1f,m_GroundLayer,QueryTriggerInteraction.Ignore);
					if (Mathf.Acos (Mathf.Clamp (hitInfo.normal.y, -1f, 1f)) * Mathf.Rad2Deg > this.m_SlopeLimit) {
						this.m_Velocity.x *= this.m_GroundDampening;
						this.m_Velocity.z *= this.m_GroundDampening;
					} else {
						Vector3 position = this.m_Transform.position;
						float y = position.y;
						position.y = Mathf.MoveTowards (y, position.y + this.m_StepOffset, Time.deltaTime);
						this.m_Transform.position = position;
						this.m_Velocity.y = 0f;
						this.m_Stepping = true;
					}
				} else {
					this.m_Slope = slope;
					this.m_Velocity.y = 0f;
				}
			}
			if (prevSlope && this.m_Slope == -1f) {
				this.m_Velocity.y = 0f;
			}
	

		}

		private void TryStopMotion (MotionState motion)
		{
			if (motion.IsActive) {
				motion.StopMotion (false);
			}
		}

		private void TryStartMotion (MotionState motion)
		{
			if (motion.Layer > 0 && !this.m_Animator.GetCurrentAnimatorStateInfo(motion.Layer).IsTag("Interruptable")) {
				return;
			}

            if (!motion.IsActive && motion.CanStart ()) {
				if (!string.IsNullOrEmpty (motion.GetDestinationState ())) {
					for (int j = 0; j < this.m_Motions.Count; j++) {
						if (this.m_Motions [j].IsActive && this.m_Motions [j].Layer == motion.Layer && !string.IsNullOrEmpty (this.m_Motions [j].GetDestinationState ())) {
							if (j > motion.Index) {
								this.m_Motions [j].StopMotion (true);
							} else {
								return;
							}
						}
					}
				}
				if(!string.IsNullOrEmpty(motion.State))
					DeterminanteDefaultStates();
                motion.StartMotion ();
			}
		}

		public void SetControllerActive (bool active)
		{
            if (this.m_ControllerActive != active ) {
                EventHandler.Execute<bool>(gameObject, "OnSetControllerActive", active);
            }
			this.m_ControllerActive = active;
            if (!this.m_ControllerActive) {
                this.m_RawInput = Vector3.zero;
                this.m_Animator.SetFloat("Forward Input", 0f);
                this.m_Animator.SetFloat("Horizontal Input", 0f);
                this.m_Rigidbody.velocity = Vector3.zero;
            }
			enabled = active;
		}

        public void SetMotionEnabled(object[] data) {
            string name = (string)data[0];
            bool state = (bool)data[1];
            SetMotionEnabled(name, state);
        }

        public void SetMotionEnabled(string name, bool state) {
            MotionState motion = GetMotion(name);
            if (motion != null) {
                motion.enabled = state;
                if (!state) {
                    motion.StopMotion(true);
                }
            }
        }

        public MotionState GetMotion(string name)
        {
            for (int i = 0; i < this.m_Motions.Count; i++)
            {
                MotionState motion = this.m_Motions[i];
                if (motion.FriendlyName == name) {
                    return motion;
                }
            }
            return null;
        }

		private void Footsteps(AnimationEvent evt) {
			if (!this.m_Animator.GetCurrentAnimatorStateInfo(1).IsName("Empty")) {
				return;
			}
			if (this.m_IsGrounded && m_Rigidbody.velocity.sqrMagnitude > 0.5f && this.m_FootstepClips.Count > 0 && evt.animatorClipInfo.weight > 0.5f)
			{
				
				float volume = evt.animatorClipInfo.weight;
				AudioClip clip = this.m_FootstepClips[UnityEngine.Random.Range(0,this.m_FootstepClips.Count)];
				PlaySound(clip, volume);
			}
		}

		private void PlaySound(AudioClip clip, float volume)
		{
			if (clip == null){return;}

			if (this.m_AudioSource == null)
			{
				this.m_AudioSource = gameObject.AddComponent<AudioSource>();
				this.m_AudioSource.outputAudioMixerGroup = this.m_AudioMixerGroup;
				this.m_AudioSource.spatialBlend = 1f;

			}
			if (this.m_AudioSource != null)
			{
				this.m_AudioSource.PlayOneShot(clip, volume);
			}
		}

		public void CopyProperties (ThirdPersonController other)
		{
			this.m_DontDestroyOnLoad = other.m_DontDestroyOnLoad;
			this.m_ForwardInput = other.m_ForwardInput;
			this.m_HorizontalInput = other.m_HorizontalInput;
			this.m_SpeedMultiplier = other.m_SpeedMultiplier;
			this.m_AimType = other.m_AimType;
			this.m_AimInput = other.m_AimInput;
			this.m_AimRotation = other.m_AimRotation;
			this.m_RotationSpeed = other.m_RotationSpeed;
			this.m_AirSpeed = other.m_AirSpeed;
			this.m_AirDampening = other.m_AirDampening;
			this.m_GroundDampening = other.m_GroundDampening;
			this.m_StepOffset = other.m_StepOffset;
			this.m_SlopeLimit = other.m_SlopeLimit;
			this.m_GroundLayer = other.m_GroundLayer;
			this.m_SkinWidth = other.m_SkinWidth;
			this.m_IdleFriction = other.m_IdleFriction;
			this.m_MovementFriction = other.m_MovementFriction;
			this.m_StepFriction = other.m_StepFriction;
			this.m_AirFriction = other.m_AirFriction;
			this.m_ForwardDampTime = other.m_ForwardDampTime;
			this.m_HorizontalDampTime = other.m_HorizontalDampTime;
			/*this.m_LookOffset = other.m_LookOffset;
			this.m_BodyWeight = other.m_BodyWeight;
			this.m_HeadWeight = other.m_HeadWeight;
			this.m_EyesWeight = other.m_EyesWeight;
			this.m_ClampWeight = other.m_ClampWeight;*/

			this.m_Motions = new List<MotionState> ();
			for (int i = 0; i < other.Motions.Count; i++) {
				MotionState motion = CopyComponent<MotionState> (other.Motions [i], gameObject);
				this.m_Motions.Add (motion);

			}
		}

		T CopyComponent<T> (T original, GameObject destination) where T : Component
		{
			System.Type type = original.GetType ();
			Component copy = destination.AddComponent (type);
			System.Reflection.FieldInfo[] fields = type.GetAllSerializedFields ();
			foreach (System.Reflection.FieldInfo field in fields) {
				if (field.IsPrivate && !field.HasAttribute<SerializeField> ()) {
					continue;
				}
				field.SetValue (copy, field.GetValue (original));

			}
			return copy as T;
		}
    }

	[System.Flags]
	public enum AimType
	{
		Button = 1,
		Axis = 2,
		Toggle = 4,
		Selectable = 8
	}
}