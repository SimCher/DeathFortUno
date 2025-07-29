using System.Collections.Generic;
using DeathFortUnoCard.Scripts.CardField.Blocks;
using DeathFortUnoCard.Scripts.CardField.Cards;
using JetBrains.Annotations;

namespace DeathFortUnoCard.Scripts.Common.Players.AI.Pathfinding.Services
{
    public static class CardBlockMatcher
    {
        [CanBeNull]
        public static Block GetSuitable(Card card, IEnumerable<Block> blockList)
        {
            if (card == null)
                return null;

            foreach (var block in blockList)
            {
                if(block == null)
                    continue;
                
                if(!block.IsWalkable)
                    continue;
                
                if(card.Color != block.Color)
                    continue;

                return block;
            }

            return null;
        }

        [CanBeNull]
        public static Block GetFirstNeighbor(Card card, Block current)
        {
            if (current == null)
                return null;

            foreach (var neighbor in current.Neighbors)
            {
                if(!neighbor.IsWalkable)
                    continue;
                
                if(card.Color != neighbor.Color)
                    continue;

                return neighbor;
            }

            return null;
        }
    }
}