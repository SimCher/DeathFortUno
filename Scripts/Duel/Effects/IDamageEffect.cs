namespace DeathFortUnoCard.Scripts.Duel.Effects
{
    public interface IDamageEffect
    {
        float Delay { get; }
        void Apply();
    }
}