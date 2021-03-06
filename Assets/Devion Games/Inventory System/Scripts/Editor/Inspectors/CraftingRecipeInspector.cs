using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [CustomEditor(typeof(CraftingRecipe))]
    public class CraftingRecipeInspector : Editor
    {
        private SerializedProperty m_Script;
        private SerializedProperty m_Name;
        private SerializedProperty m_Duration;
        private SerializedProperty m_AnimatorState;
        private SerializedProperty m_Skill;
        private SerializedProperty m_RemoveIngredientsWhenFailed;
        private SerializedProperty m_Ingredients;
        private ReorderableList m_IngredientList;
        private SerializedProperty m_CraftingModifier;
        private ReorderableList m_CraftingModifierList;

        private SerializedProperty m_Conditions;
        private UnityEngine.Object m_Target;
        private IList m_List;

        private Type m_ElementType;
        private static object m_ObjectToCopy;

        private void OnEnable()
        {
            if (target == null) return;
            this.m_Script = serializedObject.FindProperty("m_Script");
            this.m_Name = serializedObject.FindProperty("name");
            this.m_Duration = serializedObject.FindProperty("m_Duration");
            this.m_AnimatorState = serializedObject.FindProperty("m_AnimatorState");
            this.m_Skill = serializedObject.FindProperty("m_Skill");

            this.m_RemoveIngredientsWhenFailed = serializedObject.FindProperty("m_RemoveIngredientsWhenFailed");
            this.m_Ingredients = serializedObject.FindProperty("m_Ingredients");
            this.m_IngredientList = CreateItemAmountList("Ingredients (Item, Amount)", serializedObject, this.m_Ingredients);
            this.m_CraftingModifier = serializedObject.FindProperty("m_CraftingModifier");
            CreateModifierList("Crafting Item Modifers", serializedObject, this.m_CraftingModifier);
            this.m_Conditions = serializedObject.FindProperty("conditions");
            this.m_Target = target;
            this.m_List = (target as CraftingRecipe).conditions;
            this.m_ElementType = Utility.GetElementType(this.m_List.GetType());
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(this.m_Script);
            EditorGUI.EndDisabledGroup();

            serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_Name);
            EditorGUILayout.PropertyField(this.m_Duration);
            EditorGUILayout.PropertyField(this.m_AnimatorState);
            EditorGUILayout.PropertyField(this.m_Skill);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.m_RemoveIngredientsWhenFailed);
            this.m_IngredientList.DoLayoutList();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Crafting item modifiers can be used to randomize the item when crafting.", MessageType.Info);
            this.m_CraftingModifierList.DoLayoutList();
            ConditionGUI();
            serializedObject.ApplyModifiedProperties();
        }

        private void CreateModifierList(string title, SerializedObject serializedObject, SerializedProperty property)
        {
            this.m_CraftingModifierList = new ReorderableList(serializedObject, property.FindPropertyRelative("modifiers"), true, true, true, true);
            this.m_CraftingModifierList.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, title);
            };
            this.m_CraftingModifierList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                float verticalOffset = (rect.height - EditorGUIUtility.singleLineHeight) * 0.5f;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y = rect.y + verticalOffset;
                SerializedProperty element = this.m_CraftingModifierList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element, GUIContent.none, true);
            };

            this.m_CraftingModifierList.onRemoveCallback = (ReorderableList list) =>
            {
                list.serializedProperty.GetArrayElementAtIndex(list.index).objectReferenceValue = null;
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            };
        }

        private ReorderableList CreateItemAmountList(string title, SerializedObject serializedObject, SerializedProperty property)
        {
            ReorderableList list = new ReorderableList(serializedObject, property, true, true, true, true);
            list.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, title);
            };
            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                SerializedProperty itemProperty = element.FindPropertyRelative("item");
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.width -= 55f;
                EditorGUI.PropertyField(rect, itemProperty, GUIContent.none);
                rect.x += rect.width + 5;
                rect.width = 50f;
                SerializedProperty amount = element.FindPropertyRelative("amount");
                EditorGUI.PropertyField(rect, amount, GUIContent.none);
                amount.intValue = Mathf.Clamp(amount.intValue, 1, int.MaxValue);
            };
            return list;
        }

        protected void ConditionGUI()
        {

            GUILayout.Space(10f);
            for (int i = 0; i < this.m_Conditions.arraySize; i++)
            {
                SerializedProperty condition = this.m_Conditions.GetArrayElementAtIndex(i);

                object value = this.m_List[i];
                EditorGUI.BeginChangeCheck();
                if (this.m_Target != null)
                    Undo.RecordObject(this.m_Target, "Crafting Recipe Condition");

                if (EditorTools.Titlebar(value, ElementContextMenu(this.m_List, i)))
                {
                    EditorGUI.indentLevel += 1;
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField("Script", value != null ? EditorTools.FindMonoScript(value.GetType()) : null, typeof(MonoScript), true);
                    EditorGUI.EndDisabledGroup();
                    if (value == null)
                    {
                        EditorGUILayout.HelpBox("Managed reference values can't be removed or replaced. Only way to fix it is to recreate the renamed or deleted script file or delete and recreate the item and rereference it in scenes. Unity throws an error: Unknown managed type referenced: [Assembly-CSharp] + Type which has been removed.", MessageType.Error);
                    }
                    if (EditorTools.HasCustomPropertyDrawer(value.GetType()))
                    {
                        EditorGUILayout.PropertyField(condition, true);
                    }
                    else
                    {
                        foreach (var child in condition.EnumerateChildProperties())
                        {
                            EditorGUILayout.PropertyField(
                                child,
                                includeChildren: true
                            );
                        }
                    }
                    EditorGUI.indentLevel -= 1;
                }
                if (EditorGUI.EndChangeCheck())
                    EditorUtility.SetDirty(this.m_Target);
            }
            GUILayout.FlexibleSpace();
            DoAddButton();
            GUILayout.Space(10f);

        }

        private void Add(Type type)
        {

            object value = System.Activator.CreateInstance(type);
            serializedObject.Update();
            this.m_Conditions.arraySize++;
            this.m_Conditions.GetArrayElementAtIndex(this.m_Conditions.arraySize - 1).managedReferenceValue = value;
            serializedObject.ApplyModifiedProperties();
        }

        private void CreateScript(string scriptName)
        {

        }

        private void DoAddButton()
        {
            GUIStyle buttonStyle = new GUIStyle("AC Button");
            GUIContent buttonContent = new GUIContent("Add Condition");
            Rect buttonRect = GUILayoutUtility.GetRect(buttonContent, buttonStyle, GUILayout.ExpandWidth(true));
            buttonRect.x = buttonRect.width * 0.5f - buttonStyle.fixedWidth * 0.5f;
            buttonRect.width = buttonStyle.fixedWidth;
            if (GUI.Button(buttonRect, buttonContent, buttonStyle))
            {
                AddObjectWindow.ShowWindow(buttonRect, this.m_ElementType, Add, CreateScript);
            }
        }

        private GenericMenu ElementContextMenu(IList list, int index)
        {

            GenericMenu menu = new GenericMenu();
            if (list[index] == null)
            {
                return menu;
            }
            menu.AddItem(new GUIContent("Reset"), false, delegate {

                object value = System.Activator.CreateInstance(list[index].GetType());
                list[index] = value;
                EditorUtility.SetDirty(target);
            });
            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent("Remove " + this.m_ElementType.Name), false, delegate { list.RemoveAt(index); EditorUtility.SetDirty(target); });

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

            menu.AddItem(new GUIContent("Copy " + this.m_ElementType.Name), false, delegate {
                object value = list[index];
                m_ObjectToCopy = value;
            });

            if (m_ObjectToCopy != null)
            {
                menu.AddItem(new GUIContent("Paste " + this.m_ElementType.Name + " As New"), false, delegate {
                    object instance = System.Activator.CreateInstance(m_ObjectToCopy.GetType());
                    FieldInfo[] fields = instance.GetType().GetSerializedFields();
                    for (int i = 0; i < fields.Length; i++)
                    {
                        object value = fields[i].GetValue(m_ObjectToCopy);
                        fields[i].SetValue(instance, value);
                    }
                    list.Insert(index + 1, instance);
                    EditorUtility.SetDirty(target);
                });

                if (list[index].GetType() == m_ObjectToCopy.GetType())
                {
                    menu.AddItem(new GUIContent("Paste " + this.m_ElementType.Name + " Values"), false, delegate
                    {
                        object instance = list[index];
                        FieldInfo[] fields = instance.GetType().GetSerializedFields();
                        for (int i = 0; i < fields.Length; i++)
                        {
                            object value = fields[i].GetValue(m_ObjectToCopy);
                            fields[i].SetValue(instance, value);
                        }
                        EditorUtility.SetDirty(target);
                    });
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Paste " + this.m_ElementType.Name + " Values"));
                }
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