using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DevionGames
{
	[InitializeOnLoad]
	public class WriteInputManager
	{
		static WriteInputManager ()
		{
			if (!AxisDefined ("Change Speed")) {
				AddAxis (new InputAxis () {
					name = "Change Speed",
					positiveButton = "left shift",
					gravity = 1000,
					dead = 0.1f,
					sensitivity = 1000f,
					type = AxisType.KeyOrMouseButton,
					axis = 1
				});
			}
			if (!AxisDefined ("Crouch")) {
				AddAxis (new InputAxis () {
					name = "Crouch",
					positiveButton = "c",
					gravity = 1000,
					dead = 0.1f,
					sensitivity = 1000f,
					type = AxisType.KeyOrMouseButton,
					axis = 1
				});
			}
			if (!AxisDefined("Left Control"))
			{
				AddAxis(new InputAxis()
				{
					name = "Left Control",
					positiveButton = "left ctrl",
					gravity = 1000,
					dead = 0.1f,
					sensitivity = 1000f,
					type = AxisType.KeyOrMouseButton,
					axis = 1
				});
			}
			if (!AxisDefined("Mouse Control"))
			{
				AddAxis(new InputAxis()
				{
					name = "Mouse Control",
					positiveButton = "m",
					gravity = 1000,
					dead = 0.1f,
					sensitivity = 1000f,
					type = AxisType.KeyOrMouseButton,
					axis = 1
				});
			}

			if (!AxisDefined("Evade"))
			{
				AddAxis(new InputAxis()
				{
					name = "Evade",
					positiveButton = "left alt",
					gravity = 1000,
					dead = 0.1f,
					sensitivity = 1000f,
					type = AxisType.KeyOrMouseButton,
					axis = 1
				});
			}
		}

		private static SerializedProperty GetChildProperty (SerializedProperty parent, string name)
		{
			SerializedProperty child = parent.Copy ();
			child.Next (true);
			do {
				if (child.name == name)
					return child;
			} while (child.Next (false));
			return null;
		}

		private static bool AxisDefined (string axisName)
		{
			SerializedObject serializedObject = new SerializedObject (AssetDatabase.LoadAllAssetsAtPath ("ProjectSettings/InputManager.asset") [0]);
			SerializedProperty axesProperty = serializedObject.FindProperty ("m_Axes");

			axesProperty.Next (true);
			axesProperty.Next (true);
			while (axesProperty.Next (false)) {
				SerializedProperty axis = axesProperty.Copy ();
				axis.Next (true);
				if (axis.stringValue == axisName)
					return true;
			}
			return false;
		}

		public enum AxisType
		{
			KeyOrMouseButton = 0,
			MouseMovement = 1,
			JoystickAxis = 2
		};

		public class InputAxis
		{
			public string name;
			public string descriptiveName;
			public string descriptiveNegativeName;
			public string negativeButton;
			public string positiveButton;
			public string altNegativeButton;
			public string altPositiveButton;

			public float gravity;
			public float dead;
			public float sensitivity;

			public bool snap = false;
			public bool invert = false;

			public AxisType type;

			public int axis;
			public int joyNum;
		}

		private static void AddAxis (InputAxis axis)
		{
			if (AxisDefined (axis.name))
				return;

			SerializedObject serializedObject = new SerializedObject (AssetDatabase.LoadAllAssetsAtPath ("ProjectSettings/InputManager.asset") [0]);
			SerializedProperty axesProperty = serializedObject.FindProperty ("m_Axes");

			axesProperty.arraySize++;
			serializedObject.ApplyModifiedProperties ();

			SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex (axesProperty.arraySize - 1);

			GetChildProperty (axisProperty, "m_Name").stringValue = axis.name;
			GetChildProperty (axisProperty, "descriptiveName").stringValue = axis.descriptiveName;
			GetChildProperty (axisProperty, "descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
			GetChildProperty (axisProperty, "negativeButton").stringValue = axis.negativeButton;
			GetChildProperty (axisProperty, "positiveButton").stringValue = axis.positiveButton;
			GetChildProperty (axisProperty, "altNegativeButton").stringValue = axis.altNegativeButton;
			GetChildProperty (axisProperty, "altPositiveButton").stringValue = axis.altPositiveButton;
			GetChildProperty (axisProperty, "gravity").floatValue = axis.gravity;
			GetChildProperty (axisProperty, "dead").floatValue = axis.dead;
			GetChildProperty (axisProperty, "sensitivity").floatValue = axis.sensitivity;
			GetChildProperty (axisProperty, "snap").boolValue = axis.snap;
			GetChildProperty (axisProperty, "invert").boolValue = axis.invert;
			GetChildProperty (axisProperty, "type").intValue = (int)axis.type;
			GetChildProperty (axisProperty, "axis").intValue = axis.axis - 1;
			GetChildProperty (axisProperty, "joyNum").intValue = axis.joyNum;

			serializedObject.ApplyModifiedProperties ();
		}
	}
}