using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.Common.ServiceLocator
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<System.Type, IService> Services = new();

        public static int Count => Services.Count;

        public static void Register<T>(T service) where T : IService
        {
            var type = typeof(T);
            if (service == null)
            {
                Debug.LogWarning($"{nameof(service)}, [{type})] Попытка зарегистрировать null!");
                return;
            }

            if (!Services.TryAdd(type, service))
            {
                Debug.LogWarning($"Служба {type.Name} уже зарегистрирована!");
            }
        }

        [CanBeNull]
        public static T Get<T>() where T : class, IService
        {
            var type = typeof(T);

            if (Services.TryGetValue(type, out var service))
                return service as T;
            
            Debug.LogWarning($"Служба {type.Name} не найдена в {nameof(ServiceLocator)}!");
            return null;
        }

        public static void Unregister<T>() where T : IService
            => Services.Remove(typeof(T));

        public static void Clear() => Services.Clear();
    }
}