using DeathFortUnoCard.Scripts.CardField.Cards.Data;
using UnityEngine;

namespace _Imp.DeathFortUnoCard.UI.Scripts.Collections
{
    [System.Serializable]
    public class CardDataSprite
    {
        [SerializeField] public CardData data;
        [SerializeField] public Sprite sprite;
    }
}