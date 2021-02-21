using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(GameObject))]
    [ComponentMenu("GameObject/Instantiate At Mouse")]
    public class InstantiateAtMouse : Action
    {
        [SerializeField]
        private GameObject m_Original = null;
        [SerializeField]
        private bool m_IgnorePlayerCollision = true;


        public override ActionStatus OnUpdate()
        {
            if (m_Original == null)
            {
                Debug.LogWarning("The game object you want to instantiate is null.");
                return ActionStatus.Failure;
            }

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                GameObject go = GameObject.Instantiate(m_Original, hit.point, Quaternion.identity);
                if (m_IgnorePlayerCollision)
                {
                    UnityTools.IgnoreCollision(playerInfo.gameObject, go);
                }
                return ActionStatus.Success;
            }

            return ActionStatus.Failure;
        }
    }
}