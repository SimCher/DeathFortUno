using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.Common.Collections
{
    [Serializable]
    public class SerializableQueue<T> : Queue<T>, ISerializationCallbackReceiver
    {
        [SerializeField] protected T[] elements;

        public SerializableQueue()
        {
            
        }

        public SerializableQueue(int capacity) : base(capacity)
        {
            
        }

        public SerializableQueue(IEnumerable<T> collection) : base(collection)
        {
            
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
#if UNITY_EDITOR
            elements = ToArray();
#endif
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
#if UNITY_EDITOR
            if (elements != null)
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    Enqueue(elements[i]);
                }
            }
#endif
        }
    }
}