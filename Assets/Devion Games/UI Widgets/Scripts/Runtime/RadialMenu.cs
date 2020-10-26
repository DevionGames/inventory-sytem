using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DevionGames.UIWidgets
{
	public class RadialMenu : UIWidget
	{
		[SerializeField]
		private float m_Radius = 100f;
		[SerializeField]
		private float m_Angle = 360f;
		[Header ("Reference")]
		[SerializeField]
		private MenuItem m_Item = null;

		private List<MenuItem> itemCache = new List<MenuItem> ();


		private void Update ()
		{
			if (m_CanvasGroup.alpha > 0f && (Input.GetMouseButtonUp (0) || Input.GetMouseButtonUp (1) || Input.GetMouseButtonUp (2))) {

				var pointer = new PointerEventData (EventSystem.current);
				pointer.position = Input.mousePosition;
				var raycastResults = new List<RaycastResult> ();
				EventSystem.current.RaycastAll (pointer, raycastResults);
				if (raycastResults.Count > 0) {
					raycastResults [0].gameObject.SendMessage ("Press", SendMessageOptions.DontRequireReceiver);
				}
				Close ();
			}
		}

		public virtual void Show (Sprite[] icons, UnityAction<int> result)
		{
			for (int i = 0; i < itemCache.Count; i++) {
				itemCache [i].gameObject.SetActive (false);
			}
			Show ();
			for (int i = 0; i < icons.Length; i++) {
				int index = i;
				MenuItem item = AddMenuItem (icons [index]);
				float theta = Mathf.Deg2Rad * (m_Angle / icons.Length) * index;
				Vector3 position = new Vector3 (Mathf.Sin (theta), Mathf.Cos (theta), 0);
				item.transform.localPosition = position * m_Radius;

				item.onTrigger.AddListener (delegate() {
					Close ();
					if (result != null) {
						result.Invoke (index);
					}
				});
			}
		}

		public override void Show ()
		{
			m_RectTransform.position = Input.mousePosition;
			base.Show ();
		}

		private MenuItem AddMenuItem (Sprite icon)
		{
			MenuItem item = itemCache.Find (x => !x.isActiveAndEnabled);
			if (item == null) {
				item = Instantiate (m_Item) as MenuItem;
				itemCache.Add (item);
			}
			if (item.targetGraphic != null && item.targetGraphic is Image) {
				(item.targetGraphic as Image).overrideSprite = icon;
			}
			item.onTrigger.RemoveAllListeners ();
			item.gameObject.SetActive (true);
			item.transform.SetParent (m_RectTransform, false);
			return item;
		}
	}
}