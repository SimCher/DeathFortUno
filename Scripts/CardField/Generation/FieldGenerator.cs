using System;
using System.Collections.Generic;
using DeathFortUnoCard.Scripts.CardField.Blocks;
using DeathFortUnoCard.Scripts.CardField.Blocks.Data;
using DeathFortUnoCard.Scripts.CardField.Blocks.Poolers;
using DeathFortUnoCard.Scripts.CardField.Blocks.Storages;
using DeathFortUnoCard.Scripts.Common;
using DeathFortUnoCard.Scripts.Common.Extensions;
using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace DeathFortUnoCard.Scripts.CardField.Generation
{
    public class FieldGenerator : MonoBehaviour, IService
    {
        [SerializeField] private FieldSettings settings;
        [SerializeField] private BlockPooler blockPooler;
        [SerializeField] private BlockStorage blockStorage;

        public UnityEvent Finished;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            blockPooler.Pool(settings.Size.x * settings.Size.y);
        }

        private void OnDestroy()
        {
            Finished.RemoveAllListeners();
        }

        public List<Block> Generate(List<GameColor> colorOrders)
        {
            var startBlocks = new List<Block>();

            var width = blockStorage.Blocks.Width;
            var height = blockStorage.Blocks.Height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var blockView = blockPooler.GetPooled();

                    var block = blockStorage.Get(y, x);
                    blockStorage.AddView(block, blockView);
                    blockView.Initialize(block);
                    block.SetCoords(y, x);

                    if (IsStartBlock(height, width, block))
                    {
                        startBlocks.Add(block);
                    }

                    var color = GetRandomColor();

                    block.SetColor(color);

                    var position = new Vector3(x * BlockView.BoundsSize.x, 0, y * BlockView.BoundsSize.z);
                    blockView.gameObject.InitValuesForPooled(position, Quaternion.identity);
                    blockView.transform.SetParent(blockStorage.transform);
                }
            }

            blockStorage.UpdateNeighbors();

            Finished?.Invoke();

            return startBlocks;

            GameColor GetRandomColor()
            {
                if (colorOrders.Count == 0)
                {
                    Debug.Log("Нет доступных блоков.");
                    return GameColor.Unknown;
                }

                var randomColor = colorOrders[Random.Range(0, colorOrders.Count)];
                colorOrders.Remove(randomColor);
                return randomColor;
            }
        }

        private bool IsStart(int y, int x, Block block)
            => block.Coords.x == x && block.Coords.y == y;

        private bool IsStartBlock(int height, int width, Block block)
        {
            if (IsStart(0, 0, block))
            {
                return true;
            }

            if (IsStart(height - 1, width - 1, block))
            {
                return true;
            }

            var count = settings.PlayerCount;

            if (count >= 3)
            {
                if (IsStart(height - 1, 0, block))
                {
                    return true;
                }
            }

            if (count >= 4)
            {
                if (IsStart(0, width - 1, block))
                {
                    return true;
                }
            }

            return false;
        }
    }
}