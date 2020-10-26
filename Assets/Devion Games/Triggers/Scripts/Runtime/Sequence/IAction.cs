using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    public interface IAction
    {
        void Initialize(GameObject gameObject, PlayerInfo playerInfo);

        bool isActiveAndEnabled { get; }
        void OnSequenceStart();
        void OnStart();
        ActionStatus OnUpdate();
        void OnEnd();
        void OnSequenceEnd();
    }
}
