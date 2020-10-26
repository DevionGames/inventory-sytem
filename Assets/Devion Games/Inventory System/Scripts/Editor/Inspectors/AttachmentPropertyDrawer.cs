using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [CustomPropertyDrawer(typeof(VisibleItem.Attachment),true)]
    public class AttachmentPropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);
            if (property.isExpanded)
            {
                SerializedProperty region = property.FindPropertyRelative("region");
				SerializedProperty prefab = property.FindPropertyRelative("prefab");
				SerializedProperty pos = property.FindPropertyRelative("position");
				SerializedProperty rotation = property.FindPropertyRelative("rotation");
				SerializedProperty scale = property.FindPropertyRelative("scale");
				SerializedProperty gameObject = property.FindPropertyRelative("gameObject");
                EditorGUI.BeginDisabledGroup(region.objectReferenceValue == null || prefab.objectReferenceValue == null);
                
				if (GUI.Button(new Rect(position.xMin + 30f, position.yMax - 20f, position.width - 30f, 20f),gameObject.objectReferenceValue != null?"Remove Prefab Handle":"Attach Prefab Handle"))
                {
					if (gameObject.objectReferenceValue != null) {
						GameObject.DestroyImmediate(gameObject.objectReferenceValue);
						return;
					}
					VisibleItem visibleItem = (VisibleItem)property.serializedObject.targetObject;
					EquipmentHandler handler = visibleItem.GetComponent<EquipmentHandler>();
					EquipmentHandler.EquipmentBone bone = handler.Bones.Find(x => x.region == region.objectReferenceValue);
					if (bone != null) {
						GameObject go=(GameObject)GameObject.Instantiate(prefab.objectReferenceValue,bone.bone.transform);
						go.transform.localPosition = pos.vector3Value;
						go.transform.localEulerAngles = rotation.vector3Value;
						go.transform.localScale = scale.vector3Value;
						gameObject.objectReferenceValue = go;
					}
                }
                EditorGUI.EndDisabledGroup();

				if (gameObject.objectReferenceValue != null) {
					Transform transform = (gameObject.objectReferenceValue as GameObject).transform;
					pos.vector3Value = transform.localPosition;
					rotation.vector3Value = transform.localEulerAngles;
					scale.vector3Value = transform.localScale;
				}
              
            }
        }

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
                return EditorGUI.GetPropertyHeight(property) + 20f;
            return EditorGUI.GetPropertyHeight(property);
        }
    }
}