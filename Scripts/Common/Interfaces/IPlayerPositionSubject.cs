using System;
using DeathFortUnoCard.Scripts.CardField.Blocks;

namespace DeathFortUnoCard.Scripts.Common.Interfaces
{
    public interface IPlayerPositionSubject
    {
        event Action<int, Block> BlockChanged;
    }
}