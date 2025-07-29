using System;
using System.Collections.Generic;
using DeathFortUnoCard.Scripts.CardField.Cards.Data;
using DeathFortUnoCard.Scripts.CardField.Cards.Poolers;
using DeathFortUnoCard.Scripts.CardField.Cards.Storages;
using DeathFortUnoCard.Scripts.CardField.Dealers;
using DeathFortUnoCard.Scripts.Common;
using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using DeathFortUnoCard.UI.Scripts.Components;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DeathFortUnoCard.Scripts.CardField.Cards.Generation
{
    [DefaultExecutionOrder(-700)]
    public class CardGenerator : MonoBehaviour, IService
    {
        [SerializeField] private List<GameColor> allColors = new();
        [SerializeField] private CardViewPooler cardPooler;
        
        private GameDeck _gameDeck;
        private CardStorage _cardStorage;

        private int _movePercentage;
        private int _trapPercentage;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            _cardStorage = FindFirstObjectByType<CardStorage>();
            _gameDeck = FindFirstObjectByType<GameDeck>();
            
            _gameDeck.ShuffleDeck(3);
        }

        private void GetNormalizedValues(int total, out int move, out int trap)
        {
            // var result = MathInteger.Normalize(100, settings.MoveCardPercentage, settings.TrapCardPercentage);
            //
            // _movePercentage = result[0];
            // _trapPercentage = result[1];
            //
            // var moveCount = MathInteger.GetPercent(total, _movePercentage);
            // var trapCount = total - moveCount;
            //
            // result = MathInteger.Normalize(total, moveCount, trapCount);
            //
            // move = result[0];
            // trap = result[1];
            move = 0;
            trap = 0;
        }

        private void AddCardsToDeck(int total, GameColor color, int colorCount, CardType type, float typePercentage)
        {
            var neededCard = _cardStorage.GetByColorAndType(color, type);

            if (!neededCard)
            {
                Debug.LogError($"Карта с цветом {color} и типом {type} не найдена!");
                return;
            }

            var count = Mathf.RoundToInt(colorCount * typePercentage / total);

            for (int i = 0; i < count; i++)
            {
                var newCard = new Card(neededCard);
                _cardStorage.TryAdd(newCard);
                _gameDeck.Add(newCard);
            }
        }

        public void Generate(List<GameColor> colorOrders, List<CardType> typeOrders, int total, CardStorage cardStorage,
            GameDeck gameDeck)
        {
            for (int i = 0; i < total; i++)
            {
                var card = new Card(GetRandomCard());
                var cardView = cardPooler.GetPooled();
                
                cardView.Initialize(card);
                cardStorage.TryAdd(card, cardView);
                gameDeck.Add(card);
            }

            return;

            [CanBeNull]
            CardData GetRandomCard()
            {
                if (!cardStorage || cardStorage.AllCardData.Length == 0)
                {
                    Debug.LogWarning($"{nameof(cardStorage)} не назначен или данные карт не назначены!");
                    return null;
                }

                var randomColor = colorOrders[Random.Range(0, colorOrders.Count)];
                var randomType = typeOrders[Random.Range(0, typeOrders.Count)];

                colorOrders.Remove(randomColor);
                typeOrders.Remove(randomType);
                
                return cardStorage.GetByColorAndType(randomColor, randomType);
            }
        }
        public void GenerateDeck(Dictionary<GameColor, int> percentages, int totalCards)
        {
            
            // GetNormalizedValues(total, out var move, out var trap);
            // // ServiceLocator.Get<ColorGameSettings>().GetNormalizedValues(settings.TotalCards,
            // //     out var red, out var green, out var blue, out var yellow);
            // // AddCardsToDeck(total, GameColor.Red, red, CardType.Move, move);
            // // AddCardsToDeck(total, GameColor.Red, red, CardType.Trap, trap);
            // //
            // // AddCardsToDeck(total, GameColor.Green, green, CardType.Move, move);
            // // AddCardsToDeck(total, GameColor.Green, green, CardType.Trap, trap);
            // //
            // // AddCardsToDeck(total, GameColor.Blue, blue, CardType.Move, move);
            // // AddCardsToDeck(total, GameColor.Blue, blue, CardType.Trap, trap);
            // //
            // // AddCardsToDeck(total, GameColor.Yellow, yellow, CardType.Move, move);
            // // AddCardsToDeck(total, GameColor.Yellow, yellow, CardType.Trap, trap);
            //
            // _gameDeck.ShuffleDeck(3);
            //
            // Debug.Log($"Всего карт в колоде: {_gameDeck.Count}");
        }
    }
}