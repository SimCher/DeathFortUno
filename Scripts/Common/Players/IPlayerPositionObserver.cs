using DeathFortUnoCard.Scripts.CardField.Blocks;

namespace DeathFortUnoCard.Scripts.Common.Players
{
    public interface IPlayerPositionObserver
    {
        void OnBlockChanged(int id, Block block);
    }
}