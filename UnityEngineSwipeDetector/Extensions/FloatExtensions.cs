namespace Swipe.Extensions
{
    public static class FloatExtensions
    {
        public static bool Between(this float value, float minValue, float maxValue, bool include = true)
        {
            if (include)
                return value >= minValue && value <= maxValue;

            return value > minValue && value < maxValue;
        }
    }
}