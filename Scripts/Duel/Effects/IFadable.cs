namespace DeathFortUnoCard.Scripts.Duel.Effects
{
    public interface IFadable
    {
        float FadeDuration { get; }

        event System.Action OnFadeCompleted;

        void Fade();
    }
}