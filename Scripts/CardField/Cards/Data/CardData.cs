using System;
using DeathFortUnoCard.Scripts.Common;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.CardField.Cards.Data
{
    [CreateAssetMenu(fileName = "New Card Data", menuName = "Cards/Card Data", order = 1)]
    public class CardData : ScriptableObject
    {
        [SerializeField] public GameColor color;
        [SerializeField] public CardType type;

        private bool Equals(CardData other)
            => base.Equals(other) && color == other.color && type == other.type;

        public override bool Equals(object other)
            => Equals(other as CardData);

        public override int GetHashCode()
            => HashCode.Combine(base.GetHashCode(), (int) color, (int) type);

        public (string color, string print) GetPrintInfo()
        {
            var print = type switch
            {
                CardType.Unknown => "?",
                CardType.Trap => "T",
                CardType.Move => "M",
                CardType.Bonus => "B"
            };

            var c = color switch
            {
                GameColor.Red => "red",
                GameColor.Blue => "blue",
                GameColor.Yellow => "yellow",
                GameColor.Green => "green",
                GameColor.Unknown => "???"
            };

            return (c, print);
        }

        public override string ToString()
            => $"{color} of {type}";
    }
}