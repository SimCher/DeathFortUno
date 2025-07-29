using System;
using System.Collections.Generic;
using DeathFortUnoCard.Scripts.Common.Collections;

namespace DeathFortUnoCard.Scripts.CardField.Cards.Collections
{
    [Serializable]
    public class CardQueue : SerializableQueue<CardView>
    {
        public CardQueue()
        {
            
        }

        public CardQueue(int capacity) : base(capacity)
        {
            
        }

        public CardQueue(IEnumerable<CardView> cardCollection) : base(cardCollection)
        {
            
        }
    }
}