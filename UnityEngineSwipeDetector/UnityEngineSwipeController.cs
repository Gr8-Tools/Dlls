using System.Collections.Generic;
using Swipe.Detectors;
using Swipe.Utils;
using Swipe.Utils.Input;
using Swipe.Utils.Special;
using UnityEngine;

namespace Swipe
{
    public class UnityEngineSwipeController: MonoBehaviour
    {
        public ISwiperDetector Swiper { get; private set; }

        [SerializeField]
        public InputGrid touchGrid;
        [SerializeField]
        public DetectorSettings data;
        
        public readonly Dictionary<object, Condition> SpecialSwipes = new Dictionary<object, Condition>();

        protected virtual void Awake()
        {
            Input.simulateMouseWithTouches = true;
            
            var detectorType = !Input.touchSupported || !Input.touchPressureSupported
                ? typeof(MouseSwipeDetector)
                : typeof(ScreenTouchSwipeDetector);

            Swiper = gameObject.AddComponent(detectorType) as ISwiperDetector;
        }

        protected virtual void Update()
        {
            Swiper?.Detect();
        }
    }
}
