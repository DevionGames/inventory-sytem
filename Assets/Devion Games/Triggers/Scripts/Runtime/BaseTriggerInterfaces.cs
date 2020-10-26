using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DevionGames {
    /// <summary>
    /// Base class that all Trigger events inherit from.
    /// </summary>
    public interface ITriggerEventHandler
    {
    }

    public interface ITriggerUsedHandler : ITriggerEventHandler
    {
        /// <summary>
        /// Use this callback to detect trigger used events
        /// </summary>
        void OnTriggerUsed(GameObject player);
    }

    public interface ITriggerUnUsedHandler : ITriggerEventHandler
    {
        /// <summary>
        /// Use this callback to detect trigger un-used events
        /// </summary>
        void OnTriggerUnUsed(GameObject player);
    }

    public interface ITriggerCameInRange : ITriggerEventHandler {
        /// <summary>
        /// Use this callback to detect when player comes in range
        /// </summary>
        void OnCameInRange(GameObject player);
    }

    public interface ITriggerWentOutOfRange : ITriggerEventHandler
    {
        /// <summary>
        /// Use this callback to detect when player went out of range
        /// </summary>
        void OnWentOutOfRange(GameObject player);
    }

    public interface ITriggerPointerEnter : ITriggerEventHandler
    {
        /// <summary>
        /// Use this callback to detect when the pointer enters the trigger(Mouse over trigger)
        /// </summary>
        void OnPointerEnter(PointerEventData eventData);
    }

    public interface ITriggerPointerExit : ITriggerEventHandler
    {
        /// <summary>
        /// Use this callback to detect when the pointer exits the trigger(Mouse not longer over trigger)
        /// </summary>
        void OnPointerExit(PointerEventData eventData);
    }
}