using System;
using UnityEngine;

namespace Swipe.Utils
{
    /// <summary>
    /// Настройки работы детектора
    /// </summary>
    [Serializable]
    public class DetectorSettings
    {
        [SerializeField] private bool enableOnPC;
        [SerializeField] private bool sendTouches;
        [SerializeField] private bool continuingSwipe;
        [SerializeField] private bool calculateVectorOnSend;
        [SerializeField] private bool useEightDirections;
        [SerializeField] private float minSwipeLength;
        
        /// <summary>
        /// Allow to send swipes if game is runnnig on PC (or Editor)
        /// </summary>
        public bool EnableOnPc => enableOnPC;
        
        /// <summary>
        /// Allow to send Toches
        /// </summary>
        public bool SendTouches => sendTouches;
        
        /// <summary>
        /// Allow to send SwipeData if Swipe wasn't finished
        /// </summary>
        public bool ContinuingSwipe => continuingSwipe;
        
        /// <summary>
        /// Calculate VectorData from SwipeDta on send
        /// </summary>
        public bool CalculateVectorOnSend => calculateVectorOnSend;
        
        /// <summary>
        /// Calculate VectorData from SwipeDta on send
        /// </summary>
        public bool UseEightDirections => useEightDirections;

        /// <summary>
        /// min length to detect currnt input as Swipe. if the length is less, input will be detected as Touch
        /// </summary>
        public float MinSwipeLength => minSwipeLength;
    }
}