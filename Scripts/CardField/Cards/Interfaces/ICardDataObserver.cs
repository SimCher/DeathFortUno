namespace DeathFortUnoCard.Scripts.CardField.Cards.Interfaces
{
    public interface ICardDataObserver
    {
        void OnStateChanged();

        void OnDestroy();
    }
}