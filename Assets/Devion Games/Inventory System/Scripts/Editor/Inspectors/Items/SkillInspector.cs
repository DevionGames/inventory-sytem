using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [CustomEditor(typeof(Skill),true)]
    public class SkillInspector : UsableItemInspector
    {
        protected SerializedProperty m_FixedSuccessChance;
        protected SerializedProperty m_GainModifier;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (target == null) return;

            this.m_FixedSuccessChance = serializedObject.FindProperty("m_FixedSuccessChance");
            this.m_GainModifier = serializedObject.FindProperty("m_GainModifier");

        }

        public override void OnInspectorGUI()
        {
            ScriptGUI();
            serializedObject.Update();

            EditorGUILayout.PropertyField(this.m_ItemName, new GUIContent("Name"));
            EditorGUILayout.PropertyField(this.m_UseItemNameAsDisplayName, new GUIContent("Use name as display name"));
            this.m_ShowItemDisplayNameOptions.target = !this.m_UseItemNameAsDisplayName.boolValue;
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowItemDisplayNameOptions.faded))
            {
                EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
                EditorGUILayout.PropertyField(this.m_ItemDisplayName);
                EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.PropertyField(this.m_Icon);
            EditorGUILayout.PropertyField(this.m_Description);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.m_Category);
            EditorGUILayout.PropertyField(m_FixedSuccessChance);
            EditorGUILayout.PropertyField(this.m_GainModifier);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(this.m_BuyPrice, new GUIContent("Price"));
            EditorGUILayout.PropertyField(this.m_BuyCurrency, GUIContent.none);
            EditorGUILayout.EndHorizontal();
            DrawCooldownGUI();
            ActionGUI();
            serializedObject.ApplyModifiedProperties();
        }
    }
}