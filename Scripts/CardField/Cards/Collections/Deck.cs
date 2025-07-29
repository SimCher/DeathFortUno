using System;
using System.Collections.Generic;
using DeathFortUnoCard.Scripts.Common;
using DeathFortUnoCard.Scripts.Common.Extensions;
using JetBrains.Annotations;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.CardField.Cards.Collections
{
    [System.Serializable]
    public class Deck : IDisposable
    {
        [SerializeField] private List<Card> cards = new();

        public int Count => cards.Count;

        private System.Random _random = new();

        public event System.Action<int> CountChanged;

        public IReadOnlyList<Card> Cards => cards;

        private void OnCountChanged() => CountChanged?.Invoke(Count);

        public void Add(Card card)
        {
            cards.Add(card);

            OnCountChanged();
        }

        public void AddRange(IEnumerable<Card> cardCollection)
        {
            cards.AddRange(cardCollection);
            
            OnCountChanged();
        }

        [CanBeNull]
        public Card TakeAt(int index)
        {
            if (cards.Count == 0 || index < 0 || index >= Count)
                return null;

            var drawnCard = cards[index];
            cards.RemoveAt(index);
            OnCountChanged();
            return drawnCard;
        }

        [CanBeNull]
        public Card Take() => TakeAt(0);

        [CanBeNull]
        public Card Peek()
            => cards.Count > 0 ? cards[0] : null;

        [CanBeNull]
        public Card TakeRandom()
        {
            var index = _random.Next(Count);
            return TakeAt(index);
        }

        public void Clear()
        {
            if (cards.Count == 0)
                return;
            
            cards.Clear();
            OnCountChanged();
        }

        public void Shuffle(int count = 1)
            => cards.Shuffle(count);

#if UNITY_EDITOR
        public void PrintStats()
        {
            int reds = 0, yellows = 0, greens = 0, blues = 0, moves = 0, traps = 0;

            foreach (var card in cards)
            {
                switch (card.Color)
                {
                    case GameColor.Red: reds++;
                        break;
                    case GameColor.Yellow: yellows++;
                        break;
                    case GameColor.Green: greens++;
                        break;
                    case GameColor.Blue: blues++;
                        break;
                }

                switch (card.Type)
                {
                    case CardType.Move: moves++;
                        break;
                    case CardType.Trap: traps++;
                        break;
                }
            }

            var pirnt =
                $"Reds: {reds} Yellows: {yellows} Greens: {greens} Blues: {blues} Moves: {moves} Traps: {traps}";
            
            Debug.Log(pirnt);
        }
#endif
        public void Dispose()
        {
            CountChanged = null;
        }
    }
}