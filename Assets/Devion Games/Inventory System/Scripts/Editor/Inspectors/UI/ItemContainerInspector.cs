
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using DevionGames.UIWidgets;
using UnityEditor.AnimatedValues;
using UnityEngine.Events;
using UnityEngine.UI;


namespace DevionGames.InventorySystem
{
   [CustomEditor(typeof(ItemContainer),true)]
    public class ItemContainerInspector : UIWidgetInspector
    {

        private SerializedProperty m_UseButton;
        private SerializedProperty m_DynamicContainer;
        private SerializedProperty m_SlotPrefab;
        private SerializedProperty m_SlotParent;
        private AnimBool m_ShowDynamicContainer;

        private SerializedProperty m_UseReferences;

        private SerializedProperty m_MoveUsedItems;
        private SerializedProperty m_MoveItemConditions;
        private AnimBool m_ShowMoveUsedItems;


        private ReorderableList m_MoveItemConditionList;

        private SerializedProperty m_Restrictions;

        private string[] m_PropertiesToExcludeForDefaultInspector;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.m_UseButton = serializedObject.FindProperty("useButton");
            this.m_DynamicContainer = base.serializedObject.FindProperty("m_DynamicContainer");

            this.m_SlotParent = serializedObject.FindProperty("m_SlotParent");
            this.m_SlotPrefab = serializedObject.FindProperty("m_SlotPrefab");

            if (this.m_SlotParent.objectReferenceValue == null)
            {
                GridLayoutGroup group = ((MonoBehaviour)target).gameObject.GetComponentInChildren<GridLayoutGroup>();
                if (group != null)
                {
                    serializedObject.Update();
                    this.m_SlotParent.objectReferenceValue = group.transform;
                    serializedObject.ApplyModifiedProperties();
                }
            }

            this.m_ShowDynamicContainer = new AnimBool(this.m_DynamicContainer.boolValue);
            this.m_ShowDynamicContainer.valueChanged.AddListener(new UnityAction(this.Repaint));

            this.m_UseReferences = serializedObject.FindProperty("m_UseReferences");

            this.m_MoveUsedItems = serializedObject.FindProperty("m_MoveUsedItem");
            this.m_MoveItemConditions = serializedObject.FindProperty("moveItemConditions");
            this.m_ShowMoveUsedItems = new AnimBool(this.m_MoveUsedItems.boolValue);
            this.m_ShowMoveUsedItems.valueChanged.AddListener(new UnityAction(this.Repaint));

            this.m_MoveItemConditionList = new ReorderableList(serializedObject, m_MoveItemConditions, true, true, true, true);
            this.m_MoveItemConditionList.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "Move Conditions (Window, Requires Visibility)");
            };

            this.m_MoveItemConditionList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                SerializedProperty element = m_MoveItemConditionList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.width = rect.width - 22f;
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("window"), GUIContent.none);
                rect.x += rect.width + 7f;
                rect.width = 20f;
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("requiresVisibility"), GUIContent.none);
            };

            this.m_Restrictions = serializedObject.FindProperty("restrictions");
           
            for (int i = 0; i < this.m_Restrictions.arraySize; i++) {
                this.m_Restrictions.GetArrayElementAtIndex(i).objectReferenceValue.hideFlags = HideFlags.HideInInspector;
            }


            this.m_PropertiesToExcludeForDefaultInspector = new [] {
                this.m_UseButton.propertyPath,
                this.m_DynamicContainer.propertyPath,
                this.m_SlotParent.propertyPath,
                this.m_SlotPrefab.propertyPath,
                this.m_UseReferences.propertyPath,
                this.m_MoveUsedItems.propertyPath,
                this.m_MoveItemConditions.propertyPath,
                this.m_Restrictions.propertyPath
            };
        }

        protected virtual void OnDisable()
        {
            if (this.m_ShowMoveUsedItems != null){
                this.m_ShowMoveUsedItems.valueChanged.RemoveListener(new UnityAction(this.Repaint));
            }
        }


        private void DrawInspector()
        {
            EditorGUILayout.PropertyField(this.m_UseButton);
            EditorGUILayout.PropertyField(this.m_DynamicContainer);
            this.m_ShowDynamicContainer.target = this.m_DynamicContainer.boolValue;
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowDynamicContainer.faded))
            {
                EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
                EditorGUILayout.PropertyField(this.m_SlotParent);
                EditorGUILayout.PropertyField(this.m_SlotPrefab);
                EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
            }
            EditorGUILayout.EndFadeGroup();
            ItemCollection collection = (target as ItemContainer).GetComponent<ItemCollection>();
            EditorGUI.BeginDisabledGroup(collection != null);
            if (collection != null) {
                EditorGUILayout.HelpBox("You can't use references with an ItemCollection component.", MessageType.Warning);
                this.m_UseReferences.boolValue = false;
            }
            EditorGUILayout.PropertyField(this.m_UseReferences);
            
            EditorGUI.EndDisabledGroup();

        
            DrawTypePropertiesExcluding(typeof(ItemContainer),this.m_PropertiesToExcludeForDefaultInspector);

            EditorGUILayout.PropertyField(this.m_MoveUsedItems);
            this.m_ShowMoveUsedItems.target = this.m_MoveUsedItems.boolValue;
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowMoveUsedItems.faded))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(16f);
                GUILayout.BeginVertical();
                this.m_MoveItemConditionList.DoLayoutList();
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndFadeGroup();
            if (EditorTools.RightArrowButton(new GUIContent("Restrictions", "Container Restrictions")))
            {
                AssetWindow.ShowWindow("Container Restrictions", this.m_Restrictions);
            }
        }

    }
}