using System.Collections.Generic;

namespace DeathFortUnoCard.Scripts.Common.Collections
{
    public class PriorityQueue<T>
    {
        private readonly SortedDictionary<int, Queue<T>> _queues = new();
        private readonly HashSet<T> _set = new();
        private int _count;

        public int Count => _count;

        public void Enqueue(T item, int priority)
        {
            if (!_queues.TryGetValue(priority, out var queue))
            {
                queue = new Queue<T>();
                _queues[priority] = queue;
            }
            
            queue.Enqueue(item);
            _set.Add(item);
            _count++;
        }

        public T Dequeue()
        {
            foreach (var pair in _queues)
            {
                var queue = pair.Value;
                var item = queue.Dequeue();
                if (queue.Count == 0)
                    _queues.Remove(pair.Key);

                _set.Remove(item);
                _count--;
                return item;
            }

            throw new System.InvalidOperationException("Очередь пуста");
        }

        public bool Contains(T item) => _set.Contains(item);

        public T Peek()
        {
            foreach (var queue in _queues.Values)
            {
                if (queue.Count > 0)
                    return queue.Peek();
            }
            
            throw new System.InvalidOperationException("Очередь пуста");
        }

        public void Clear()
        {
            _queues.Clear();
            _set.Clear();
            _count = 0;
        }
    }
}