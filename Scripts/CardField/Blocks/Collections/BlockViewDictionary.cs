using System.Collections.Generic;
using DeathFortUnoCard.Scripts.Common.Collections;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.CardField.Blocks.Collections
{
    [System.Serializable]
    public class BlockViewDictionary : SerializableDictionary<Block, BlockView>
    {
        private readonly Dictionary<Vector2Int, Block> _coordsToBlock = new();

        public int Count => _coordsToBlock.Count;

        public int Height { get; }

        public int Width { get; }

        public BlockViewDictionary(int height, int width)
        {
            Height = height;
            Width = width;
        }

        private void TrySetNeighbor(Block current, Vector2Int neighborCoords)
        {
            if (_coordsToBlock.TryGetValue(neighborCoords, out var neighbor) && neighbor != null)
            {
                current.SetNeighbor(neighbor);
            }
        }

        public void SetNeighbors()
        {
            foreach (var (coords, current) in _coordsToBlock)
            {
                if(current == null)
                    continue;

                TrySetNeighbor(current, coords + Vector2Int.left);
                TrySetNeighbor(current, coords + Vector2Int.right);
                TrySetNeighbor(current, coords + Vector2Int.up);
                TrySetNeighbor(current, coords + Vector2Int.down);
            }
        }

        public void Add(int y, int x, Block block)
        {
            var coords = new Vector2Int(x, y);
            block.Coords = coords;
            _coordsToBlock[coords] = block;
        }

        public void Remove(int y, int x)
        {
            var coords = new Vector2Int(x, y);
            _coordsToBlock.Remove(coords);
        }

        public bool TryGetValue(int y, int x, out Block block) =>
            _coordsToBlock.TryGetValue(new Vector2Int(x, y), out block);

        public bool TryGetView(Block block, out BlockView view)
            => TryGetValue(block, out view);

        public bool TryGetCenterBlock(out Block block)
        {
            block = null;
            if (_coordsToBlock.Count == 0)
                return false;

            var minX = int.MaxValue;
            var maxX = int.MinValue;
            var minY = int.MaxValue;
            var maxY = int.MinValue;

            foreach (var coords in _coordsToBlock.Keys)
            {
                if (coords.x < minX) minX = coords.x;
                if (coords.x > maxX) maxX = coords.x;
                if (coords.y < minY) minY = coords.y;
                if (coords.y > maxY) maxY = coords.y;
            }

            var center = new Vector2Int((minX + maxX) / 2, (minY + maxY) / 2);
            return _coordsToBlock.TryGetValue(center, out block);
        }

        public bool ContainsCoords(int y, int x) => _coordsToBlock.ContainsKey(new Vector2Int(x, y));

        public void SetView(Block block, BlockView view) => this[block] = view;
    }
}