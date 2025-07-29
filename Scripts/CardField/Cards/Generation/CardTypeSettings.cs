using System.Collections.Generic;
using System.Linq;
using DeathFortUnoCard.Scripts.CardField.Cards.Collections;
using DeathFortUnoCard.Scripts.Utils;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.CardField.Cards.Generation
{
    [CreateAssetMenu(fileName = "New Card Type Settings", menuName = "Cards/Card Type Settings", order = 1)]
    public class CardTypeSettings : ScriptableObject
    {
        [field: SerializeField, Range(16,128)] public int TotalCount { get; private set; }
        [field: SerializeField, Range(1, 8)] public int StartDealCount { get; set; } = 4;
        [field: SerializeField, Range(7, 10)] public int CardLimit { get; set; } = 10;
        
        public CardTypeIntDictionary typePercentages = new()
        {
            {CardType.Move, 50},
            {CardType.Trap, 50}
        };

        public Dictionary<CardType, int> GetNormalizedPercentages()
        {
            var values = typePercentages.Values.ToArray();

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = MathInteger.GetPercent(TotalCount, values[i]);
            }

            var normalized = MathInteger.Normalize(TotalCount, values);

            var result = new Dictionary<CardType, int>();
            var index = 0;
            foreach (var key in typePercentages.Keys)
            {
                result[key] = normalized[index];
                index++;
            }

            return result;
        }
    }
}