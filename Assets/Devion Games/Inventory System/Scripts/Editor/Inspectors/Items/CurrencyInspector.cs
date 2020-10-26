using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [CustomEditor(typeof(Currency))]
    public class CurrencyInspector : ItemInspector
    {
        private SerializedProperty m_CurrencyConversions;

        private ReorderableList m_CurrencyConversionList;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (target == null) {
                return;
            }
            this.m_CurrencyConversions = serializedObject.FindProperty("currencyConversions");
            this.m_CurrencyConversionList = new ReorderableList(serializedObject, this.m_CurrencyConversions, true, true, true, true);
            this.m_CurrencyConversionList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Conversions");
            };
               
            this.m_CurrencyConversionList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = this.m_CurrencyConversions.GetArrayElementAtIndex(index);
                float width = rect.width;
                rect.y += 2f;
                rect.height = 17f;
                Vector2 size=EditorStyles.label.CalcSize(new GUIContent("1 " + this.m_ItemName.stringValue + " to"));
                rect.width = size.x+5f;
                GUI.Label(rect, "1 " + this.m_ItemName.stringValue + " to");
                rect.x += rect.width+2f;
                rect.width = 75f;
                EditorGUI.PropertyField(rect,element.FindPropertyRelative("factor"),GUIContent.none);
                rect.x += rect.width + 2f;
                rect.width = width - size.x + 5f - 75f - 12f;
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("currency"), GUIContent.none);
            };

        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(this.m_Script);
            EditorGUI.EndDisabledGroup();

            serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_ItemName,new GUIContent("Name"));
            EditorGUILayout.PropertyField(this.m_Icon);
            GUILayout.Space(3);
            this.m_CurrencyConversionList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}