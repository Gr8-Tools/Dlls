using Swipe.Utils.Send;

namespace Swipe.Detectors
{
    public interface ISwiperDetector
    {
        /// <summary>
        /// Событие Нажатия, определенное детектором 
        /// </summary>
        event System.Action<TouchInfo> onTouch;
        
        /// <summary>
        /// Событие "Свайпа" определенное детектором
        /// </summary>
        event System.Action<SwipeInfo> onSwipe;
        
        /// <summary>
        /// Событие "Спеицлаьного свайпа" (параметры которого жестко заданы), опредленного детектором
        /// </summary>
        event System.Action<object, SwipeInfo> onSpecialSwipe;

        /// <summary>
        /// Блокирует определенный fingerId для чтения SwipeDetector'ом 
        /// </summary>
        void LockInput(int id);
        
        /// <summary>
        /// Разблокирует определенный fingerId для чтения SwipeDetector'ом 
        /// </summary>
        void UnlockInput(int id);
        
        /// <summary>
        /// Считывает текущие сосотояния ввода для определения свайпа
        /// </summary>
        void Detect();
    }
}
