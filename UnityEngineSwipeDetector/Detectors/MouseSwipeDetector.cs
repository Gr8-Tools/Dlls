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
    public class MouseSwipeDetector:MonoBehaviour, ISwiperDetector
    {
        public event Action<TouchInfo> onTouch = delegate { };
        public event Action<SwipeInfo> onSwipe = delegate { };
        public event Action<object, SwipeInfo> onSpecialSwipe = delegate { };

        private DetectorSettings Params => _parent.data;
        private InputGrid TouchGrid => _parent.touchGrid;
        private Dictionary<object, Condition> SpecialSwipes => _parent.SpecialSwipes;

        private bool _isInitialized;
        private UnityEngineSwipeController _parent;
        private readonly Dictionary<int, InputInfo> _inputList = new Dictionary<int, InputInfo>();
        
        public void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            _parent = gameObject.GetComponent<UnityEngineSwipeController>();
            _isInitialized = true;
        }

        /// <summary>
        /// Блокирует определенный fingerId для чтения SwipeDetector'ом 
        /// </summary>
        [Obsolete("Не используется при управлении мышью")]
        public void LockInput(int id) { }
        [Obsolete("Не используется при управлении мышью")]
        public void UnlockInput(int id) { }

        public void Detect()
        {
            if (!_isInitialized)
                return;

            if (!Params.EnableOnPc)
                return;
            
            const int mouseButtonsCount = 3;
            for (var mouseButtonCode = 0; mouseButtonCode < mouseButtonsCount; mouseButtonCode++)
            {
                Begin(mouseButtonCode);
                Continuing(mouseButtonCode);
                End(mouseButtonCode);
            }
        }

        /// <summary>
        /// Фиксирует начало нажатия кнопкой мыши 
        /// </summary>
        private void Begin(int mouseButtonCode)
        {
            if(!unityInput.GetMouseButtonDown(mouseButtonCode))
                return;

            Vector2 mousePosition = unityInput.mousePosition;
            if (_inputList.ContainsKey(mouseButtonCode))
                _inputList.Remove(mouseButtonCode);
            
            _inputList.Add(mouseButtonCode, new InputInfo
            {
                Position = mousePosition,
                CellId = TouchGrid.GetCellId(mousePosition),
                Time = Time.time
            });
        }

        /// <summary>
        /// Фиксирует свайпы до отпускания мыши
        /// </summary>
        private void Continuing(int mouseButtonCode)
        {
            if(!Params.ContinuingSwipe)
                return;
            
            if(!unityInput.GetMouseButton(mouseButtonCode))
                return;
            
            if(!_inputList.ContainsKey(mouseButtonCode))
                return;
            
            TrySend(mouseButtonCode, unityInput.mousePosition, false);
        }

        /// <summary>
        /// Фиксирует окончание свайпов по отпусканию мыши
        /// </summary>
        /// <param name="mouseButtonCode"></param>
        private void End(int mouseButtonCode)
        {
            if(!unityInput.GetMouseButtonUp(mouseButtonCode))
                return;
            
            if(!_inputList.ContainsKey(mouseButtonCode))
                return;
            
            TrySend(mouseButtonCode, unityInput.mousePosition);
            _inputList.Remove(mouseButtonCode);
        }
        
        /// <summary>
        /// Проверяет, является ли текущий ввод "нажатием", "свайпом" или "специальным свайпом" 
        /// </summary>
        private void TrySend(int mouseButtonCode, Vector2 mousePosition, bool checkTouch = true)
        {
            if(checkTouch)
                CheckTouch(in mousePosition, _inputList[mouseButtonCode]);

            var swipeData = new SwipeInfo();
            var isSwipe = CheckSwipe(in mousePosition, _inputList[mouseButtonCode], ref swipeData);
            if (isSwipe)
                CheckSpecialSwipe(swipeData);
        }
        
        /// <summary>
        /// Проверяет, является ли текущий ввод нажатием и "сообщает" о нем 
        /// </summary>
        private void CheckTouch(in Vector2 mousePosition, InputInfo startInfo)
        {
            if (!Params.SendTouches)
                return;

            if ((mousePosition - startInfo.Position).magnitude >= Params.MinSwipeLength)
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
        private bool CheckSwipe(in Vector2 mousePosition, InputInfo startInfo, ref SwipeInfo swipeInfo)
        {
            if ((mousePosition - startInfo.Position).magnitude < Params.MinSwipeLength)
                return false;

            swipeInfo = new SwipeInfo
            {
                StartInfo = startInfo,
                FinishInfo = new InputInfo
                {
                    Position = mousePosition,
                    CellId = TouchGrid.GetCellId(mousePosition),
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
    }
}
