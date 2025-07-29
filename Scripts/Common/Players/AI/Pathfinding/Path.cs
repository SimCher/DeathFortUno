using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DeathFortUnoCard.Scripts.CardField.Blocks;
using JetBrains.Annotations;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.Common.Players.AI.Pathfinding
{
    [System.Serializable]
    public class Path : IReadOnlyList<Block>
    {
        [field: SerializeReference] public IReadOnlyList<Block> Blocks { get; set; }

        public int Count => Blocks.Count;

        public Path(IEnumerable<Block> blocks) => Blocks = blocks.ToList();

        [CanBeNull]
        public Block GetNext(int step = 1) => Count > step ? Blocks[step] : GetCurrent();

        [CanBeNull]
        public Block GetCurrent() => Count > 0 ? Blocks[0] : null;

        public bool Contains(Block block) => Blocks.Contains(block);
        public IEnumerator<Block> GetEnumerator()
        {
            foreach (var block in Blocks)
            {
                yield return block;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Block this[int index] => Blocks[index];
    }
}