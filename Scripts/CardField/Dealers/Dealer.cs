using System;
using DeathFortUnoCard.Scripts.CardField.Cards.Collections;
using DeathFortUnoCard.Scripts.CardField.Cards.Storages;
using DeathFortUnoCard.Scripts.Common;
using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using DeathFortUnoCard.UI.Scripts.Components;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace DeathFortUnoCard.Scripts.CardField.Dealers
{
    /// <summary>
    /// Раздаёт карты игрокам из игровой колоды и принимает карты от игроков в неё
    /// </summary>
    [RequireComponent(typeof(Mover))]
    public class Dealer : MonoBehaviour, IService
    {
        #region Serialized Fields

        [SerializeField] private Animator dealerHand;
        [SerializeField] private CardQueue dealQueue;
        [SerializeField] private Transform[] playerDecks;
        [SerializeField] private GameDeck gameDeck;
        [SerializeField] private DrawDeck drawDeck;
        [SerializeField] private TurnController turnController;

        public UnityEvent startDealOver;
        
        #endregion

        #region Private Fields

        private CardStorage _storage;

        private int _currentDeckIndex;
        private bool _isDealing;
        private bool _isStartDealingOver;
        
        #endregion

        #region Props

        /// <summary>
        /// Возможна ли раздача карт
        /// </summary>
        public bool CanDeal => gameDeck.IsVisible;

        private int DeckIndex
        {
            get
            {
                var curIndex = _currentDeckIndex;

                if (_currentDeckIndex + 1 >= PlayerDecks.Length)
                    _currentDeckIndex = 0;
                else
                    _currentDeckIndex++;

                return curIndex;
            }
        }

        /// <summary>
        /// Transform игровых рук
        /// </summary>
        [CanBeNull]
        public Transform[] PlayerDecks
        {
            get
            {
                if (playerDecks == null || playerDecks.Length == 0)
                {
                    Debug.LogError($"{nameof(playerDecks)} не назначены в {nameof(Dealer)}");
                    return null;
                }

                return playerDecks;
            }
        }

        #endregion

        #region Private Methods

        private Mover _mover;

        #endregion

        #region Unity Events

        private void Awake()
        {
            ServiceLocator.Register(this);
            _mover = GetComponent<Mover>();
        }

        private void OnDestroy()
        {
            startDealOver.RemoveAllListeners();
        }

        #endregion

        #region Private Methods

        private void CreateCard()
        {
            var card = gameDeck.Take();
            var cardView = _storage.GetViewByCard(card);
            dealQueue.Enqueue(cardView);
        }

        private void EnqueueCards(int count)
        {
            switch (count)
            {
                case <= 0:
                    return;
                case 1:
                    CreateCard();
                    break;
                default:
                {
                    for(int i = 0; i < count; i++)
                        CreateCard();
                    break;
                }
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(int startDealCount, int playersCount, PlayerHand[] playerHands)
        {
            for (int i = 0; i < playerHands.Length; i++)
            {
                playerDecks[i] = playerHands[i].transform;
            }
            
            _storage = ServiceLocator.Get<CardStorage>();
            EnqueueCards(startDealCount * playersCount);
        }

        public void StartDeal()
        {
            if (_isStartDealingOver)
                return;

            if (dealQueue.Count == 0)
            {
                _isStartDealingOver = true;
                startDealOver?.Invoke();
                return;
            }

            var currentCard = dealQueue.Dequeue().gameObject;
            var index = DeckIndex;
            dealerHand.Play("Deal");
            _mover.MoveToTarget(PlayerDecks[index], currentCard);
        }

        public void Take()
        {
            var playerDeck = turnController.CurrentPlayer.Data.Hand;
            var card = playerDeck.Take();
            
            if(card != null)
                drawDeck.Add(card);
        }

        public void Deal()
        {
            EnqueueCards(1);

            if (dealQueue.Count == 0)
                return;

            var current = dealQueue.Dequeue().gameObject;
            current.SetActive(true);
            
            dealerHand.Play("Deal");
            _mover.MoveToTarget(turnController.CurrentPlayer.Data.Hand.transform, current);
        }

        public bool TryDeal()
        {
            if (!CanDeal)
                return false;
            
            Deal();
            return true;
        }

        #endregion
    }
}