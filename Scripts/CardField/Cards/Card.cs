using System;
using DeathFortUnoCard.Scripts.CardField.Cards.Data;
using DeathFortUnoCard.Scripts.CardField.Cards.Interfaces;
using DeathFortUnoCard.Scripts.Common;
using DeathFortUnoCard.UI.Scripts.Components;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.CardField.Cards
{
    [Serializable]
    public class Card : ICardDataSubject, IDisposable
    {
        [SerializeField]
        private CardData data;

        [SerializeField]
        private PlayerHand owner;

        [SerializeField]
        private bool isSelected;
        
        public event Action StateChanged;
        public event Action<bool> SelectedStateChanged; 

        public GameColor Color => data.color;
        public CardType Type => data.type;

        public bool IsMove => data && data.type == CardType.Move;
        public bool IsTrap => data && data.type == CardType.Trap;

        [field: SerializeField] public bool IsEnabled { get; private set; } = true;
        
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (value != isSelected)
                {
                    isSelected = value;
                    SelectedStateChanged?.Invoke(value);
                }
            }
        }

        public PlayerHand Owner
        {
            get => owner;
            private set
            {
                owner = value;
            }
        }
        
        public Card(CardData newData)
        {
            data = newData;
        }

        public void SetOwner(PlayerHand newOwner)
        {
            Owner = newOwner;
            // if(!newOwner)
        }

        public (string color, string print) GetPrint()
        {
            if (!data)
                return ("null", "null");
            
            var print = Type switch
            {
                CardType.Unknown => "No",
                CardType.Trap => "T",
                CardType.Move => "M",
                CardType.Bonus => "B",
                _ => throw new ArgumentOutOfRangeException()
            };

            var color = Color switch
            {
                GameColor.Red => "red",
                GameColor.Blue => "blue",
                GameColor.Yellow => "yellow",
                GameColor.Green => "green",
                GameColor.Unknown => "unknown"
            };
            
            return (color, print);
        }

        public void Dispose()
        {
            StateChanged = null;
            SelectedStateChanged = null;
        }
    }
}