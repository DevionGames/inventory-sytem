using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace DevionGames.StatSystem{
	public class UIStat : MonoBehaviour {
		[Header("Stat Definition")]
		[SerializeField]
		protected string m_StatsHandler = "Player Stats";
		[SerializeField]
		protected string m_StatName=string.Empty;
		[Header("UI References")]
		[SerializeField]
		protected Text statName;
		[SerializeField]
		protected Image statBar;
		[SerializeField]
		protected Image statBarFade;
		[SerializeField]
		protected Text currentValue;
		[SerializeField]
		protected Text value;
		[SerializeField]
		protected Button m_IncrementButton;

		protected StatsHandler m_Handler;
		protected virtual StatsHandler handler {
			get {
				if (this.m_Handler == null)
					this.m_Handler = StatsManager.GetStatsHandler(this.m_StatsHandler);
				return this.m_Handler;
			}
		}

        protected Stat m_Stat;
		protected virtual Stat stat
        {
			get {
				if (this.m_Stat == null && handler != null)
					this.m_Stat = handler.GetStat(this.m_StatName);
				return this.m_Stat;
			}
        }

        private void Awake()
        {
			if (this.m_IncrementButton != null)
			{
				this.m_IncrementButton.onClick.AddListener(delegate () {
					stat.IncrementalValue += 1;
					handler.freeStatPoints -= 1;
					handler.UpdateStats();
				});
			}
		}

		protected virtual void Update(){
			if (stat == null) { return; }

			if(statBar != null ){
				statBar.fillAmount = stat.NormalizedValue;
			}

			if (statBarFade != null) {
				statBarFade.fillAmount = Mathf.MoveTowards(statBarFade.fillAmount, stat.NormalizedValue, Time.deltaTime*0.5f);
			}

			if (currentValue != null)
			{
				currentValue.text =stat.CurrentValue.ToString();
			}
			if (value != null)
			{
				value.text = stat.Value.ToString();
			}
			if (statName != null)
			{
				statName.text = stat.Name;
			}

			if (this.m_IncrementButton != null)
			{
				if (handler.freeStatPoints > 0)
				{
					this.m_IncrementButton.gameObject.SetActive(true);
				}
				else
				{
					this.m_IncrementButton.gameObject.SetActive(false);
				}
			}
		}

		private void OnCharacterLoaded(CallbackEventData data) {
			if (GetComponentInParent(data.GetData("Slot").GetType()) != (Component)data.GetData("Slot")){
				return;
			}
			//StatRenderer renderer = GetComponentInParent<StatRenderer>();
			//if (renderer != null)
			//{
				string key = data.GetData("CharacterName")+ ".StatSystem." + this.m_StatsHandler + "." + this.m_StatName;
				if (PlayerPrefs.HasKey(key + ".Value"))
				{
					float value = PlayerPrefs.GetFloat(key + ".Value");
					if (this.value != null)
						this.value.text = value.ToString();
				}
				if (PlayerPrefs.HasKey(key + ".CurrentValue"))
				{
					float currentValue = PlayerPrefs.GetFloat(key + ".CurrentValue");
					if (this.currentValue != null)
						this.currentValue.text = currentValue.ToString();
				}
			//}
		}
	}
}