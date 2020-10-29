using UnityEngine;
using System.Collections;

namespace DevionGames.UIWidgets
{
	/// <summary>
	/// UIWidget is responsible for the management of widgets as well as animating them. 
	/// Your custom widgets should extend from this class or from child classes. 
	/// This way you can always track existing widgets by name in your game using WidgetUtility.Find<T>(name).
	/// </summary>
	[RequireComponent (typeof(CanvasGroup))]
	public class UIWidget : CallbackHandler
	{
		/// <summary>
		/// Name of the widget.
		/// </summary>
		[Tooltip("Name of the widget. You can find a reference to a widget with WidgetUtility.Find<T>(name).")]
		[SerializeField]
		private new string name;

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name {
			get{ return name; }
			set{ name = value; }
		}

		/// <summary>
		/// Callbacks for Inspector.
		/// </summary>
        public override string[] Callbacks
        {
            get
            {
                return new string[]{
                    "OnShow",
                    "OnClose",
                };
            }
        }

		/// <summary>
		/// Widgets with higher priority will be prefered when used with WidgetUtility.Find<T>(name).
		/// </summary>
		[Tooltip("Widgets with higher priority will be prefered when used with WidgetUtility.Find<T>(name).")]
        [Range (0, 100)]
		public int priority;

        /// <summary>
        /// Key to toggle show and close
        /// </summary>
        [Header("Appearence")]
		[Tooltip("Key to show or close this widget.")]
        [SerializeField]
        private KeyCode m_KeyCode = KeyCode.None;

		[Tooltip("Easing equation type used to tween this widget.")]
		[SerializeField]
		private EasingEquations.EaseType m_EaseType= EasingEquations.EaseType.EaseInOutBack;

        /// <summary>
        /// The duration to tween this widget.
        /// </summary>
		[Tooltip("The duration to tween this widget.")]
		[SerializeField]
		private float m_Duration = 0.7f;

        /// <summary>
        /// The AudioClip that will be played when this widget shows.
        /// </summary>
		[Tooltip("The AudioClip that will be played when this widget shows.")]
		[SerializeField]
        protected AudioClip m_ShowSound;

		/// <summary>
		/// The AudioClip that will be played when this widget closes.
		/// </summary>
		[Tooltip("The AudioClip that will be played when this widget closes.")]
		[SerializeField]
		protected AudioClip m_CloseSound;

		/// <summary>
		/// Brings this window to front in Show()
		/// </summary>
		[Tooltip("Focus the widget. This will bring the widget to front when it is shown.")]
		[SerializeField]
        private bool m_Focus = true;

        /// <summary>
        /// If true deactivates the gameobject when closed.
        /// </summary>
		[Tooltip("If true, deactivates the game object when it gets closed. This prevets Update() to be called every frame.")]
        [SerializeField]
		protected bool m_DeactivateOnClose = true;

		/// <summary>
		/// Gets a value indicating whether this widget is visible.
		/// </summary>
		/// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
		public bool IsVisible { 
			get { 
				return m_CanvasGroup.alpha == 1f; 
			} 
		}

		/// <summary>
		/// The RectTransform of the widget.
		/// </summary>
		protected RectTransform m_RectTransform;
		/// <summary>
		/// The CanvasGroup of the widget.
		/// </summary>
		protected CanvasGroup m_CanvasGroup;
		/// <summary>
		/// Checks if Show() is already called. This prevents from calling Show() multiple times when the widget is not finished animating. 
		/// </summary>
		protected bool m_IsShowing;

		private TweenRunner<FloatTween> m_AlphaTweenRunner;
		private TweenRunner<Vector3Tween> m_ScaleTweenRunner;
       
