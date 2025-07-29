using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.Common.Collections
{
    [Serializable]
    public class SerializableHashSet<T> : ICollection<T>, ISerializationCallbackReceiver
    {
        [SerializeField] protected List<T> serializedList = new();

        protected HashSet<T> _internalSet = new();
        
        private int _count;
        private bool _isReadOnly;

        public SerializableHashSet() { }

        public SerializableHashSet(int capacity)
        {
            serializedList = new List<T>(capacity);
            _internalSet = new HashSet<T>(capacity);
        }

        public SerializableHashSet(IEnumerable<T> enumerable)
        {
            _internalSet = new HashSet<T>(enumerable);
            serializedList = new List<T>(_internalSet);
        }

        public IEnumerator<T> GetEnumerator() => _internalSet.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T item)
        {
            if(_internalSet.Add(item))
                serializedList.Add(item);
        }

        public void Clear()
        {
            _internalSet.Clear();
            serializedList.Clear();
        }

        public bool Contains(T item) => _internalSet.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => _internalSet.CopyTo(array, arrayIndex);

        public bool Remove(T item)
        {
            if (_internalSet.Remove(item))
            {
                serializedList.Remove(item);
                return true;
            }

            return false;
        }

        public int Count => _internalSet.Count;

        public bool IsReadOnly => false;

        public void OnBeforeSerialize()
        {
            serializedList.Clear();
            foreach (var item in _internalSet)
            {
                serializedList.Add(item);
            }
        }

        public void OnAfterDeserialize()
        {
            _internalSet = new HashSet<T>(serializedList);
        }
    }
}