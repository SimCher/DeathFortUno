using System.Collections.Generic;
using DeathFortUnoCard.Scripts.CardField.Blocks;
using DeathFortUnoCard.Scripts.Common.Collections;

namespace DeathFortUnoCard.Scripts.Common.Players.AI.Pathfinding.Collections
{
    [System.Serializable]
    public class BlockHashset : SerializableHashSet<Block>
    {
        public Block this[int index] => serializedList[index];
        public BlockHashset(IEnumerable<Block> enumerable) : base(enumerable)
        {
            
        }
    }
}