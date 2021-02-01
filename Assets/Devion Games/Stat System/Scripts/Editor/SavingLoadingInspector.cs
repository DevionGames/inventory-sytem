using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace DevionGames.StatSystem.Configuration
{
    [CustomEditor(typeof(SavingLoading))]
    public class SavingLoadingInspector : Editor
    {
        private SerializedProperty m_Script;
        private SerializedProperty m_AutoSave;
        private AnimBool m_ShowSave;
        private SerializedProperty m_Provider;
        private AnimBool m_ShowMySQL;

        private SerializedProperty m_SavingKey;
        private SerializedProperty m_SavingRate;
        private SerializedProperty m_ServerAdress;
        private SerializedProperty m_SaveScript;
        private SerializedProperty m_LoadScript;

        protected virtual void OnEnable()
        {
            this.m_Script = serializedObject.FindProperty("m_Script");
            this.m_AutoSave = serializedObject.FindProperty("autoSave");
            this.m_ShowSave = new AnimBool(this.m_AutoSave.boolValue);
            this.m_ShowSave.valueChanged.AddListener(new UnityAction(Repaint));

            this.m_Provider = serializedObject.FindProperty("provider");
            this.m_ShowMySQL = new AnimBool(this.m_Provider.enumValueIndex == 1);
            this.m_ShowMySQL.valueChanged.AddListener(new UnityAction(Repaint));


            this.m_SavingKey = serializedObject.FindProperty("savingKey");
            this.m_SavingRate = serializedObject.FindProperty("savingRate");
            this.m_ServerAdress = serializedObject.FindProperty("serverAdress");
            this.m_SaveScript = serializedObject.FindProperty("saveScript");
            this.m_LoadScript = serializedObject.FindProperty("loadScript");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(this.m_Script);
            EditorGUI.EndDisabledGroup();

            serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_AutoSave);
            this.m_ShowSave.target = this.m_AutoSave.boolValue;
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowSave.faded))
            {
                EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
                EditorGUILayout.PropertyField(this.m_SavingKey);
                EditorGUILayout.PropertyField(this.m_SavingRate);

                EditorGUILayout.PropertyField(m_Provider);
                this.m_ShowMySQL.target = m_Provider.enumValueIndex == 1;
                if (EditorGUILayout.BeginFadeGroup(this.m_ShowMySQL.faded))
                {
                    EditorGUILayout.PropertyField(this.m_ServerAdress);
                    EditorGUILayout.PropertyField(this.m_SaveScript);
                    EditorGUILayout.PropertyField(this.m_LoadScript);
                }
                EditorGUILayout.EndFadeGroup();

                EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.Space(2f);
            EditorTools.Seperator();
            SavedDataGUI();
            serializedObject.ApplyModifiedProperties();
        }

        private void SavedDataGUI() {
            List<string> keys = PlayerPrefs.GetString("StatSystemSavedKeys").Split(';').ToList();
            keys.RemoveAll(x => string.IsNullOrEmpty(x));

            if (EditorTools.Foldout("StatSystemSavedData", new GUIContent("Saved Data " + keys.Count)))
            {
                EditorTools.BeginIndent(1, true);
                if (keys.Count == 0)
                {
                    GUILayout.Label("No data saved on this device!");
                }


                for (int i = 0; i < keys.Count; i++)
                {
                    string key = keys[i];
                    GenericMenu keyMenu = new GenericMenu();

                    keyMenu.AddItem(new GUIContent("Delete Key"), false, () => {
                        List<string> allKeys = new List<string>(keys);
                        allKeys.Remove(key);
                        PlayerPrefs.SetString("StatSystemSavedKeys", string.Join(";", allKeys));
                        PlayerPrefs.DeleteKey(key+".Stats");
                    });

                    if (EditorTools.Foldout(key, new GUIContent(key), keyMenu))
                    {
                        EditorTools.BeginIndent(1, true);
                        string data = PlayerPrefs.GetString(key+".Stats");
                        GUILayout.Label(data, EditorStyles.wordWrappedLabel);
                        EditorTools.EndIndent();
                    }
                }
                EditorTools.EndIndent();
            }
        }
    }
}