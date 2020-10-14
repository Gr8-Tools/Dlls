using Swipe.Utils;
using UnityEngine;

namespace Swipe.Extensions
{
    public static class SwipeExtensions
    {
        public static SwipeDirection GetSwipeDirection(this Vector2 vector, bool useEightDirections = false)
        {
            float angle = Vector2.SignedAngle(Vector2.right, vector);

            if (useEightDirections)
            {
                if (angle.Between(67.5f, 112.5f)) return SwipeDirection.Up;
                if (angle.Between(22.5f, 67.5f)) return SwipeDirection.UpRight;
                if (angle.Between(-22.5f, 22.5f)) return SwipeDirection.Right;
                if (angle.Between(-67.5f, -22.5f)) return SwipeDirection.DownRight;
                if (angle.Between(-112.5f, -67.5f)) return SwipeDirection.Down;
                if (angle.Between(-157.5f, -112.5f)) return SwipeDirection.DownLeft;
                if (angle.Between(112.5f, 157.5f)) return SwipeDirection.UpLeft;
                
                return SwipeDirection.Left;
            }

            if (angle.Between(135, 45)) return SwipeDirection.Up;
            if (angle.Between(45, -45)) return SwipeDirection.Right;
            if (angle.Between(-45, -135)) return SwipeDirection.Down;

            return SwipeDirection.Left;
        }
    }
}