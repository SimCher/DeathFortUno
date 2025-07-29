using UnityEngine;

namespace DeathFortUnoCard.Scripts.CardField.Blocks.Collections.Base
{
    [System.Serializable]
    public class Cell<TKey, TValue>
    {
        [field: SerializeField]
        public TKey Key { get; set; }
        
        [field: SerializeField]
        public TValue Value { get; set; }
    }
}