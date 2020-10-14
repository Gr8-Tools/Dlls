using System;
using Swipe.Utils.Send;
using UnityEngine;

namespace Swipe.Utils.Special
{
    /// <summary>
    /// Условие проверки на длинну свайпа
    /// </summary>
    [Serializable]
    public class LengthCondition
    {
        [Header("Usability")]
        [SerializeField] private bool checkMinLength;
        [SerializeField] private bool checkMaxLength;

        [Header("Clamp values")] 
        [SerializeField] private float minLength;
        [SerializeField] private float maxLength;
        
        /// <summary>
        /// Используется ли это условие
        /// </summary>
        public bool IsUsed => checkMinLength || checkMaxLength;

        public LengthCondition(bool checkMinLength, bool checkMaxLength, float minLength = 0f, float maxLength = 0f)
        {
            this.checkMinLength = checkMinLength;
            this.checkMaxLength = checkMaxLength;

            this.minLength = minLength;
            this.maxLength = maxLength;
        }

        /// <summary>
        /// Выполнены ли условия проверки длины
        /// </summary>
        public bool Achieved(SwipeInfo swipeInfo)
        {
            var length = swipeInfo.OptionalInfo?.Length 
                         ?? (swipeInfo.FinishInfo.Position - swipeInfo.StartInfo.Position).magnitude;

            if (checkMinLength && length < minLength)
                return false;

            if (checkMaxLength && length > maxLength)
                return false;

            return true;
        }
    }
}