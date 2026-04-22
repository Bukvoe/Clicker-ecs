using UnityEngine;

namespace _Project.CodeBase.Shared
{
    public static class Extensions
    {
        public static float NormalizeProgress(this float current, float max)
        {
            if (max <= 0f)
            {
                return 0f;
            }

            return Mathf.Clamp01(current / max);
        }
    }
}
