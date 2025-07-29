using System.Collections.Generic;
using System.Linq;
using DeathFortUnoCard.Scripts.Common;
using DeathFortUnoCard.Scripts.Utils;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.CardField.Generation
{
    [CreateAssetMenu(fileName = "New Color Game Settings", menuName = "Color Game Settings", order = 1)]
    public class ColorGameSettings : ScriptableObject
    {
        public ColorGameIntDictionary colorPercentages = new()
        {
            {GameColor.Red, 25},
            {GameColor.Green, 25},
            {GameColor.Blue, 25},
            {GameColor.Yellow, 25}
        };
        
        public Dictionary<GameColor, int> GetNormalizedPercentages(int total)
        {
            var values = colorPercentages.Values.ToArray();

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = MathInteger.GetPercent(total, values[i]);
            }

            var normalized = MathInteger.Normalize(total, values);

            var result = new Dictionary<GameColor, int>();
            var index = 0;
            foreach (var key in colorPercentages.Keys)
            {
                result[key] = normalized[index];
                index++;
            }

            return result;
        }
    }
}