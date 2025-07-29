using System;
using System.Collections.Generic;
using System.Linq;

namespace DeathFortUnoCard.Scripts.Utils
{
    public static class MathInteger
    {
        private static int Normalize(int current, int total, int target)
            => (int) Math.Round((double) current / total * target);

        // public static int GetPercent(int value, int percents, int maxPercents = 100)
        //     => (value * percents + maxPercents / 2) / maxPercents;

        public static int GetPercent(int maxValue, int value, int maxPercents = 100)
        {
            if (maxValue == 0) return 0;
            return (value * maxPercents + maxValue / 2) / maxValue;
        }

        public static int[] Normalize(int target, params int[] values)
        {
            if (target <= 0)
                throw new ArgumentException("Целевая сумма значений меньше либо равна нулю!");

            var total = values.Sum();

            if (total == 0)
                throw new ArgumentException("Сумма переданных значений равна 0!");

            if (total == target)
                return values;

            var newTotal = 0;

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = Normalize(values[i], total, target);
                newTotal += values[i];
            }

            var difference = target - newTotal;

            switch (difference)
            {
                case 0:
                    return values;
                case > 0:
                {
                    for (int i = 0; i < difference; i++)
                    {
                        var maxIndex = Array.IndexOf(values, values.Max());
                        values[maxIndex]++;
                    }

                    break;
                }
                case < 0:
                {
                    for (int i = 0; i < -difference; i++)
                    {
                        var maxIndex = Array.IndexOf(values, values.Max());
                        values[maxIndex]--;
                    }

                    break;
                }
            }

            return values;
        }
    }
}