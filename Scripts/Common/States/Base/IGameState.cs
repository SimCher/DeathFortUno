namespace DeathFortUnoCard.Scripts.Common.States.Base
{
    public interface IGameState<in TArgs>
    {
        void On(TArgs args);
        void Off();
    }
}