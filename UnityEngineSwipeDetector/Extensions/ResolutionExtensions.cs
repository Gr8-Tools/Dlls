using UnityEngine;

namespace Swipe.Extensions
{
    public static class ResolutionExtensions
    {
        public static bool IsEmpty(this Resolution resolution)
        {
            return resolution.width == 0f && resolution.height == 0f;
        }
    }
}