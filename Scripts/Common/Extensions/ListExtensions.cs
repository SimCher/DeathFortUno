using System.Collections.Generic;
using Unity.VisualScripting;

namespace DeathFortUnoCard.Scripts.Common.Extensions
{
    public static class ListExtensions
    {
        public static void Shuffle<T>(this IList<T> items)
        {
            var n = items.Count;
            var random = new System.Random();

            for (int i = n - 1; i > 0; i--)
            {
                var j = random.Next(0, i + 1);
                (items[i], items[j]) = (items[j], items[i]);
            }
        }

        public static void Shuffle<T>(this IList<T> items, int count)
        {
            if (count <= 0)
            {
                throw new System.InvalidOperationException(
                    $"{nameof(count)} меньше или равен нулю. Перетасовка отменена.");
            }
            
            if(count == 1)
                Shuffle(items);
            else
            {
                for (int i = count; i > 0; i--)
                    Shuffle(items);
                {
                    
                }
            }
        }
    }
}