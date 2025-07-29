using System;
using System.Collections.Generic;
using DeathFortUnoCard.Scripts.CardField.Cards.Collections;
using DeathFortUnoCard.Scripts.CardField.Cards.Data;
using DeathFortUnoCard.Scripts.CardField.Cards.Poolers;
using DeathFortUnoCard.Scripts.Common;
using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using JetBrains.Annotations;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.CardField.Cards.Storages
{
    [DefaultExecutionOrder(-700)]
    [DisallowMultipleComponent]
    public class CardStorage : MonoBehaviour, IService
    {
        [SerializeField] private CardViewPooler pooler;
        [SerializeField] private CardData[] allCardData;

        [field: SerializeField] public CardViewDictionary cardsDictionary { get; private set; }

        public CardData[] AllCardData => allCardData;
        
        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void OnDestroy()
        {
            foreach (var card in cardsDictionary.Keys)
            {
                card.Dispose();
            }
        }

        public bool TryAdd(Card card, CardView view = null)
            => cardsDictionary.TryAdd(card, view);

        public void SetCards(CardData[] newCards)
        {
            allCardData = new CardData[newCards.Length];

            for (int i = 0; i < allCardData.Length; i++)
            {
                allCardData[i] = newCards[i];
            }
        }

        public CardData GetByColorAndType(GameColor color, CardType type)
        {
            for (int i = 0; i < allCardData.Length; i++)
            {
                if (allCardData[i].color == color && allCardData[i].type == type)
                    return allCardData[i];
            }

            throw new InvalidOperationException();
        }

        [CanBeNull]
        public CardView GetViewByCard(Card card)
        {
            return cardsDictionary.GetValueOrDefault(card);
        }
    }
}