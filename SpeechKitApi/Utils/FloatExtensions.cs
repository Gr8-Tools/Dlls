namespace SpeechKitApi.Utils
{
    public static class FloatExtensions
    {
        /// <summary>
        /// Возвращает значение, ограниченное сверху и снизу от min до max
        /// </summary>
        public static float Clamp(this float value, float min, float max)
            => value < min ? min : (value > max ? max : value);
        
        /// <summary>
        /// Возвращает значение, ограниченное сверху и снизу от 0 до 1 
        /// </summary>
        public static float Clamp01(this float value)
            => value < 0f ? 0f : (value > 1f ? 1f : value);
    }
}