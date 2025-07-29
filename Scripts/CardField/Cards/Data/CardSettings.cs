using UnityEngine;

namespace DeathFortUnoCard.Scripts.CardField.Cards.Data
{
    [CreateAssetMenu(fileName = "New Card Settings", menuName = "Cards/Card Settings", order = 1)]
    public class CardSettings : ScriptableObject
    {
        [field: SerializeField, Range(16, 128)]
        public int TotalCards { get; private set; } = 36;

        [field: SerializeField, Range(0, 100)] public int MoveCardPercentage { get; set; } = 50;
        [field: SerializeField, Range(0, 100)] public int TrapCardPercentage { get; set; } = 50;
        [field: SerializeField, Range(1, 8)] public int StartDealCount { get; set; } = 1;
        [field: SerializeField, Range(7, 10)] public int CardLimit { get; private set; } = 10;
        
        
    }
}