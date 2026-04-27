using System.Globalization;
using System.Numerics;
using _Project.CodeBase.Shared.Interfaces;
using Leopotam.EcsLite;
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

        public static string ToCurrencyString(this BigInteger value)
        {
            return $"${value.ToString("N0", CultureInfo.InvariantCulture)}";
        }

        public static void SetDirty<T>(this EcsPool<T> pool, int entity) where T : struct, IDirtyFlag
        {
            if (!pool.Has(entity))
            {
                pool.Add(entity);
            }
        }
    }
}
