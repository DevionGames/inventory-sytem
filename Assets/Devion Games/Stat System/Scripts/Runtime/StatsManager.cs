using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System.Linq;
using DevionGames.StatSystem.Configuration;

namespace DevionGames.StatSystem
{
    public class StatsManager : MonoBehaviour
    {
        private static StatsManager m_Current;

        /// <summary>
        /// The StatManager singleton object. This object is set inside Awake()
        /// </summary>
        public static StatsManager current
        {
            get
            {
                Assert.IsNotNull(m_Current, "Requires a Stats Manager.Create one from Tools > Devion Games > Stat System > Create Stat Manager!");
                return m_Current;
            }
        }

        [SerializeField]
        private StatConfigurations m_Configurations = null;

        /// <summary>
        /// Gets the item database. Configurate it inside the editor.
        /// </summary>
        /// <value>The database.</value>
        public static StatConfigurations Configurations
        {
            get
            {
                if (StatsManager.current != null)
                {
                    Assert.IsNotNull(StatsManager.current.m_Configurations, "Please assign StatConfigurations to the Stats Manager!");
                    return StatsManager.current.m_Configurations;
                }
                return null;
            }
        }

        private static Default m_DefaultSettings;
        public static Default DefaultSettings
        {
            get
            {
                if (m_DefaultSettings == null)
                {
                    m_DefaultSettings = GetSetting<Default>();
                }
                return m_DefaultSettings;
            }
        }

        private static UI m_UI;
        public static UI UI
        {
            get
            {
                if (m_UI == null)
                {
                    m_UI = GetSetting<UI>();
                }
                return m_UI;
            }
        }

        private static Notifications m_Notifications;
        public static Notifications Notifications
        {
            get
            {
                if (m_Notifications == null)
                {
                    m_Notifications = GetSetting<Notifications>();
                }
                return m_Notifications;
            }
        }

        private static SavingLoading m_SavingLoading;
        public static SavingLoading SavingLoading
        {
            get
            {
                if (m_SavingLoading == null)
                {
                    m_SavingLoading = GetSetting<SavingLoading>();
                }
                return m_SavingLoading;
            }
        }

        private static T GetSetting<T>() where T : Configuration.Settings
        {
            if (StatsManager.Configurations != null)
            {
                return (T)StatsManager.Configurations.settings.Where(x => x.GetType() == typeof(T)).FirstOrDefault();
            }
            return default(T);
        }

        private List<StatsHandler> m_StatsHandler;

        /// Don't destroy this object instance when loading new scenes.
        /// </summary>
        public bool dontDestroyOnLoad = true;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            if (StatsManager.m_Current != null)
            {
                // Debug.Log("Multiple Stat Manager in scene...this is not supported. Destroying instance!");
                Destroy(gameObject);
                return;
            }
            else
            {
                StatsManager.m_Current = this;
                if (dontDestroyOnLoad)
                {
                    if (transform.parent != null)
                    {
                        if (StatsManager.DefaultSettings.debugMessages)
                            Debug.Log("Stats Manager with DontDestroyOnLoad can't be a child transform. Unparent!");
                        transform.parent = null;
                    }
                    DontDestroyOnLoad(gameObject);
                }
                this.m_StatsHandler = new List<StatsHandler>();
                if (StatsManager.SavingLoading.autoSave)
                {
                    StartCoroutine(RepeatSaving(StatsManager.SavingLoading.savingRate));
                }
                if (StatsManager.DefaultSettings.debugMessages)
                    Debug.Log("Stats Manager initialized.");
            }
        }

        private void Start()
        {
            if (StatsManager.SavingLoading.autoSave)
            {
                StartCoroutine(DelayedLoading(1f));
            }
        }

        public static StatsHandler GetStatsHandler(string name) {
            return StatsManager.current.m_StatsHandler.Find(x=>x.HandlerName == name);
        }


        public static void RegisterStatsHandler(StatsHandler handler) {
            if (!StatsManager.current.m_StatsHandler.Contains(handler)) {
                StatsManager.current.m_StatsHandler.Add(handler);
            }
        }

        public static void Save()
        {
            string key = PlayerPrefs.GetString(StatsManager.SavingLoading.savingKey) + ".StatSystem";
            StatsHandler[] results = Object.FindObjectsOfType<StatsHandler>().Where(x=>x.saveable).ToArray();
            if (results.Length > 0)
            {
                string data = JsonSerializer.Serialize(results);
                foreach (StatsHandler handler in results) {
                    foreach (Stat stat in handler.stats) {
                        PlayerPrefs.SetFloat(key+"."+handler.HandlerName+"."+stat.Name+".Value",stat.Value);
                        PlayerPrefs.SetFloat(key + "." + handler.HandlerName + "." + stat.Name + ".CurrentValue", stat.CurrentValue);
                    }
                }

                PlayerPrefs.SetString(key, data);
                if (StatsManager.DefaultSettings.debugMessages)
                    Debug.Log("[Stat System] Stats saved " + key + data);
            }
        }

        public static void Load() {
            string key = PlayerPrefs.GetString(StatsManager.SavingLoading.savingKey) + ".StatSystem";
            string data = PlayerPrefs.GetString(key);
            if (string.IsNullOrEmpty(data)) { return; }

            List<StatsHandler> results = Object.FindObjectsOfType<StatsHandler>().Where(x => x.saveable).ToList();
            List<object> list = MiniJSON.Deserialize(data) as List<object>;
            for (int i = 0; i < list.Count; i++)
            {
                Dictionary<string, object> handlerData = list[i] as Dictionary<string, object>;
                string handlerName = (string)handlerData["Name"];
                StatsHandler handler = results.Find(x => x.HandlerName == handlerName);
                if (handler != null)
                {
                    handler.SetObjectData(handlerData);
                    handler.UpdateStats();
                }
            }
            
            if (StatsManager.DefaultSettings.debugMessages)
                Debug.Log("[Stat System] Stats loaded " + StatsManager.SavingLoading.savingKey + ".StatSystem" + data);
        }



        private IEnumerator DelayedLoading(float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
            Load();
        }

        private IEnumerator RepeatSaving(float seconds)
        {
            while (true)
            {
                yield return new WaitForSeconds(seconds);
                Save();
            }
        }

    }
}