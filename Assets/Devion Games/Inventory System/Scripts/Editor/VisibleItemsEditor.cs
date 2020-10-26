using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using System.Reflection;
using System.IO;

namespace DevionGames.InventorySystem
{
    public class VisibleItemsEditor : AssetWindow
    {

        private static VisibleItemsEditor.Styles m_Styles;
        private string m_SearchString = "Search...";
        private int m_SelectedIndex = -1;
        private Item m_SelectedItem;

        public new static void ShowWindow(string title, SerializedProperty elements)
        {
            VisibleItemsEditor[] objArray = Resources.FindObjectsOfTypeAll<VisibleItemsEditor>();
            VisibleItemsEditor window = (objArray.Length <= 0 ? ScriptableObject.CreateInstance<VisibleItemsEditor>() : objArray[0]);
            window.hideFlags = HideFlags.HideAndDontSave;
            window.minSize = new Vector2(260f, 200f);
            window.titleContent = new GUIContent(title);
            window.m_ElementPropertyPath = elements.propertyPath;
            window.m_Target = elements.serializedObject.targetObject;
            window.m_Targets = new UnityEngine.Object[elements.arraySize];
            for (int i = 0; i < elements.arraySize; i++)
            {
                window.m_Targets[i] = elements.GetArrayElementAtIndex(i).objectReferenceValue;

                window.m_Targets[i].hideFlags = EditorPrefs.GetBool("InventorySystem.showAllComponents") ? HideFlags.None:HideFlags.HideInInspector;

            }
            window.m_HasPrefab = PrefabUtility.GetNearestPrefabInstanceRoot(window.m_Target) != null;
            window.m_Editors = new List<Editor>();
            window.elementType = Utility.GetType(elements.arrayElementType.Replace("PPtr<$", "").Replace(">", ""));
            for (int i = 0; i < window.m_Targets.Length; i++)
            {
                Editor editor = Editor.CreateEditor(window.m_Targets[i]);
                window.m_Editors.Add(editor);
            }
            window.FixMissingAssets();
            window.ShowUtility();
        }

        protected override void OnGUI()
        {
            if (VisibleItemsEditor.m_Styles == null)
            {
                VisibleItemsEditor.m_Styles = new VisibleItemsEditor.Styles();
            }
            DrawSearchField();
            DrawHeader();
            this.m_ScrollPosition = EditorGUILayout.BeginScrollView(this.m_ScrollPosition);
            if (this.m_SelectedIndex != -1)
            {
                DrawElement();
            }
            else
            {
                DrawElementList();
            }
           
            EditorGUILayout.EndScrollView();
            if (this.m_SelectedIndex == -1)
            {
                GUILayout.FlexibleSpace();
                DoAddButton(); 
                GUILayout.Space(10f);
            }
        }

        private void DrawElement() {
            Editor editor = this.m_Editors[this.m_SelectedIndex];
            editor.OnInspectorGUI();
            SerializedObject elementObject = new SerializedObject(this.m_Targets[this.m_SelectedIndex]);
            m_SelectedItem = elementObject.FindProperty("item").objectReferenceValue as Item;
        }

        private void DrawElementList()
        {
          /*  GUIStyle selectButton = new GUIStyle("MeTransitionSelectHead")
            {
                alignment = TextAnchor.MiddleLeft
            };
            selectButton.padding.left = 10;*/

            for (int i = 0; i < this.m_Targets.Length; i++)
            {
                UnityEngine.Object target = this.m_Targets[i];

                SerializedObject elementObject = new SerializedObject(target);

                Item item = elementObject.FindProperty("item").objectReferenceValue as Item;
                if (!SearchMatch(item) && Event.current.type== EventType.Repaint)
                {
                    continue;
                }
                GUILayout.BeginHorizontal();
                Color color = GUI.backgroundColor;
                Rect rect = GUILayoutUtility.GetRect(new GUIContent((item != null ? item.Name : "Null")), VisibleItemsEditor.m_Styles.elementButtonText, GUILayout.Height(25f));
                rect.width -= 25f;
                GUI.backgroundColor = (rect.Contains(Event.current.mousePosition) ? new Color(0, 1.0f, 0, 0.3f) : new Color(0, 0, 0, 0.0f));

                if (GUI.Button(rect, (item != null ? item.Name : "Null"), VisibleItemsEditor.m_Styles.elementButtonText))
                {
                    GUI.FocusControl("");
                    this.m_SelectedIndex = i;
                    this.m_SelectedItem = item;
                }
                GUI.backgroundColor = color;
                Rect position = new Rect(rect.x + rect.width + 4f, rect.y + 4f, 25, 25);
                if (GUI.Button(position, "", "OL Minus"))
                {
                    RemoveTarget(i);
                }
                GUILayout.EndHorizontal();
            }
        }

