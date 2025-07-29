using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace DeathFortUnoCard.Scripts.Utils
{
    public static class MemoryProfiler
    {
        public static void LogMemoryUsage()
        {
            var totalManaged = GC.GetTotalMemory(false) / (1024 * 1024);
            Debug.Log($"<color=blue>[Managed] Исп: </color>{totalManaged} МБ");

            var unityUsed = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024);
            Debug.Log($"[Unity Profiler] Выделено: {unityUsed} МБ");

            var process = Process.GetCurrentProcess();
            var totalRAM = process.WorkingSet64 / (1024 * 1024);
            Debug.Log($"<color=blue>[Process] Задействовано процессом: </color>{totalRAM} МБ");
        }

        public static void LogDetailedMemoryUsage()
        {
            for (int i = 0; i < GC.MaxGeneration; i++)
            {
                long size = GC.CollectionCount(i);
                Debug.Log($"Поколение {i} собрано: {size}");
            }
        }
    }
}