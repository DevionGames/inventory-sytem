using UnityEditor;
using UnityEngine;

namespace DevionGames.StatSystem
{
    [InitializeOnLoad]
    public class ExecutionOrderValidator
    {
        static ExecutionOrderValidator()
        {
            var temp = new GameObject();

            var handler = temp.AddComponent<StatsHandler>();
            MonoScript handlerScript = MonoScript.FromMonoBehaviour(handler);
            if (MonoImporter.GetExecutionOrder(handlerScript) != -50)
            {
                MonoImporter.SetExecutionOrder(handlerScript, -50);
                Debug.Log("Fixing exec order for " + handlerScript.name);
            }
            Object.DestroyImmediate(temp);
        }
    }
}