        private bool SearchMatch(Item item)
        {
            if (item != null && !item.Name.ToLower().Contains(m_SearchString.ToLower()))
            {
                return false;
            }
            return true;
        }

        private void DrawHeader()
        {
            GUIContent content = new GUIContent((this.m_SelectedIndex != -1) ? (this.m_SelectedItem != null ? this.m_SelectedItem.Name : "Null") : "Items");
            Rect headerRect = GUILayoutUtility.GetRect(content, VisibleItemsEditor.m_Styles.header);
            if (GUI.Button(headerRect, content, VisibleItemsEditor.m_Styles.header))
            {
                this.m_SelectedIndex = -1;
            }
            if (Event.current.type == EventType.Repaint && this.m_SelectedIndex != -1)
            {
                VisibleItemsEditor.m_Styles.leftArrow.Draw(new Rect(headerRect.x, headerRect.y + 4f, 16f, 16f), false, false, false, false);
            }
        }

        /*private void DrawHeader()
        {
            GUIStyle headerStyle = new GUIStyle("In BigTitle")
            {
                font = EditorStyles.boldLabel.font,
                stretchWidth = true,
            };

            string headerTitle = (this.m_SelectedIndex != -1) ? (this.m_SelectedItem != null ? this.m_SelectedItem.Name : "Null") : "Items";
            Rect headerRect = GUILayoutUtility.GetRect(new GUIContent(headerTitle), headerStyle);

            if (GUI.Button(headerRect, headerTitle, headerStyle))
            {
                this.m_SelectedIndex = -1;
            }
            if (this.m_SelectedIndex != -1)
            {
                GUIStyle leftArrow = new GUIStyle("AC LeftArrow");
                GUI.Label(new Rect(headerRect.x, headerRect.y + 4f, 16f, 16f), "", leftArrow);
            }
        }*/

        private void DrawSearchField()
        {
            GUILayout.Space(6f);
            EditorGUI.BeginDisabledGroup(this.m_SelectedIndex != -1);
            this.m_SearchString = SearchField(this.m_SearchString);
            EditorGUI.EndDisabledGroup();
        }

        private string SearchField(string search, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            string before = search;

            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, "ToolbarSeachTextField", options);
            rect.x += 2f;
            rect.width -= 2f;
            Rect buttonRect = rect;
            buttonRect.x = rect.width - 14;
            buttonRect.width = 14;

            if (!String.IsNullOrEmpty(before))
                EditorGUIUtility.AddCursorRect(buttonRect, MouseCursor.Arrow);

            if (Event.current.type == EventType.MouseUp && buttonRect.Contains(Event.current.mousePosition) || before == "Search..." && GUI.GetNameOfFocusedControl() == "SearchTextFieldFocus")
            {
                before = "";
                GUI.changed = true;
                GUI.FocusControl(null);

            }

                GUI.SetNextControlName("SearchTextFieldFocus");
            GUIStyle style = new GUIStyle("ToolbarSeachTextField");
            if (before == "Search...")
            {
                style.normal.textColor = Color.gray;
                style.hover.textColor = Color.gray;
            }
            string after = EditorGUI.TextField(rect, "", before, style);
            if(this.m_SelectedIndex == -1)
            EditorGUI.FocusTextInControl("SearchTextFieldFocus");

            GUI.Button(buttonRect, GUIContent.none, (after != "" && after != "Search...") ? "ToolbarSeachCancelButton" : "ToolbarSeachCancelButtonEmpty");
            EditorGUILayout.EndHorizontal();
            return after;
        }


        private class Styles
        {
            public GUIStyle header = new GUIStyle("DD HeaderStyle");
            public GUIStyle rightArrow = "AC RightArrow";
            public GUIStyle leftArrow = "AC LeftArrow";
            public GUIStyle elementButton = new GUIStyle("MeTransitionSelectHead");
            public GUIStyle background = "grey_border";
            public GUIStyle elementButtonText;

            public Styles()
            {

                this.header.stretchWidth = true;
                this.header.margin = new RectOffset(1, 1, 0, 4);

                this.elementButton.alignment = TextAnchor.MiddleLeft;
                this.elementButton.padding.left = 22;
                this.elementButton.margin = new RectOffset(1, 1, 0, 0);

                elementButtonText = new GUIStyle("MeTransitionSelectHead")
                {
                    alignment = TextAnchor.MiddleLeft,
                    padding = new RectOffset(5, 0, 0, 0),
                    overflow = new RectOffset(0, -1, 0, 0),
                    richText = true
                };
                elementButtonText.normal.background = null;
                elementButtonText.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.788f, 0.788f, 0.788f, 1f) : new Color(0.047f, 0.047f, 0.047f, 1f);
            }
        }
    }
}