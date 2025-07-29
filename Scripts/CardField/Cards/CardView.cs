using DeathFortUnoCard.Scripts.CardField.Cards.Effects;
using DeathFortUnoCard.Scripts.CardField.Cards.Interfaces;
using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using DeathFortUnoCard.UI.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DeathFortUnoCard.Scripts.CardField.Cards
{
    [RequireComponent(typeof(Image), typeof(CardHoverEffect))]
    public class CardView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ICardDataObserver
    {
        [SerializeField] private Image image;
        [SerializeField] private bool isForDraw;

        [SerializeField]
        private Card data;
        private CardHoverEffect _hover;

        public Card Data => data;

        // private void OnEnable()
        // {
        //     _hover = GetComponent<CardHoverEffect>();
        //     image.color = Color.white;
        // }

        private void Awake()
        {
            _hover = GetComponent<CardHoverEffect>();
            image.color = Color.white;
        }

        private void OnDisable()
        {
            image.sprite = null;
        }

        private void Show()
        {
            var uires = ServiceLocator.Get<UIResources>();
            image.sprite = uires.GetCardSpriteByCardData(data.Color, data.Type);
        }

        private void SetDefaultView()
        {
            image.sprite = ServiceLocator.Get<UIResources>().cardBack;
        }

        public void Initialize(Card newData)
        {
            data = newData;

            data.StateChanged += OnStateChanged;
            data.SelectedStateChanged += _hover.SetSelectedState;
            
            OnStateChanged();
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!data.Owner)
                return;

            if (!data.IsEnabled)
                return;

            if (data.IsSelected)
                return;

            data.Owner.SetSelectedCard(data);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!data.Owner)
                return;

            if (!data.IsEnabled)
                return;

            if (data.IsSelected)
                return;
            
            _hover.IncreaseVfx();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!data.Owner)
                return;

            if (!data.IsEnabled)
                return;

            if (data.IsSelected)
                return;
            
            _hover.DecreaseVfx();
        }

        public void UpdateView(bool forDraw = false)
        {
            if (!data.Owner)
            {
                if (forDraw)
                {
                    Show();
                }
                else
                {
                    SetDefaultView();
                }

                return;
            }
            
            
        }

        public void OnStateChanged()
        {
            if(!isForDraw)
                gameObject.SetActive((bool)data.Owner);
            Show();
        }

        public void OnDestroy()
        {
            if (data != null)
            {
                if (_hover)
                    data.SelectedStateChanged -= _hover.SetSelectedState;
                data.StateChanged -= OnStateChanged;
            }
        }
    }
}