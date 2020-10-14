using Swipe.Extensions;
using UnityEngine;

namespace Swipe.Utils.Send
{
    /// <summary>
    /// Дополнительная информация о свайпе
    /// </summary>
    public class OptionalSwipeInfo
    {
        /// <summary>
        /// Направление (Vector2)
        /// </summary>
        public Vector2 Direction { get; }//optional
        
        /// <summary>
        /// Направление (перечисление)
        /// </summary>
        public SwipeDirection SwipeDirection { get; }//optional
        
        /// <summary>
        /// Длина свайпа
        /// </summary>
        public float Length { get; }//optional
        
        /// <summary>
        /// Длительность свайпа (по вермени)
        /// </summary>
        public float DeltaTime { get; }//optional

        public OptionalSwipeInfo(SwipeInfo swipeInfo, bool useEightDirections)
        {
            var vector = (swipeInfo.FinishInfo.Position - swipeInfo.StartInfo.Position);

            Direction = vector.normalized;
            Length = vector.magnitude;
            DeltaTime = swipeInfo.FinishInfo.Time - swipeInfo.StartInfo.Time;

            SwipeDirection = Direction.GetSwipeDirection(useEightDirections);
        }
    }
}