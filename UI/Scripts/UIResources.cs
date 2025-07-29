using System.Collections.Generic;
using _Imp.DeathFortUnoCard.UI.Scripts.Collections;
using DeathFortUnoCard.Scripts.CardField.Cards;
using DeathFortUnoCard.Scripts.CardField.Cards.Data;
using DeathFortUnoCard.Scripts.Common;
using DeathFortUnoCard.Scripts.Common.Loaders;
using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using DeathFortUnoCard.UI.Scripts.Collections;
using UnityEngine;

namespace DeathFortUnoCard.UI.Scripts
{
    public class UIResources : MonoBehaviour, IService
    {
        [SerializeField] public string showHideUI;
        [SerializeField] public string skipTurn;

        [SerializeField] public Sprite cardBack;
        [SerializeField] public CardDataSprite[] cardFronts;

        [Header("Trap Texts")]
        public TrapTextDictionary trapTexts;
        
        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Unregister<UIResources>();
        }

        public Sprite GetCardSpriteByCardData(GameColor color, CardType type)
        {
            for (int i = 0; i < cardFronts.Length; i++)
            {
                if (cardFronts[i].data.color == color && cardFronts[i].data.type == type)
                    return cardFronts[i].sprite;
            }
            
            Debug.LogError($"Не найден спрайт для {nameof(CardData)} с цветом: {color} и типом {type}");
            return null;
        }

        public void FillData(string id, List<TextSection> sections)
        {
            if (id != "trap")
                return;
            
            trapTexts.Clear();

            foreach (var section in sections)
            {
                foreach (var entry in section.entries)
                {
                    trapTexts.Add(entry.id, entry.text);
                }
            }
        }
    }
}