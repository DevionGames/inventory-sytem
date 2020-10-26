using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    public class EquipmentHandlerEditor : EditorWindow
    {
        public static EquipmentHandlerEditor instance;
        private SerializedObject serializedObject;
        private string searchString = "Search...";
        private Vector2 scrollPosition;
        private int selectedIndex = -1;
        private Item selectedÍtem;
        private Object target;
        
        public static void ShowWindow(SerializedObject serializedObject)
        {
            EquipmentHandlerEditor window = EditorWindow.GetWindow<EquipmentHandlerEditor>(true,"Setup Items");
            window.minSize = new Vector2(227,200);
            window.serializedObject = serializedObject;
            window.target = serializedObject.targetObject;
            window.wantsMouseMove = true;
        }

        private void OnEnable()
        {
            instance = this;
        }

        private void Update()
        {
            Repaint();
        }


        private void OnGUI()
        {
            if (serializedObject == null){
                serializedObject = new SerializedObject(target);
            }
            GUILayout.BeginVertical("grey_border");
            GUILayout.Space(6f);
            searchString = EditorTools.SearchField(searchString);
            

            GUIStyle header = new GUIStyle("In BigTitle") {
                font = EditorStyles.boldLabel.font,
                stretchWidth = true,
                
                
            };

            SerializedProperty elements = serializedObject.FindProperty("items");

            string headerTitle = (selectedIndex != -1) ?(selectedÍtem != null?selectedÍtem.Name:"Null"): "Items";
            Rect headerRect = GUILayoutUtility.GetRect(new GUIContent(headerTitle), header);
          
            if (GUI.Button(headerRect,headerTitle,header)){
                selectedIndex = -1;
            }
           
            if (selectedIndex != -1) {
                GUIStyle leftArrow = new GUIStyle("AC LeftArrow");
                GUI.Label(new Rect(headerRect.x, headerRect.y + 4f, 16f, 16f), "", leftArrow);
                serializedObject.Update();
                SerializedProperty element = elements.GetArrayElementAtIndex(selectedIndex);
                DrawItemDefinition(element);
                selectedÍtem = (Item)element.FindPropertyRelative("item").objectReferenceValue;
                serializedObject.ApplyModifiedProperties();
                return;
            }
            GUILayout.Space(-5);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
           
            GUIStyle selectButton = new GUIStyle("MeTransitionSelectHead")
            {
                alignment = TextAnchor.MiddleLeft
                
            };
            selectButton.padding.left = 10;
   
            for (int i = 0; i < elements.arraySize; i++)
            {
                SerializedProperty element = elements.GetArrayElementAtIndex(i);
                Item item = (Item)element.FindPropertyRelative("item").objectReferenceValue;
          
                if (!MatchesSearch(item, searchString))
                {
                    continue;
                }
                GUILayout.BeginHorizontal();
                Color color = GUI.backgroundColor;
                Rect rect = GUILayoutUtility.GetRect(new GUIContent((item != null ? item.Name : "Null")), selectButton, GUILayout.Height(25f));
                rect.width -= 25f;
                GUI.backgroundColor = (rect.Contains(Event.current.mousePosition) ? new Color(0, 1.0f, 0, 0.3f) : new Color(0, 0, 0, 0.0f));

                if (GUI.Button(rect,(item!=null?item.Name:"Null"), selectButton))
                {
                    GUI.FocusControl("");
                    selectedIndex = i;
                    selectedÍtem = item;
                }
                GUI.backgroundColor = color;
                Rect position = new Rect(rect.x + rect.width+4f, rect.y + 4f, 25, 25);
                if (GUI.Button(position, "", "OL Minus"))
                {
                    serializedObject.Update();
                    elements.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();
                }
                GUILayout.EndHorizontal();

            }
         
            GUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
       
            if (GUILayout.Button("Add Item", GUILayout.Height(25)))
            {
                GUI.FocusControl("");
                serializedObject.Update();
                elements.InsertArrayElementAtIndex(elements.arraySize);
                elements.GetArrayElementAtIndex(elements.arraySize - 1).FindPropertyRelative("item").objectReferenceValue = null;
                elements.GetArrayElementAtIndex(elements.arraySize - 1).FindPropertyRelative("attachments").arraySize = 0;
                serializedObject.ApplyModifiedProperties();
            }

            GUILayout.EndVertical();
        }

        private void DrawItemDefinition(SerializedProperty property)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative("item"), true);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("attachments"), true);
            GUILayout.Label("Animator:",EditorStyles.boldLabel);

           /* EditorGUILayout.PropertyField(property.FindPropertyRelative("m_InputName"), true);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("m_StartType"), true);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("m_StopType"), true);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("m_TransitionDuration"), true);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("m_State"), true);*/
        }

      

        private bool MatchesSearch(Item item, string search) {
            return searchString == "Search..." || item != null && item.Name.ToLower().Contains(search.ToLower());
        }

    }
}