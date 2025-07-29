using DeathFortUnoCard.Scripts.CardField.Blocks;
using DeathFortUnoCard.Scripts.CardField.Cards;
using JetBrains.Annotations;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.CardField.Checkers.Data
{
    [System.Serializable]
    public class PlayerSelectedData
    {
        [field: SerializeField] [CanBeNull] public Card Card { get; set; }
        [field: SerializeField] [CanBeNull] public Block Block { get; set; }

        public void Clear()
        {
            Card = null;
            Block = null;
        }
    }
}