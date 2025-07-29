using UnityEngine;

namespace DeathFortUnoCard.Scripts.Common.Interfaces
{
    public interface IGameFieldObserver
    {
        void OnUIBlockSelected(Vector2Int coords);
    }
}