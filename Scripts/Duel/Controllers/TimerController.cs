using System;
using DeathFortUnoCard.Scripts.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace DeathFortUnoCard.Scripts.Duel.Controllers
{
    [RequireComponent(typeof(Timer))]
    public class TimerController : MonoBehaviour
    {
        [SerializeField] private int time = 3;

        private Timer _timer;

        public UnityEvent onEnable;

        private void Awake()
        {
            if (!TryGetComponent(out _timer))
            {
                Debug.LogError($"{nameof(TimerController)}: Компонент {nameof(_timer)} не найден на объекте {name}");
                enabled = false;
            }
        }

        private void Start()
        {
            StopTimer();
        }

        private void OnDestroy()
        {
            onEnable.RemoveAllListeners();
        }

        public void StartTimerOnce()
        {
            _timer.enabled = true;
            _timer.StartTimer(time);
            if(_timer.IsTicking)
                onEnable?.Invoke();
        }

        public void StopTimer() => _timer.enabled = false;
    }
}