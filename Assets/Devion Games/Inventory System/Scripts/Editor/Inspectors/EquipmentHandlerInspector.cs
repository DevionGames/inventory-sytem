using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR;

namespace DevionGames.InventorySystem
{
    [CustomEditor(typeof(EquipmentHandler), true)]
    public class EquipmentHandlerInspector : Editor
    {
        private SerializedProperty m_Script;
        private SerializedProperty m_WindowName;
        private GameObject gameObject;
        private SerializedProperty m_Database;

        private void OnEnable()
        {
            this.m_Script = serializedObject.FindProperty("m_Script");
            this.m_WindowName = serializedObject.FindProperty("m_WindowName");
            this.m_Database = serializedObject.FindProperty("m_Database");
            EquipmentHandler handler = target as EquipmentHandler;
            gameObject = handler.gameObject;

            VisibleItem[] visibleItems = gameObject.GetComponents<VisibleItem>();
            for (int i = 0; i < visibleItems.Length; i++) {
                visibleItems[i].hideFlags = EditorPrefs.GetBool("InventorySystem.showAllComponents") ? HideFlags.None : HideFlags.HideInInspector;
            }

            if (!EditorApplication.isPlaying)
            {
                for (int i = 0; i < handler.VisibleItems.Count; i++)
                {
                    if (handler.VisibleItems[i].gameObject != gameObject)
                    {
                        if (ComponentUtility.CopyComponent(handler.VisibleItems[i]))
                        {
                            VisibleItem compoent = gameObject.AddComponent(handler.VisibleItems[i].GetType()) as VisibleItem;
                            ComponentUtility.PasteComponentValues(compoent);
                            handler.VisibleItems[i] = compoent;
                        }
                    }
                }
            }
            EditorApplication.playModeStateChanged += PlayModeState;
        }


        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= PlayModeState;
        }

        bool playModeStateChange;
        private void PlayModeState(PlayModeStateChange state)
        {
            playModeStateChange = (state == PlayModeStateChange.ExitingEditMode || state == PlayModeStateChange.EnteredEditMode || state == PlayModeStateChange.ExitingPlayMode);
        }

        private void OnDestroy()
        {
            if (gameObject != null && target == null && !playModeStateChange)
            {
                VisibleItem[] visibleItems = gameObject.GetComponents<VisibleItem>();
                for (int i = 0; i < visibleItems.Length; i++)
                {
                    DestroyImmediate(visibleItems[i]);
                }
                Debug.LogWarning("EquipmentHandler component removed, cleaning up visual items.");
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(this.m_Script);
            EditorGUI.EndDisabledGroup();
            serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_WindowName);
            serializedObject.ApplyModifiedProperties();

            if (EditorTools.RightArrowButton(new GUIContent("Bones"), GUILayout.Height(24f)))
            {
                ShowBoneMap();
            }

            if (EditorTools.RightArrowButton(new GUIContent("Items"), GUILayout.Height(24f)))
            {
                VisibleItemsEditor.ShowWindow("Items", serializedObject.FindProperty("m_VisibleItems"));
            }

            if (EditorWindow.mouseOverWindow != null)
            {
                EditorWindow.mouseOverWindow.Repaint();
            }
        }

        private void ShowBoneMap()
        {
            UtilityInstanceWindow.ShowWindow("Bones", delegate ()
            {

                SelectDatabaseButton();
                GUILayout.Space(3f);  
                ItemDatabase database = this.m_Database.objectReferenceValue as ItemDatabase;
                if (database == null)
                    return;

                List<EquipmentHandler.EquipmentBone> bones = (target as EquipmentHandler).Bones;
                var firstNotSecond = database.equipments.Except(bones.Select(x => x.region)).ToList();
                var secondNotFirst = bones.Select(x => x.region).Except(database.equipments).ToList();

                for (int i = 0; i < firstNotSecond.Count; i++)
                {
                    InsertEquipmentBone(firstNotSecond[i]);
                }

                for (int i = 0; i < secondNotFirst.Count; i++)
                {
                    bones.RemoveAll(x => x.region == secondNotFirst[i]);
                }

                SerializedProperty property = serializedObject.FindProperty("m_Bones");
                serializedObject.Update();
                EditorGUI.BeginChangeCheck();
                for (int i = 0; i < property.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    SerializedProperty element = property.GetArrayElementAtIndex(i);
                    EditorGUILayout.LabelField((element.FindPropertyRelative("region").objectReferenceValue as EquipmentRegion).Name, GUILayout.Width(120f));
                    EditorGUILayout.PropertyField(element.FindPropertyRelative("bone"), GUIContent.none);
                    EditorGUILayout.EndHorizontal();
                }
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(target);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                }
            });
        }


        private void SelectDatabaseButton()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Database", GUILayout.Width(120f));
            ItemDatabase database = this.m_Database.objectReferenceValue as ItemDatabase;
            GUIStyle buttonStyle = EditorStyles.objectField;
            GUIContent buttonContent = new GUIContent(database != null ? database.name : "Null");
            Rect buttonRect = GUILayoutUtility.GetRect(buttonContent,buttonStyle);
            //buttonRect = EditorGUI.PrefixLabel(buttonRect, new GUIContent("Database"));
            if (GUI.Button(buttonRect, buttonContent, buttonStyle))
            {
                ObjectPickerWindow.ShowWindow(buttonRect, typeof(ItemDatabase),
                    (UnityEngine.Object obj) => {
                        serializedObject.Update();
                        this.m_Database.objectReferenceValue = obj;
                        serializedObject.ApplyModifiedProperties();
                    },
                    () => {
                      
                    });
            }
            EditorGUILayout.EndHorizontal();
        }

        private SerializedProperty InsertEquipmentBone(EquipmentRegion region) {
            SerializedProperty bones = serializedObject.FindProperty("m_Bones");
            serializedObject.Update();
            bones.InsertArrayElementAtIndex(bones.arraySize);
            SerializedProperty current = bones.GetArrayElementAtIndex(bones.arraySize - 1);
            current.FindPropertyRelative("region").objectReferenceValue = region;
            current.FindPropertyRelative("bone").objectReferenceValue=null;
            serializedObject.ApplyModifiedProperties();
            return current;
        }
    }
}