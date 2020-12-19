using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [CustomPropertyDrawer(typeof(HasItem))]
    public class HasItemDrawer : PropertyDrawer
    {
        private SerializedProperty m_RequiredItems;
        private Dictionary<string, ReorderableList> m_ListMap = new Dictionary<string, ReorderableList>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            ReorderableList m_RequiredItemList = null;
            if (!this.m_ListMap.TryGetValue(property.propertyPath, out m_RequiredItemList))
            {
          
                m_RequiredItems = property.FindPropertyRelative("requiredItems");
                m_RequiredItemList = new ReorderableList(property.serializedObject, this.m_RequiredItems, true, true, true, true);
                m_RequiredItemList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = m_RequiredItemList.serializedProperty.GetArrayElementAtIndex(index);
                    SerializedProperty itemProperty = element.FindPropertyRelative("item");
                    rect.y += 2;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    rect.width *= 0.5f;
                    EditorGUI.PropertyField(rect, itemProperty, GUIContent.none);
                    rect.x += rect.width + 5;
                    rect.width -= 5f;
                    SerializedProperty window = element.FindPropertyRelative("stringValue");
                    /*if (InventorySystemEditor.Database == null || InventorySystemEditor.Database.items.Count == 0)
                    {
                        rect.y += (9 + EditorGUIUtility.singleLineHeight + 6);
                    }*/
                    EditorGUI.PropertyField(rect, window, GUIContent.none);
                };
                m_RequiredItemList.drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Required Items(Item, Window)");
                };
                this.m_ListMap.Add(property.propertyPath, m_RequiredItemList);
            }
          //  m_RequiredItemList.elementHeight = (InventorySystemEditor.Database != null && InventorySystemEditor.Database.items.Count > 0 || m_RequiredItemList.count == 0 ? 21 : (30 + EditorGUIUtility.singleLineHeight + 4));
            try
            {
                m_RequiredItemList.DoLayoutList();
            }catch  {
                if (Event.current.type == EventType.Repaint)
                    this.m_ListMap.Remove(property.propertyPath);
            }
            EditorGUILayout.Space();

        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 5f;
        }
    }
}