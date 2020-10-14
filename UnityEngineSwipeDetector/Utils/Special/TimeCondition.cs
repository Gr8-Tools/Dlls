using System;
using Swipe.Utils.Send;
using UnityEngine;

namespace Swipe.Utils.Special
{
    /// <summary>
    /// Условие проверки на длительность (по времени) свайпа
    /// </summary>
    [Serializable]
    public class TimeCondition
    {
        [Header("Usability")] 
        [SerializeField] private bool isUsed;

        [Header("Settings")] 
        [SerializeField] private bool inBounds;
        [SerializeField] private float deltaTime;
        
        public bool IsUsed => isUsed;

        public TimeCondition(float deltaTime, bool inBounds)
        {
            this.deltaTime = deltaTime;
            this.inBounds = inBounds;
            isUsed = true;
        }
        
        /// <summary>
        /// Выполнены ли условия проверки длины
        /// </summary>
        public bool Achieved(SwipeInfo swipeInfo)
        {
            var continueTime = swipeInfo.OptionalInfo?.DeltaTime 
                         ?? swipeInfo.FinishInfo.Time - swipeInfo.StartInfo.Time;

            return inBounds
                ? continueTime <= deltaTime
                : continueTime > deltaTime;
        }
    }
}