using DeathFortUnoCard.Scripts.CardField.Blocks.Collections.Base;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.CardField.Blocks.Collections
{
    [System.Serializable]
    public class BlockBoard : Board<Vector2Int, Block>
    {
        public BlockBoard(int height, int width) : base(height, width)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var block = new Block
                    {
                        Coords = new Vector2Int(x, y)
                    };

                    board[y, x] = new Cell<Vector2Int, Block>
                    {
                        Key = block.Coords,
                        Value = block
                    };
                }
            }
        }

        public void SetNeighbors()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var current = board[y, x].Value;
                    if(current == null)
                        continue;

                    if (x > 0)
                    {
                        current.SetNeighbor(board[y, x - 1].Value);
                    }

                    if (x < Width - 1)
                    {
                        current.SetNeighbor(board[y, x + 1].Value);
                    }

                    if (y > 0)
                    {
                        current.SetNeighbor(board[y - 1, x].Value);
                    }

                    if (y < Height - 1)
                    {
                        current.SetNeighbor(board[y + 1, x].Value);
                    }
                }
            }
        }

        public void Add(int y, int x, Block block)
            => SetAt(y, x, block.Coords, block);

        public void Remove(int y, int x) => board[y, x] = null;

        public bool TryGetValue(int y, int x, out Block info)
        {
            var item = board[y, x];

            if (item == null)
            {
                info = null;
                return false;
            }

            info = item.Value;
            return info != null;
        }
    }
}