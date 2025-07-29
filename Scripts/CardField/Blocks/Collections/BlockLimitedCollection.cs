using System;
using DeathFortUnoCard.Scripts.Common.Collections;

namespace DeathFortUnoCard.Scripts.CardField.Blocks.Collections
{
    [Serializable]
    public class BlockLimitedCollection : LimitedCollection<Block>
    {
        public BlockLimitedCollection(int capacity) : base(capacity)
        {
        }

        public override bool Contains(Block block)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == block)
                    return true;
            }

            return false;
        }
    }
}