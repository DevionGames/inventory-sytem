using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.UI;

namespace DevionGames.UIWidgets
{
	[RequireComponent (typeof(CanvasGroup))]
	public class UIWidget : CallbackHandler
	{
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

        [Range (0, 100)]
		public int priority;

        /// <summary>
        /// Key to toggle show and close
        /// </summary>
        [Header("Appearence")]
        [SerializeField]
        private KeyCode m_KeyCode = KeyCode.None;

		[SerializeField]
		private EasingEquations.EaseType m_EaseType= EasingEquations.EaseType.EaseInOutBack;
        /// <summary>
        /// The duration to tween this widget.
        /// </summary>
        [SerializeField]
		private float m_Duration = 0.7f;
        /// <summary>
        /// The AudioClip that will be played when this widget shows.
        /// </summary>
        public AudioClip showSound;
		/// <summary>
		/// The AudioClip that will be played when this widget closes.
		/// </summary>
		public AudioClip closeSound;
        /// <summary>
        /// Brings this window to front in Show()
        /// </summary>
        [SerializeField]
        private bool m_Focus = true;
        /// <summary>
        /// If true deactivates the gameobject when closed.
        /// </summary>
        [SerializeField]
		protected bool m_DeactivateOnClose = true;
		/// <summary>
		/// Events that will be invoked when this widget is shows.
		/// </summary>
		[HideInInspector]
		public WidgetEvent onShow;
		/// <summary>
		/// Events that will be invoked when this widget closes.
		/// </summary>
		[HideInInspector]
		public WidgetEvent onClose;

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
		
		private TweenRunner<FloatTween> m_AlphaTweenRunner;
		private TweenRunner<Vector3Tween> m_ScaleTweenRunner;
        protected bool m_IsShowing;


        private void Awake ()
		{
			WidgetInputHandler.RegisterInput(this.m_KeyCode, this);
			m_RectTransform = GetComponent<RectTransform> ();
			m_CanvasGroup = GetComponent<CanvasGroup> ();
			if (!IsVisible) {
				m_RectTransform.localScale = Vector3.zero;
			}
			if (this.m_AlphaTweenRunner == null)
				this.m_AlphaTweenRunner = new TweenRunner<FloatTween> ();
			this.m_AlphaTweenRunner.Init (this);
			
			if (this.m_ScaleTweenRunner == null)
				this.m_ScaleTweenRunner = new TweenRunner<Vector3Tween> ();
			this.m_ScaleTweenRunner.Init (this);
			
			onShow.AddListener (delegate {
				Execute ("OnShow", new CallbackEventData());
			});
			onClose.AddListener (delegate {
				Execute ("OnClose", new CallbackEventData());
			});
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

            if (m_IsShowing) {
                return;
            }
            m_IsShowing = true;
			gameObject.SetActive (true);
            if (m_Focus) {
				Focus ();
			}
			TweenCanvasGroupAlpha (m_CanvasGroup.alpha, 1f);
			TweenTransformScale (Vector3.ClampMagnitude (m_RectTransform.localScale, 1.9f), Vector3.one);
			
			WidgetUtility.PlaySound (showSound, 1.0f);
			m_CanvasGroup.interactable = true;
			m_CanvasGroup.blocksRaycasts = true;
			if (onShow != null) {
				onShow.Invoke ();
			}

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
			
			WidgetUtility.PlaySound (closeSound, 1.0f);
			m_CanvasGroup.interactable = false;
			m_CanvasGroup.blocksRaycasts = false;
			if (onClose != null) {
				onClose.Invoke ();
			}
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
			WidgetInputHandler.UnregisterInput(m_KeyCode, this);
		}

		[System.Serializable]
		public class WidgetEvent:UnityEvent
		{

		}
	}
}