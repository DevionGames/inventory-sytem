using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

namespace DevionGames
{
    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EnumFlagsAttribute attr = attribute as EnumFlagsAttribute;
            if (!string.IsNullOrEmpty(attr.label)) {
                label.text = attr.label;
            }
            if (!string.IsNullOrEmpty(attr.tooltip))
            {
                label.tooltip = attr.tooltip;
            }
            property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
        }
    }
}