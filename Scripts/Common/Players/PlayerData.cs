using DeathFortUnoCard.Scripts.CardField.Blocks;
using DeathFortUnoCard.UI.Scripts.Components;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.Common.Players
{
    [System.Serializable]
    public class PlayerData
    {
        private int _bonusTurns;
        
        [field: SerializeField] public PlayerHand Hand { get; set; }
        [field: SerializeField] public Block StartBlock { get; set; }
        [field: SerializeField] public UIPlayerMarker Marker { get; set; }

        public int BonusTurns
        {
            get => _bonusTurns;
            set
            {
                _bonusTurns = value switch
                {
                    < 0 => 0,
                    > 9 => 9,
                    _ => value
                };
            }
        }
        
        public bool SkipNextTurn { get; set; }
        
        
    }
}