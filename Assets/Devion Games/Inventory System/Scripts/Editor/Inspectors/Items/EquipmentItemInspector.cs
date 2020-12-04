using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace DevionGames.InventorySystem
{
    [CustomEditor(typeof(EquipmentItem), true)]
    public class EquipmentItemInspector :  UsableItemInspector
    {
        private ReorderableList regionList;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (target == null) return;
            this.regionList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_Region"), true, true, true, true);
            this.regionList.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "Equipment Region");
            };
            this.regionList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                SerializedProperty element = regionList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, element, GUIContent.none, true);
            };
        }


        private void DrawInspector() {
            GUILayout.Space(3f);
            GUILayout.Label("Equipment:", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("The equipment region defines where the item should be equiped to. Use Left Hand and Right Hand for two-handed weapons.", MessageType.Info);
            regionList.DoLayoutList();
        }

    }
}