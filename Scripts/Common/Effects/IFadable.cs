namespace DeathFortUnoCard.Scripts.Common.Effects
{
    public interface IFadable
    {
        float FadeDuration { get; }

        event System.Action OnFadeComplete;

        void Fade();
    }
}