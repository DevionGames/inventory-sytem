using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    public enum Direction
    {
        Forward,
        Backward,
        Up,
        Down,
        Left,
        Right
    }

    public static class PhysicsUtility
    {
        public static Vector3 GetDirection(Transform transform, Direction direction) {
            switch (direction)
            {
                case Direction.Backward:
                    return -transform.forward;
                case Direction.Up:
                    return transform.up;
                case Direction.Down:
                    return -transform.up;
                case Direction.Left:
                    return -transform.right;
                case Direction.Right:
                    return transform.right;
                default:
                    return transform.forward;
            }
        }
    }
}