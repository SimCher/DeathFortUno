using UnityEngine;

namespace DeathFortUnoCard.Scripts.Common.Extensions
{
    public static class TransformExtensions
    {
        public static void SetScaleVisibility(this Transform t, bool state)
            => t.localScale = state ? Vector3.one : Vector3.zero;
    }
}