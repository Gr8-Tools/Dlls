using System;

namespace Swipe.Extensions
{
    public static class FloatExtensions
    {
        public static bool Between(this float value, float minValue, float maxValue, bool include = true)
        {
            var promMinValue = Math.Min(minValue, maxValue);
            var promMaxValue = Math.Max(minValue, maxValue);
            
            if (include)
                return value >= promMinValue && value <= promMaxValue;

            return value > promMinValue && value < promMaxValue;
        }
    }
}