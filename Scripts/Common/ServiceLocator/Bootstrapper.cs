using DeathFortUnoCard.Scripts.Utils;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.Common.ServiceLocator
{
    internal static class Bootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Initialize() => MemoryProfiler.LogMemoryUsage();
    }
}