using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace DevionGames
{
    public class ThirdPersonControllerSetupWindow : EditorWindow
    {
		private GameObject m_Character;
		private AnimatorController m_AnimatorController;
		private PhysicMaterial m_MaxFriction;
		private PhysicMaterial m_Frictionless;
		private bool m_DefaultMotions=true, m_CharacterIK = true;

		[UnityEditor.MenuItem("Tools/Devion Games/Third Person Controller/Setup Character", false,0)]
		public static void ShowWindow()
		{
			ThirdPersonControllerSetupWindow window = EditorWindow.GetWindow<ThirdPersonControllerSetupWindow>("Character Setup");
			Vector2 size = new Vector2(300f, 100f);
			window.minSize = size;
			window.wantsMouseMove = true;
			window.m_AnimatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Devion Games/Third Person Controller/Example/Animator Controllers/Third Person Controller.controller");
			window.m_MaxFriction = AssetDatabase.LoadAssetAtPath<PhysicMaterial>("Assets/Devion Games/Third Person Controller/Example/Materials/MaxFriction.physicMaterial");
			window.m_Frictionless = AssetDatabase.LoadAssetAtPath<PhysicMaterial>("Assets/Devion Games/Third Person Controller/Example/Materials/Frictionless.physicMaterial");
		}

        private void OnGUI()
        {
			if (m_Character == null) {
				EditorGUILayout.HelpBox("Select the GameObject which will be used as the character. The required components will be added to it.", MessageType.Error);
			}
			m_Character = (GameObject)EditorGUILayout.ObjectField("Character", m_Character, typeof(GameObject), true);
			if (m_Character == null) {
				return;
			}

			m_AnimatorController= (AnimatorController)EditorGUILayout.ObjectField("Animator Controller", m_AnimatorController, typeof(AnimatorController), false);
			m_DefaultMotions = EditorGUILayout.Toggle("Default Motions",m_DefaultMotions);
			m_CharacterIK = EditorGUILayout.Toggle("Character IK",m_CharacterIK);
			GUILayout.FlexibleSpace();
            if (GUILayout.Button("Build Character"))
            {
				if (EditorUtility.IsPersistent(m_Character)){
					m_Character = Instantiate(m_Character);
					Selection.activeObject = m_Character;
				}
				m_Character.layer = 2;
				m_Character.tag = "Player";
				SetupAnimatorController();
				SetupRigidbody();
				SetupCapsuleCollider();
				SetupThirdPersonController();
				SetupCharacterIK();
            }
		}

		private void SetupAnimatorController() {
			Animator animator = m_Character.GetComponent<Animator>();
			if (animator == null)
			{
				animator = m_Character.AddComponent<Animator>();
			}
			animator.runtimeAnimatorController = m_AnimatorController;
		}

		private void SetupRigidbody() {
			Rigidbody rigidbody = m_Character.GetComponent<Rigidbody>();
			if (rigidbody == null) {
				rigidbody = m_Character.AddComponent<Rigidbody>();
			}
			rigidbody.mass = 1;
			rigidbody.drag = 0;
			rigidbody.angularDrag = 999;
			rigidbody.useGravity = true;
			rigidbody.isKinematic = false;
			rigidbody.interpolation = RigidbodyInterpolation.None;
			rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
			rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		}

		private void SetupCapsuleCollider() {
			CapsuleCollider collider = m_Character.GetComponent<CapsuleCollider>();
			if (collider == null)
			{
				collider = m_Character.AddComponent<CapsuleCollider>();
			}
			collider.isTrigger = false;
			collider.material = null;
			collider.center = new Vector3(0f, 0.9f, 0f);
			collider.radius=0.25f;
			collider.height = 1.8f;
			collider.direction = 1;
		}

		private void SetupThirdPersonController() {
			ThirdPersonController controller = m_Character.GetComponent<ThirdPersonController>();
			if (controller == null) {
				controller = m_Character.AddComponent<ThirdPersonController>();
			}
			controller.IdleFriction = m_MaxFriction;
			controller.MovementFriction = m_Frictionless;
			controller.StepFriction = m_Frictionless;
			controller.AirFriction = m_Frictionless;

			if (!m_DefaultMotions) {
				return;
			}
			List<MotionState> motions = new List<MotionState>();
			Swim swim=m_Character.AddComponent<Swim>();
			swim.State = "Swim";
			motions.Add(swim);

			Fall fall= m_Character.AddComponent<Fall>();
			fall.State = "Fall";
			fall.StartType = StartType.Automatic;
			fall.StopType = StopType.Manual;
			motions.Add(fall);


			ChangeHeight changeHeight = m_Character.AddComponent<ChangeHeight>();
			changeHeight.State = "Crouch";
			changeHeight.InputName = "Crouch";
			changeHeight.StartType = StartType.Down;
			changeHeight.StopType = StopType.Toggle;
			motions.Add(changeHeight);

			Jump jump = m_Character.AddComponent<Jump>();
			jump.State = "Jump";
			jump.InputName = "Jump";
			jump.StartType = StartType.Down;
			jump.StopType = StopType.Automatic;
			motions.Add(jump);

			Push push = m_Character.AddComponent<Push>();
			push.State = "Push";
			motions.Add(push);

			ChangeSpeed changeSpeed = m_Character.AddComponent<ChangeSpeed>();
			changeSpeed.InputName = "Change Speed";
			changeSpeed.StartType = StartType.Down;
			changeSpeed.StopType = StopType.Up;
			motions.Add(changeSpeed);

			controller.Motions = motions;
		}

		private void SetupCharacterIK() {

			CharacterIK characterIK = m_Character.GetComponent<CharacterIK>();
			if (characterIK == null)
			{
				characterIK = m_Character.AddComponent<CharacterIK>();
			}

			if (!m_CharacterIK) {
				DestroyImmediate(characterIK);
			}
		}
	}
}