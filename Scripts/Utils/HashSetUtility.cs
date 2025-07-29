using System;
using System.Collections.Generic;

namespace DeathFortUnoCard.Scripts.Utils
{
    public static class HashSetUtility
    {
        public static void Add<T>(HashSet<T> set, T newItem, Action<T> onAddAction)
        {
            set.Add(newItem);
            onAddAction(newItem);
        }
        public static void ForEach<T>(HashSet<T> set, Action<T> action)
        {
            var enumerator = set.GetEnumerator();
            while (enumerator.MoveNext())
            {
                action(enumerator.Current);
            }
        }
    }
}