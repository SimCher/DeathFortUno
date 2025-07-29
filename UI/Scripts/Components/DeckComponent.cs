using DeathFortUnoCard.Scripts.CardField.Cards;
using DeathFortUnoCard.Scripts.CardField.Cards.Collections;
using UnityEngine;

namespace DeathFortUnoCard.UI.Scripts.Components
{
    public abstract class DeckComponent : MonoBehaviour
    {
        [SerializeField] protected Deck deck = new();

        public Deck Deck
        {
            get => deck;
            protected set => deck = value;
        }

        private void OnDestroy()
        {
            deck.Dispose();
        }

        public int Count => deck.Count;

        public abstract void Add(Card card);

        public abstract Card Take();
    }
}