using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DevionGames
{
    [CustomEditor(typeof(ActionTemplate))]
    public class ActionTemplateInspector : Editor
    {
        private SerializedProperty m_Script;
        private SerializedProperty m_Actions;
        private SerializedProperty m_Add;

        private void OnEnable()
        {
            this.m_Script = serializedObject.FindProperty("m_Script");
            this.m_Actions = serializedObject.FindProperty("actions");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(this.m_Script);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();
            serializedObject.Update();
            DoGUI(this.m_Actions);
            serializedObject.ApplyModifiedProperties();
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }

        protected void DoGUI(SerializedProperty elements)
        {
            EditorGUIUtility.wideMode = true;
            for (int i = 0; i < elements.arraySize; i++)
            {
                SerializedProperty action = elements.GetArrayElementAtIndex(i);

                object value = action.GetValue();
                EditorGUI.BeginChangeCheck();

                if (EditorTools.Titlebar(value, ElementContextMenu(elements.GetValue() as IList, i)))
                {
                    EditorGUI.indentLevel += 1;
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField("Script", value != null ? EditorTools.FindMonoScript(value.GetType()) : null, typeof(MonoScript), true);
                    EditorGUI.EndDisabledGroup();
                    if (value == null)
                    {
                        EditorGUILayout.HelpBox("Managed reference values can't be removed or replaced. Only way to fix it is to recreate the renamed or deleted script file or delete and recreate the Action. Unity throws an error: Unknown managed type referenced: [Assembly-CSharp] + Type which has been removed.", MessageType.Error);
                    }

                    if (EditorTools.HasCustomPropertyDrawer(value.GetType()))
                    {
                        EditorGUILayout.PropertyField(action, true);
                    }
                    else
                    {
                        foreach (var child in action.EnumerateChildProperties())
                        {
                            EditorGUILayout.PropertyField(
                                child,
                                includeChildren: true
                            );
                        }
                    }
                    EditorGUI.indentLevel -= 1;
                }
            }


            DoAddButton(typeof(Action));
            GUILayout.Space(10f);
        }

        private void Add(Type type)
        {
            object value = System.Activator.CreateInstance(type);
            this.m_Add.serializedObject.Update();
            this.m_Add.arraySize++;
            this.m_Add.GetArrayElementAtIndex(this.m_Add.arraySize - 1).managedReferenceValue = value;
            this.m_Add.serializedObject.ApplyModifiedProperties();
        }


        private void CreateScript(string scriptName)
        {
            Debug.LogWarning("Not implemented yet.");
        }

        private void DoAddButton(Type type)
        {
            GUIStyle buttonStyle = new GUIStyle("AC Button");
            GUIContent buttonContent = new GUIContent("Add Action");
            Rect buttonRect = GUILayoutUtility.GetRect(buttonContent, buttonStyle, GUILayout.ExpandWidth(true));
            buttonRect.x += buttonRect.width * 0.5f - buttonStyle.fixedWidth * 0.5f;
            buttonRect.width = buttonStyle.fixedWidth;
            if (GUI.Button(buttonRect, buttonContent, buttonStyle))
            {
                this.m_Add = this.m_Actions;
                AddObjectWindow.ShowWindow(buttonRect, type, Add, CreateScript);
            }
        }

        private GenericMenu ElementContextMenu(IList list, int index)
        {

            GenericMenu menu = new GenericMenu();
            if (list[index] == null)
            {
                return menu;
            }
            Type elementType = list[index].GetType();
            menu.AddItem(new GUIContent("Reset"), false, delegate {

                object value = System.Activator.CreateInstance(list[index].GetType());
                list[index] = value;
                EditorUtility.SetDirty(target);
            });
            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent("Remove"), false, delegate {
                list.RemoveAt(index);
                EditorUtility.SetDirty(target);
            });

            if (index > 0)
            {
                menu.AddItem(new GUIContent("Move Up"), false, delegate {
                    object value = list[index];
                    list.RemoveAt(index);
                    list.Insert(index - 1, value);
                    EditorUtility.SetDirty(target);
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Move Up"));
            }

            if (index < list.Count - 1)
            {
                menu.AddItem(new GUIContent("Move Down"), false, delegate
                {
                    object value = list[index];
                    list.RemoveAt(index);
                    list.Insert(index + 1, value);
                    EditorUtility.SetDirty(target);
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Move Down"));
            }

            if (list[index] != null)
            {
                MonoScript script = EditorTools.FindMonoScript(list[index].GetType());
                if (script != null)
                {
                    menu.AddSeparator(string.Empty);
                    menu.AddItem(new GUIContent("Edit Script"), false, delegate { AssetDatabase.OpenAsset(script); });
                }
            }
            return menu;
        }
    }
}