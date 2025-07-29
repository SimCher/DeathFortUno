using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.Common.Collections
{
    [Serializable]
    public class LimitedCollection<T>
    {
        [SerializeField] protected T[] items;
        protected readonly int capacity;
        protected int count;

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= count)
                    throw new ArgumentOutOfRangeException();

                return items[index];
            }
        }

        public int Count => count;

        public LimitedCollection(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException("Вместимость должна быть больше нуля");

            this.capacity = capacity;
            items = new T[capacity];
            count = 0;
        }

        public void Add(T item)
        {
            if (count < capacity)
            {
                items[count++] = item;
            }
            else
            {
                Array.Copy(items, 1, items, 0, capacity - 1);
                items[capacity - 1] = item;
            }
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= count)
                throw new ArgumentOutOfRangeException();

            for (int i = index; i < count - 1; i++)
            {
                items[i] = items[i + 1];
            }

            items[count - 1] = default;
            count--;
        }

        public virtual bool Contains(T item)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (EqualityComparer<T>.Default.Equals(items[i], item))
                    return true;
            }

            return false;
        }
    }
}