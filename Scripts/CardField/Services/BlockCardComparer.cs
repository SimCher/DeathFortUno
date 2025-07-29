using DeathFortUnoCard.Scripts.CardField.Blocks;
using DeathFortUnoCard.Scripts.CardField.Cards;

namespace DeathFortUnoCard.Scripts.CardField.Services
{
    public static class BlockCardComparer
    {
        public static bool CheckMove(Card selectedCard, Block selectedBlock, Block currentBlock)
        {
            if (!selectedBlock.IsWalkable)
                return false;

            if (!selectedBlock.IsNeighbor(currentBlock))
                return false;

            if (selectedCard.Color != selectedBlock.Color)
                return false;

            return true;
        }

        public static bool CheckTrap(Card selectedCard, Block selectedBlock)
        {
            if (!selectedBlock.IsWalkable)
                return false;

            if (selectedBlock.IsTarget)
                return false;

            if (selectedCard.Color != selectedBlock.Color)
                return false;

            return true;
        }
    }
}