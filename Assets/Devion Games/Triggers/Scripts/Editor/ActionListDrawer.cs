using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DevionGames
{
    [CustomDrawer(typeof(List<Action>))]
    public class ActionListDrawer : CustomDrawer
    {

        public override void OnGUI(GUIContent label)
        {
            if (EditorTools.RightArrowButton(label, GUILayout.Height(20f))) {
                ObjectWindow.ShowWindow("Edit Actions", (IList)value, SetDirty);
            }
        }

    }
}