        private void Awake ()
		{
			//Register the KeyCode to show or close the widget.
			WidgetInputHandler.RegisterInput(this.m_KeyCode, this);
			m_RectTransform = GetComponent<RectTransform> ();
			m_CanvasGroup = GetComponent<CanvasGroup> ();

			if (!IsVisible) {
				//Set local scale to zero, when widget is not visible. Used to correctly animate the widget.
				m_RectTransform.localScale = Vector3.zero;
			}
			if (this.m_AlphaTweenRunner == null)
				this.m_AlphaTweenRunner = new TweenRunner<FloatTween> ();
			this.m_AlphaTweenRunner.Init (this);
			
			if (this.m_ScaleTweenRunner == null)
				this.m_ScaleTweenRunner = new TweenRunner<Vector3Tween> ();
			this.m_ScaleTweenRunner.Init (this);
            m_IsShowing = IsVisible;
			OnAwake ();
		}

		protected virtual void OnAwake ()
		{
		}

		private void Start ()
		{
			OnStart ();
			StartCoroutine (OnDelayedStart ());
		}

		protected virtual void OnStart ()
		{
		}

		private IEnumerator OnDelayedStart ()
		{
			yield return null;
			if (!IsVisible && m_DeactivateOnClose) {
				gameObject.SetActive (false);
			}
		}

		/// <summary>
		/// Show this widget.
		/// </summary>
		public virtual void Show ()
		{
            if (this.m_IsShowing) {
                return;
            }
            this.m_IsShowing = true;
			gameObject.SetActive (true);
            if (this.m_Focus) {
				Focus ();
			}
			TweenCanvasGroupAlpha (m_CanvasGroup.alpha, 1f);
			TweenTransformScale (Vector3.ClampMagnitude (m_RectTransform.localScale, 1.9f), Vector3.one);
			
			WidgetUtility.PlaySound (this.m_ShowSound, 1.0f);
			m_CanvasGroup.interactable = true;
			m_CanvasGroup.blocksRaycasts = true;
			Execute("OnShow", new CallbackEventData());
		}

		/// <summary>
		/// Close this widget.
		/// </summary>
		public virtual void Close ()
		{
            if (!m_IsShowing) {
                return;
            }
            m_IsShowing = false;
			TweenCanvasGroupAlpha (m_CanvasGroup.alpha, 0f);
			TweenTransformScale (m_RectTransform.localScale, Vector3.zero);
			
			WidgetUtility.PlaySound (this.m_CloseSound, 1.0f);
			m_CanvasGroup.interactable = false;
			m_CanvasGroup.blocksRaycasts = false;
			Execute("OnClose", new CallbackEventData());

		}

		private void TweenCanvasGroupAlpha (float startValue, float targetValue)
		{
				FloatTween alphaTween = new FloatTween {
					easeType = m_EaseType,
					duration = m_Duration,
					startValue = startValue,
					targetValue = targetValue
				};

				alphaTween.AddOnChangedCallback ((float value) => {
					m_CanvasGroup.alpha = value;
				});
				alphaTween.AddOnFinishCallback (() => {
					if (alphaTween.startValue > alphaTween.targetValue) {
						if (m_DeactivateOnClose && !this.m_IsShowing) {
							gameObject.SetActive (false);
						}
					} 
				});
			
			m_AlphaTweenRunner.StartTween (alphaTween);
		}

		private void TweenTransformScale (Vector3 startValue, Vector3 targetValue)
		{
            Vector3Tween scaleTween = new Vector3Tween
            {
                easeType = m_EaseType,
                duration = m_Duration,
                startValue = startValue,
                targetValue = targetValue
            };
            scaleTween.AddOnChangedCallback((Vector3 value) => {
                m_RectTransform.localScale = value;
            });

            m_ScaleTweenRunner.StartTween(scaleTween);
        }

		/// <summary>
		/// Toggle the visibility of this widget.
		/// </summary>
		public virtual void Toggle ()
		{
			if (!IsVisible) {
				Show ();
			} else {
				Close ();
			}
		}

		/// <summary>
		/// Brings the widget to the top
		/// </summary>
		public virtual void Focus ()
		{
			m_RectTransform.SetAsLastSibling ();
		}

		protected virtual void OnDestroy() {
			//Unregister input key
			WidgetInputHandler.UnregisterInput(this.m_KeyCode, this);
		}
	}
}