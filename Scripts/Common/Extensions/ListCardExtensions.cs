using System.Collections.Generic;
using DeathFortUnoCard.Scripts.CardField.Cards;
using JetBrains.Annotations;

namespace DeathFortUnoCard.Scripts.Common.Extensions
{
    public static class ListCardExtensions
    {
        public static void SetSelectedCard(this IReadOnlyList<Card> cardList, [CanBeNull] Card selectedCard)
        {
            if (selectedCard == null)
            {
                for (int i = 0; i < cardList.Count; i++)
                {
                    cardList[i].IsSelected = false;
                }

                return;
            }

            for (int i = 0; i < cardList.Count; i++)
            {
                cardList[i].IsSelected = ReferenceEquals(cardList[i], selectedCard);
            }
            
        }
    }
}