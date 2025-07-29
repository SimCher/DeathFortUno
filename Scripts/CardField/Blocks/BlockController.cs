using DeathFortUnoCard.Scripts.CardField.Blocks.Storages;
using DeathFortUnoCard.Scripts.Common.Players;
using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.CardField.Blocks
{
    public class BlockController : MonoBehaviour, IService
    {
        [SerializeField] private BlockStorage blockStorage;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        public void PlacePlayerOnStartBlock(Player player, Block startBlock)
        {
            if (blockStorage.TryGetView(startBlock, out var view))
            {
                startBlock.SwitchTrapState(null);
                view.PlaceObject(player.transform);
                player.SetStartBlock(startBlock);
                return;
            }
            
            Debug.LogError($"Не удалось разместить {nameof(player)} на {nameof(startBlock)}, т.к. не удалось получить " +
                           $"{nameof(BlockView)}");
        }

        public void PlaceCurrentPlayerOnStartBlock(Player player)
        {
            if (blockStorage.TryGetView(player.Data.StartBlock, out var view))
            {
                player.Data.StartBlock.SwitchTrapState(null);
                view.PlaceObject(player.transform);
                return;
            }
            
            Debug.LogError($"Не удалось разместить {nameof(player)} на {nameof(player.Data.StartBlock)}, т.к. не " +
                           $"удалось получить " +
                           $"{nameof(BlockView)}");
        }
    }
}