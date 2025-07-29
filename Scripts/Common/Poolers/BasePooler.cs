using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.Common.Poolers
{
    public abstract class BasePooler<T> : MonoBehaviour where T : Component
    {
        [SerializeField] protected GameObject prefab;
        [SerializeField, CanBeNull] protected Transform parent;
        [SerializeField] protected int initialAmount;
        [SerializeField] protected int maxPoolSize = 100;
        [SerializeField] protected bool onStart = true;

        protected readonly List<T> pooledObjects = new();
        protected readonly Queue<T> inactiveObjects = new();
        protected readonly LinkedList<T> activeObjects = new();

        public int ActiveCount => activeObjects.Count;
        public int InactiveCount => inactiveObjects.Count;

        protected bool isInitialized;

        protected void Start()
        {
            if (!onStart)
                return;
            
            if (!isInitialized)
                Initialize();
        }
        
#if UNITY_EDITOR
        protected void OnValidate()
        {
            if(!prefab) Debug.LogError($"prefab не назначен в {nameof(BasePooler<T>)}");
            if(initialAmount <= 0) Debug.LogError($"initialAmount должен быть больше нуля в {nameof(BasePooler<T>)}");
            if (initialAmount > maxPoolSize)
            {
                Debug.LogWarning($"initialAmount превышает maxPoolSize. Увеличиваю maxPoolSize вдвое!");
                while (initialAmount > maxPoolSize)
                {
                    maxPoolSize *= 2;
                }
            }
        }
#endif

        protected virtual void Initialize()
        {
            if (!prefab || initialAmount <= 0)
            {
                Debug.LogError("Некорректные настройки пула: prefab или initialAmount не назначены!");
                enabled = false;
                return;
            }

            if (!parent)
                parent = transform;

            for (int i = 0; i < initialAmount; i++)
            {
                CreateNew();
            }

            isInitialized = true;
        }

        [CanBeNull]
        protected T CreateNew()
        {
            if (pooledObjects.Count >= maxPoolSize)
                return null;

            var obj = Instantiate(prefab, parent);
            obj.SetActive(false);
            var component = obj.GetComponent<T>();
            pooledObjects.Add(component);
            inactiveObjects.Enqueue(component);
            return component;
        }

        public void Pool(int newAmount)
        {
            initialAmount = newAmount;
            if(!isInitialized)
                Initialize();
        }

        public virtual T GetPooled()
        {
            if(!isInitialized)
                Initialize();

            if (inactiveObjects.Count > 0)
            {
                var next = inactiveObjects.Dequeue();
                next.gameObject.SetActive(true);
                activeObjects.AddLast(next);
                return next;
            }

            var oldest = activeObjects.First?.Value;
            if (oldest)
            {
                Return(oldest);
                return GetPooled();
            }

            return null;
        }

        public virtual void Return(T obj)
        {
            if (!obj || !pooledObjects.Contains(obj))
            {
                Debug.LogWarning($"Объект {obj?.name} не из пула.");
                return;
            }

            if (!activeObjects.Remove(obj))
            {
                Debug.LogWarning($"Объект {obj.name} уже возвращён.");
                return;
            }
            
            obj.gameObject.SetActive(false);
            inactiveObjects.Enqueue(obj);
        }
    }
}