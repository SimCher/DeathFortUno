using UnityEngine;

namespace DeathFortUnoCard.Scripts.Trapped.Utils
{
    public class LuckPredictor
    {
        private readonly System.Random _rng = new();

        private float CalculateRealChance(bool isDuel, int playerValue, int[] dealerDice)
        {
            var matchCount = 0;
            for (int i = 0; i < dealerDice.Length; i++)
            {
                if (dealerDice[i] == playerValue)
                    matchCount++;
            }

            switch (isDuel)
            {
                case true:
                    return matchCount > 0 ? 1f : 0f;
                case false:
                    if (matchCount == 0)
                        return 1f;
                    if (matchCount == dealerDice.Length)
                        return 0f;
                    return 0.5f;
            }
        }

        public float PredictOutcomePercentage(bool isDuel, int playerValue, int[] dealerDice)
        {
            var realChance = CalculateRealChance(isDuel, playerValue, dealerDice);
            var noise = (float) (_rng.NextDouble() * 0.2 - 0.1);
            var predicted = Mathf.Clamp01(realChance + noise);

            return Mathf.Round(predicted * 100f);
        }
    }
}