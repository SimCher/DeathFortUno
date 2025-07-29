using System;

namespace DeathFortUnoCard.Scripts.CardField.Blocks.Interfaces
{
    public interface IBlockDataSubject
    {
        event Action StateChanged;

        void OnDestroy();
    }
}