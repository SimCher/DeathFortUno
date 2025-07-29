using DeathFortUnoCard.Scripts.Common.Players;
using DeathFortUnoCard.Scripts.Trapped;

namespace DeathFortUnoCard.Scripts.Common.States
{
    [System.Serializable]
    public class DuelData
    {
        public Player Trapped { get; set; }
        public Player TrapOwner { get; set; }

        public ShootMode Mode => Trapped == TrapOwner ? ShootMode.Suicide : ShootMode.Duel;
    }
}