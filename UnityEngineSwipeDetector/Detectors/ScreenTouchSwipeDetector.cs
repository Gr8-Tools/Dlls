using System;
using System.Collections.Generic;
using System.Linq;
using Swipe.Utils;
using Swipe.Utils.Input;
using Swipe.Utils.Send;
using Swipe.Utils.Special;
using UnityEngine;
using unityInput = UnityEngine.Input;

namespace Swipe.Detectors
{
    public class ScreenTouchSwipeDetector : MonoBehaviour, ISwiperDetector
    {
        /// <summary>
        /// Событие "Нажатия", определенное детектором 
        /// </summary>
        public event Action<TouchInfo> onTouch = delegate { };
        
        /// <summary>
        /// Событие "Свайпа" определенное детектором
        /// </summary>
        public event Action<SwipeInfo> onSwipe = delegate { };//Отправляем событие о том, что был совершен свайп
        
        /// <summary>
        /// Событие "Спеицлаьного свайпа" (параметры которого жестко заданы), опредленного детектором
        /// </summary>
        public event Action<object, SwipeInfo> onSpecialSwipe = delegate { };

        /// <summary>
        /// Параметры роботы дектора
        /// </summary>
        private DetectorSettings Params => _parent.data;
        
        /// <summary>
        /// Информация о сетке считывания свайпов
        /// </summary>
        private InputGrid TouchGrid => _parent.touchGrid;
        private Dictionary<object, Condition> SpecialSwipes => _parent.SpecialSwipes;

        
        private bool _isInitialized;
        private UnityEngineSwipeController _parent;
        private readonly List<int> _inputLockList = new List<int>();
        private readonly Dictionary<int, InputInfo> _touchList = new Dictionary<int, InputInfo>();

        public void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// Инициализация
        /// </summary>
        private void Initialize()
        {
            _parent = gameObject.GetComponent<UnityEngineSwipeController>();
            _isInitialized = true;
        }

        /// <summary>
        /// Блокирует определенный fingerId для чтения SwipeDetector'ом 
        /// </summary>
        public void LockInput(int id)
        {
            if (_touchList.ContainsKey(id))
                _touchList.Remove(id);

            _inputLockList.Add(id);
        }
        
        /// <summary>
        /// Разблокирует определенный fingerId для чтения SwipeDetector'ом 
        /// </summary>
        public void UnlockInput(int id)
        {
            _inputLockList.Remove(id);
        }
        
        /// <summary>
        /// Проверяет, является ли нажатие заблокированным 
        /// </summary>
        private bool IsLocked(in Touch touch)
        {
            var fingerId = touch.fingerId;
            return _inputLockList.Any(id => id == fingerId);
        }

        public void Detect()
        {
            if (!_isInitialized)
                return;

            if (unityInput.touchCount == 0)
                return;

            Begin();
            Continuing();
            End();
        }

        /// <summary>
        /// Фиксирует начала свайпов
        /// </summary>
        private void Begin()
        {
            var touches = unityInput.touches
                .Where(SelectTouches(TouchPhase.Began));
            
            foreach(var touch in touches)
            {
                if (IsLocked(in touch))
                    continue;

                var touchPosition = touch.position; 
                if (_touchList.ContainsKey(touch.fingerId))
                    _touchList.Remove(touch.fingerId);

                _touchList.Add(touch.fingerId, new InputInfo
                {
                    Position = touchPosition,
                    CellId = TouchGrid.GetCellId(touchPosition),
                    Time = Time.time
                });
            }
        }

        /// <summary>
        /// Фиксирует свайпы до отпускания пальца
        /// </summary>
        private void Continuing()
        {
            if (!Params.ContinuingSwipe)
                return;

            var touches = unityInput.touches
                .Where(SelectTouches(TouchPhase.Moved, TouchPhase.Stationary));
            
            foreach (var touch in touches)
            {
                if (IsLocked(in touch))
                    continue;

                if (!_touchList.ContainsKey(touch.fingerId))
                    continue;

                TrySend(touch, false);//В данном случае мы не отправляем тапы
            }
        }

        /// <summary>
        /// Фиксирует окончания свайпов по отпусканию пальца
        /// </summary>
        private void End()
        {
            var touches = unityInput.touches
                .Where(SelectTouches(TouchPhase.Ended, TouchPhase.Canceled));
            
            foreach (var touch in touches)
            {
                if (IsLocked(in touch))
                    continue;

                if (!_touchList.ContainsKey(touch.fingerId))
                    continue;

                TrySend(touch);

                _touchList.Remove(touch.fingerId);
            }
        }

        /// <summary>
        /// Проверяет, является ли текущий ввод "нажатием", "свайпом" или "специальным свайпом" 
        /// </summary>
        private void TrySend(Touch touch, bool checkTouch = true)
        {
            if(checkTouch)
                CheckTouch(touch, _touchList[touch.fingerId]);

            var swipeData = new SwipeInfo();
            var isSwipe = CheckSwipe(in touch, _touchList[touch.fingerId], ref swipeData);
            if (isSwipe)
                CheckSpecialSwipe(swipeData);
        }
        
        /// <summary>
        /// Проверяет, является ли текущий ввод нажатием и "сообщает" о нем 
        /// </summary>
        private void CheckTouch(Touch touch, InputInfo startInfo)
        {
            if (!Params.SendTouches)
                return;

            if ((touch.position - startInfo.Position).magnitude >= Params.MinSwipeLength)
                return;

            onTouch(new TouchInfo
            {
                CellId = startInfo.CellId,
                GridSize = TouchGrid.Size
            });
        }
        
        /// <summary>
        /// Проверяет, является ли текущиий ввод "свайпом" 
        /// </summary>
        private bool CheckSwipe(in Touch touch, InputInfo startInfo, ref SwipeInfo swipeInfo)
        {
            if ((touch.position - startInfo.Position).magnitude < Params.MinSwipeLength)
                return false;

            swipeInfo = new SwipeInfo
            {
                StartInfo = startInfo,
                FinishInfo = new InputInfo
                {
                    Position = touch.position,
                    CellId = TouchGrid.GetCellId(touch.position),
                    Time = Time.time
                },
                GridSize = TouchGrid.Size
            };
            
            if (Params.CalculateVectorOnSend)
                swipeInfo.GetOptionalInfo(Params.UseEightDirections);

            onSwipe(swipeInfo);
            return true;
        }
        
        /// <summary>
        /// Определяет "специальные свайпы" и сообщает о них 
        /// </summary>
        private void CheckSpecialSwipe(SwipeInfo swipeInfo)
        {
            if (SpecialSwipes.Count == 0)
                return;

            var specialSwipeKeyValuePairs = SpecialSwipes.Where(el => el.Value.Achieved(swipeInfo));
            foreach(var kvp in specialSwipeKeyValuePairs)
                onSpecialSwipe(kvp.Key, swipeInfo);
        }
        
        /// <summary>
        /// Возвращает функцию селектор совпадения фазы нажатия 
        /// </summary>
        private static Func<Touch, bool> SelectTouches(params TouchPhase[] phases)
            => touch => phases.Any(ph => ph == touch.phase);
    }
}
