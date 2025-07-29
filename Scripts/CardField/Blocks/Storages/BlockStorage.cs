using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DeathFortUnoCard.Scripts.CardField.Blocks.Collections;
using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace DeathFortUnoCard.Scripts.CardField.Blocks.Storages
{
    public class BlockStorage : MonoBehaviour, IService
    {
        [SerializeField] private BlockViewDictionary blocks;

        public UnityEvent<Block> onBlockAdded;

        public int Count => blocks?.Count ?? 0;
        public Block TargetBlock { get; private set; }

        public BlockViewDictionary Blocks => blocks;

        private bool _isInitialized;

        private void Awake() => ServiceLocator.Register(this);

        private void OnDestroy()
        {
            ServiceLocator.Clear();
            onBlockAdded.RemoveAllListeners();
        }

        public void Initialize(int height, int width)
        {
            blocks = new BlockViewDictionary(height, width);
            for (int y = 0; y < blocks.Height; y++)
            {
                for (int x = 0; x < blocks.Width; x++)
                {
                    AddAt(y, x, new Block());
                }
            }
            _isInitialized = true;
        }
        
        private void CheckInitialized()
        {
            if (!_isInitialized)
                throw new InvalidOperationException($"{nameof(BlockStorage)} не инициализирован!");
        }

        public void AddView(Block block, BlockView view)
        {
            CheckInitialized();
            blocks.SetView(block, view);
        }

        [CanBeNull]
        public Block Get(int y, int x)
        {
            CheckInitialized();
            return blocks.TryGetValue(y, x, out var block) ? block : null;
        }

        [CanBeNull]
        public Block Get(Vector2Int coords) => Get(coords.y, coords.x);

        public bool TryGetView(Block block, out BlockView view)
        {
            CheckInitialized();
            return blocks.TryGetView(block, out view);
        }

        public void UpdateNeighbors()
        {
            CheckInitialized();

            if (TargetBlock == null && blocks.TryGetCenterBlock(out var center))
            {
                TargetBlock = center;
                TargetBlock.ChangeTargetState(true);
            }
            
            blocks.SetNeighbors();
        }

        public void AddAt(int y, int x, Block block)
        {
            if (blocks.ContainsCoords(y, x))
            {
                Debug.LogWarning($"Координаты {y}:{x} уже содержат Block.");
                return;
            }

            block.Coords = new Vector2Int(x, y);
            blocks.Add(y, x, block);
            onBlockAdded?.Invoke(block);
        }
        // [SerializeField] private BlockViewDictionary blocks;
        // public UnityEvent<Block> onBlockAdded;
        //
        // private bool _isInitialized;
        // public int Count => blocks.Width * blocks.Height;
        //
        // public Block TargetBlock { get; private set; }
        //
        // public BlockViewDictionary Blocks => blocks;
        // private void Awake()
        // {
        //     ServiceLocator.Register(this);
        // }
        //
        // private void OnDestroy()
        // {
        //     ServiceLocator.Clear();
        // }
        //
        // public void Initialize(int height, int width)
        // {
        //     blocks.Generate(width, height);
        //     _isInitialized = true;
        // }
        //
        // public void AddView(Block block, BlockView view) => blocks[block] = view;
        //
        // [CanBeNull]
        // public Block Get(int y, int x)
        // {
        //     if (!_isInitialized)
        //         throw new InvalidOperationException($"{nameof(BlockStorage)} не инициализирован!");
        //     return blocks.TryGetValue(y, x, out var result) ? result : null;
        // }
        //
        // [CanBeNull]
        // public Block Get(Vector2Int coords)
        // {
        //     if (!_isInitialized)
        //         throw new InvalidOperationException($"{nameof(BlockStorage)} не инициализирован!");
        //     return Get(coords.y, coords.x);
        // }
        //
        // public bool TryGetView(Block key, out BlockView view)
        // {
        //     return blocks.TryGetView(key, out view);
        // }
        //
        // public void UpdateNeighbors()
        // {
        //     if (!_isInitialized)
        //         throw new InvalidOperationException($"{nameof(BlockStorage)} не инициализирован!");
        //     
        //     if (TargetBlock == null)
        //     {
        //         if (blocks.GetCenterBlock(out var center))
        //         {
        //             TargetBlock = center;
        //             TargetBlock.ChangeTargetState(true);
        //         }
        //     }
        //     
        //     blocks.SetNeighbors();
        // }
        //
        // public void AddAt(int y, int x, Block block)
        // {
        //     if (!_isInitialized)
        //         throw new InvalidOperationException($"{nameof(BlockStorage)} не инициализирован!");
        //     
        //     if (Get(y, x) == null)
        //     {
        //         Debug.LogWarning($"{nameof(BlockStorage)} уже хранит {nameof(Block)} по координатам {y}:{x}");
        //         return;
        //     }
        //     
        //     block.SetCoords(y, x);
        //     blocks.Add(y, x, block);
        //     onBlockAdded?.Invoke(block);
        // }
        //
        // // public void PlaceObjectAt(Vector2Int blockCoords, Transform placableOther)
        // // {
        // //     var block = Get(blockCoords.y, blockCoords.x);
        // //     var topPoint = block.
        // // }
    }
}