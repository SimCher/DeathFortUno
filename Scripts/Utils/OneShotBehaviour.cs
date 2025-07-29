using UnityEngine;

namespace DeathFortUnoCard.Scripts.Utils
{
    /// <summary>
    /// Базовый MonoBehaviour, который выполняет одноразовую логику в Start
    /// и автоматически удаляет сам себя после выполнения.
    /// Используется для временных или инициализированных компонентов.
    /// </summary>
    public abstract class OneShotBehaviour : MonoBehaviour
    {
        protected virtual void Start()
        {
            Run();
            Destroy(this);
        }

        protected abstract void Run();
    }
}