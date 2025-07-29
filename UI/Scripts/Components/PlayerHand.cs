using System.Collections.Generic;
using DeathFortUnoCard.Scripts.CardField.Cards;
using DeathFortUnoCard.Scripts.CardField.Cards.Storages;
using DeathFortUnoCard.Scripts.CardField.Dealers;
using DeathFortUnoCard.Scripts.Common.Extensions;
using DeathFortUnoCard.Scripts.Common.Players;
using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using DeathFortUnoCard.UI.Scripts.Components.Data;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace DeathFortUnoCard.UI.Scripts.Components
{
    [RequireComponent(typeof(Visibility))]
    public class PlayerHand : MonoBehaviour, IUnique
    {
        [SerializeField] private GameObject dealer;
        [SerializeField] private List<Card> cards;
        [SerializeField] private Transform cardKeeper;
        [SerializeField] private DeckPointerCollection pointers;

        [SerializeField]
        private Card selectedCard;

        private Visibility _visibility;

        private Dealer _dealer;

        public IReadOnlyCollection<Card> Cards => cards;
        
        public int CardLimit { get; private set; }

        public int CardCount => cards.Count;

        public bool IsOverflowing => CardLimit - cards.Count < 2;
        
        [CanBeNull]
        public Card SelectedCard
        {
            get => selectedCard;
            private set
            {
                selectedCard = value;
                onCardSelected?.Invoke(value);
            }
        }

        public DeckPointerCollection Pointers => pointers;

        private CardStorage _storage;

        public UnityEvent<Card> onCardSelected;

        private void Start()
        {
            _visibility = GetComponent<Visibility>();
            _dealer = ServiceLocator.Get<Dealer>();

            _storage = ServiceLocator.Get<CardStorage>();
            dealer.GetComponent<Mover>().onMoveFinished.AddListener(AddCard);
            ServiceLocator.Get<GameDeck>().onDeckSelected.AddListener(ClearSelectedCard);

            Hide();
        }

        public bool TryDrawCard()
        {
            return _dealer.TryDeal();
        }

        public void SetCardLimit(int limit)
        {
            if (limit <= 0)
            {
                Debug.LogError($"{nameof(limit)} не может быть равен или быть меньше нуля.");
                return;
            }

            CardLimit = limit;
        }

        public Card Take()
        {
            if (SelectedCard == null)
            {
                Debug.LogWarning($"{nameof(SelectedCard)} равен null!");
                return null;
            }

            var neededCard = SelectedCard;
            SelectedCard.SetOwner(null);
            cards.SetSelectedCard(null);
            cards.Remove(SelectedCard);

            SelectedCard = null;
            var view = _storage.GetViewByCard(neededCard);
            if (!view)
            {
                Debug.LogWarning($"{nameof(view)} не найден в {nameof(PlayerHand)} на объекте {name}");
                return null;
            }
            view.gameObject.SetActive(false);

            return neededCard;
        }

        public void SetSelectedCard(Card card)
        {
            if (card == null)
                return;

            SelectedCard = card;
            cards.SetSelectedCard(SelectedCard);
        }

        public void ClearSelectedCard()
        {
            if (SelectedCard == null)
                return;

            SelectedCard = null;
            cards.SetSelectedCard(null);
        }

        public void Add(Card card)
        {
            cards.Add(card);
            card.SetOwner(this);
        }

        public void AddCard(Transform deckTransform, GameObject cardView)
        {
            if (!deckTransform.TryGetComponent(out PlayerHand hand))
                return;

            if (hand.Id != Id)
                return;

            if (!cardView.TryGetComponent(out CardView view))
                return;
            
            cardView.transform.SetParent(cardKeeper);
            cardView.transform.SetScaleVisibility(true);
            cardView.transform.SetAsLastSibling();
            
            Add(view.Data);
            
            view.UpdateView();
        }

        public void Show() => _visibility.Show();

        public void Hide() => _visibility.Hide();

        public int Id { get; private set; }

        public void SetId(int newId) => Id = newId;
    }
}