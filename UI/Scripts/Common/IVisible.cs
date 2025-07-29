namespace DeathFortUnoCard.UI.Scripts.Common
{
    public interface IVisible
    {
        bool IsVisible { get; }

        void Show();
        void Hide();
    }
}