using System.Collections.Generic;
using DeathFortUnoCard.Scripts.Common.Extensions;
using DeathFortUnoCard.Scripts.Utils;

namespace DeathFortUnoCard.Scripts.Common.Generation.Services
{
    public class DictionaryRandomizer<T>
    {
        public List<T> Randomize(Dictionary<T, int> percentages, int total)
        {
            var resultList = new List<T>(total);

            var valueCount = percentages.Count;
            var keys = new T[total];
            var rawCounts = new int[valueCount];

            var i = 0;
            foreach (var kvp in percentages)
            {
                keys[i] = kvp.Key;
                rawCounts[i] = MathInteger.GetPercent(total, kvp.Value);
                i++;
            }

            var normalizedCounts = MathInteger.Normalize(total, rawCounts);

            for (int k = 0; k < valueCount; k++)
            {
                var key = keys[k];
                var count = normalizedCounts[k];

                for (int j = 0; j < count; j++)
                {
                    resultList.Add(key);
                }
            }

            resultList.Shuffle();

            return resultList;
        }
    }
}