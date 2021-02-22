using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DevionGames.StatSystem
{
	public class UIStat : MonoBehaviour
	{
		[Header("Stat Definition")]
		[SerializeField]
		protected string m_StatsHandler = "Player Stats";
		[StatPicker]
		[SerializeField]
		protected Stat m_Stat;
		[StatPicker]
		[SerializeField]
		protected Stat m_FreePoints;
		[Header("UI References")]
		[SerializeField]
		protected Text m_StatName;
		[SerializeField]
		protected Image m_StatBar;
		[SerializeField]
		protected Image m_StatBarFade;
		[SerializeField]
		protected Text m_CurrentValue;
		[SerializeField]
		protected Text m_Value;
		[SerializeField]
		protected Button m_IncrementButton;

		private Stat stat;
		private Stat freePoints;

		protected virtual void Start() {
			if (this.m_IncrementButton != null)
			{
				this.m_IncrementButton.onClick.AddListener(delegate () {
					stat.Add(1f);
					freePoints.Subtract(1f);
				});
			}
		}

		protected virtual void Update() {
			//TODO BETTER FIX, SelectableUIStat is displaying same values for all enemies
			//if (stat == null) {
				StatsHandler handler = GetStatsHandler();
				if (handler == null)
					return;
				stat = handler.GetStat(this.m_Stat);
				if(this.m_FreePoints != null)
					freePoints = handler.GetStat(this.m_FreePoints);

				if (this.m_StatName != null)
					this.m_StatName.text = this.stat.Name;
		//	}
			Repaint();
		}

		protected virtual StatsHandler GetStatsHandler() { 
			return StatsManager.GetStatsHandler(this.m_StatsHandler); 
		}

		protected virtual void Repaint() {
			if (stat is Attribute attribute)
			{
				float normalized = attribute.CurrentValue / attribute.Value;

				if (this.m_StatBar != null)
				{
					this.m_StatBar.fillAmount = normalized;
				}

				if (this.m_StatBarFade != null)
				{
					this.m_StatBarFade.fillAmount = Mathf.MoveTowards(this.m_StatBarFade.fillAmount, normalized, Time.deltaTime * 0.5f);
				}

				if (this.m_CurrentValue != null)
				{
					this.m_CurrentValue.text = attribute.CurrentValue.ToString();
				}
			}

			if (this.m_Value != null)
			{

				this.m_Value.text = stat.Value.ToString();
			}
			
			if (this.m_IncrementButton != null && freePoints != null)
			{
				this.m_IncrementButton.gameObject.SetActive(freePoints.Value > 0?true:false);
			}
		}

		private void OnCharacterLoaded(CallbackEventData data)
		{
			if (GetComponentInParent(data.GetData("Slot").GetType()) != (Component)data.GetData("Slot"))
			{
				return;
			}
			
			string key = data.GetData("CharacterName") + ".Stats." + this.m_StatsHandler + "." + this.m_Stat.Name;
			if (PlayerPrefs.HasKey(key + ".Value"))
			{
				float value = PlayerPrefs.GetFloat(key + ".Value");
				if (this.m_Value != null)
					this.m_Value.text = value.ToString();
			}
			if (PlayerPrefs.HasKey(key + ".CurrentValue"))
			{
				float currentValue = PlayerPrefs.GetFloat(key + ".CurrentValue");
				if (this.m_CurrentValue != null)
					this.m_CurrentValue.text = currentValue.ToString();
			}
		}
	}
}