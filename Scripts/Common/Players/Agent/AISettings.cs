namespace DeathFortUnoCard.Scripts.Common.Players.Agent
{
    [System.Serializable]
    public class AISettings
    {
        public AIPlayStyle playStyle = AIPlayStyle.Balanced;
        public AIDifficulty difficulty = AIDifficulty.Medium;
    }
}