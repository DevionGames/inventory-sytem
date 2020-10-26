using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [CustomEditor(typeof(Slot),true)]
    public class SlotInspector : CallbackHandlerInspector
    {

        public override void OnInspectorGUI()
        {
            ScriptGUI();
            serializedObject.Update();
            for (int i = 0; i < m_DrawInspectors.Count; i++)
            {
                this.m_DrawInspectors[i].Invoke();
            }

            DrawPropertiesExcluding(serializedObject, this.m_PropertiesToExcludeForChildClasses);
            if (EditorTools.RightArrowButton(new GUIContent("Restrictions", "Slot restrictions")))
            {
                AssetWindow.ShowWindow("Slot Restrictions", serializedObject.FindProperty("restrictions"));
            }
            TriggerGUI();
            serializedObject.ApplyModifiedProperties();
            if (EditorWindow.mouseOverWindow != null)
            {
                EditorWindow.mouseOverWindow.Repaint();
            }
        }


       /* public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (EditorTools.RightArrowButton(new GUIContent("Restrictions", "Slot restrictions")))
            {
                AssetWindow.ShowWindow("Slot Restrictions", serializedObject.FindProperty("restrictions"));
            }
            if (EditorWindow.mouseOverWindow != null)
            {
                EditorWindow.mouseOverWindow.Repaint();
            }
        }*/
    }
}