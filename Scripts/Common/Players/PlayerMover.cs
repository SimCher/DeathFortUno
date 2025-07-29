using DeathFortUnoCard.Scripts.CardField.Blocks;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.Common.Players
{
    public class PlayerMover : MonoBehaviour
    {
        private TurnController _turnController;

        private void Start()
        {
            _turnController = ServiceLocator.ServiceLocator.Get<TurnController>();
        }

        public void SetCurrentPlayerDestination(BlockView selectedBlock)
        {
            if (!selectedBlock)
            {
                Debug.LogError($"Дан пустой {nameof(selectedBlock)} в {nameof(PlayerMover)}");
                return;
            }

            _turnController.CurrentPlayer.Destination = selectedBlock;
        }

        public void StartCurrentPlayerMove() => _turnController.CurrentPlayer.StartMoving();
    }
}