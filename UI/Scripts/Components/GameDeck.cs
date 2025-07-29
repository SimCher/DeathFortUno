using System;
using DeathFortUnoCard.Scripts.CardField.Cards;
using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using DeathFortUnoCard.UI.Scripts.Common;
using DeathFortUnoCard.UI.Scripts.Managers;
using DeathFortUnoCard.UI.Scripts.Services;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DeathFortUnoCard.UI.Scripts.Components
{
    public class GameDeck : DeckComponent, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IService
    {
        #region Serialized Fields

        [Header("Ссылки")]
        [SerializeField] private TMP_Text cardCountText;

        #endregion

        #region Private Fields

        private UIAnimatorService _animator;
        
        private UIManager _uiManager;

        #endregion

        #region Events

        public UnityEvent onDeckIsEmpty;
        public UnityEvent onDeckSelected;

        #endregion

        #region Props

        private bool _isVisible = true;

        public bool IsVisible
        {
            get => _isVisible;
            private set
            {
                _isVisible = value;
                if(!value)
                    _animator.Decrease();
            }
        }

        #endregion

        #region Unity Events

        private void Awake()
        {
            ServiceLocator.Register(this);
            _animator = new UIAnimatorService(GetComponent<Animator>());
        }

        private void Start()
        {
            _uiManager = ServiceLocator.Get<UIManager>();
        }

        private void OnDestroy()
        {
            deck.CountChanged -= UpdateCount;
        }

        #endregion

        #region Interface Methods

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!IsVisible)
                return;
            
            onDeckSelected?.Invoke();
            
            Disable();
            
            _uiManager.SwitchSkipTurnText(true);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsVisible)
                return;
            
            _animator.Increase();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(!IsVisible)
                return;
            
            _animator.Decrease();
        }

        #endregion

        #region Private Methods

        private void UpdateCount(int count)
        {
            if (Count == 0)
            {
                Disable();
            }

            cardCountText.text = deck.Count.ToString();
        }

        #endregion

        #region Public Methods

        public void Enable()
        {
            if (!IsVisible)
                IsVisible = true;
            
            _uiManager.SwitchSkipTurnText(false);
        }

        public void Disable()
        {
            if (IsVisible)
                IsVisible = false;
        }

        public override void Add(Card card)
        {
            deck.Add(card);
            UpdateCount(deck.Count);
        }

        [CanBeNull]
        public override Card Take()
        {
            var card = deck.Take();

            if (card == null)
            {
                onDeckIsEmpty?.Invoke();
            }
            
            UpdateCount(deck.Count);
            
            Disable();

            return card;
        }

        public void ShuffleDeck(int shuffleCount) => deck.Shuffle(shuffleCount);

        #endregion
    }
}