using DeathFortUnoCard.Scripts.CardField.Cards;
using DeathFortUnoCard.Scripts.Common.Extensions;
using JetBrains.Annotations;
using UnityEngine;

namespace DeathFortUnoCard.UI.Scripts.Components
{
    [RequireComponent(typeof(CardView))]
    public class DrawDeck : DeckComponent
    {
        [SerializeField] private CardView topCardView;

        private void Start()
        {
            topCardView = GetComponent<CardView>();
            CheckCount();
        }

        private void CheckCount()
            => transform.SetScaleVisibility(Count > 0);

        public override void Add(Card card)
        {
            deck.Add(card);
            topCardView.Initialize(card);
            
            topCardView.UpdateView(true);
            
            CheckCount();
        }

        [CanBeNull]
        public override Card Take()
        {
            var card = deck.Take();
            CheckCount();
            return card;
        }
    }
}