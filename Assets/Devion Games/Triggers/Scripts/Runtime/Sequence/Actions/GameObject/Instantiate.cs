using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(GameObject))]
    [ComponentMenu("GameObject/Instantiate")]
    public class Instantiate : Action
    {
        [SerializeField]
        private TargetType m_Target = TargetType.Self;
        [SerializeField]
        private GameObject m_Original = null;
        [SerializeField]
        private string m_BoneName = string.Empty;
        [SerializeField]
        private Vector3 m_Offset = Vector3.zero;
        [SerializeField]
        private bool m_IgnorePlayerCollision = true;

        private Transform m_Bone;
      
        public override void OnStart()
        {
            this.m_Bone = FindBone(GetTarget(m_Target).transform, this.m_BoneName);
            if (this.m_Bone == null)
                this.m_Bone = GetTarget(m_Target).transform;
        }

        public override ActionStatus OnUpdate()
        {
            if (m_Original == null)
            {
                Debug.LogWarning("The game object you want to instantiate is null.");
                return ActionStatus.Failure;
            }

            GameObject go = GameObject.Instantiate(m_Original, this.m_Bone.position+this.m_Offset, this.m_Bone.rotation, this.m_Bone);
            if (m_IgnorePlayerCollision)
            {
                UnityTools.IgnoreCollision(playerInfo.gameObject, go);
            }
            return ActionStatus.Success;
        }

        private Transform FindBone(Transform current, string name)
        {
            if (current.name == name)
                return current;
            for (int i = 0; i < current.childCount; ++i)
            {
                Transform found = FindBone(current.GetChild(i), name);
                if (found != null)
                    return found;
            }
            return null;
        }
    }
}