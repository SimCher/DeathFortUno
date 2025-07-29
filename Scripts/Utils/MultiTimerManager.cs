using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DeathFortUnoCard.Scripts.Utils
{
    /// <summary>
    /// Мультитаймер, который выпонляет свои отдельные действия по завершении
    /// </summary>
    public class MultiTimerManager : MonoBehaviour
    {
        /// <summary>
        /// Тип таймера
        /// </summary>
        [Serializable]
        private class Timer : IDisposable
        {
            private WaitForSecondsRealtime _waitDelay;

            /// <summary>
            /// Время, которое должен отсчитать таймер, в секундах
            /// </summary>
            public int timeInSeconds;

            /// <summary>
            /// Действия по истечении таймера
            /// </summary>
            public UnityEvent onTimeOut;

            /// <summary>
            /// Задержка для корутины
            /// </summary>
            public WaitForSecondsRealtime WaitDelay => _waitDelay ??= new WaitForSecondsRealtime(timeInSeconds);

            public Timer(int seconds, params UnityAction[] actions)
            {
                timeInSeconds = seconds;

                onTimeOut = new UnityEvent();

                if (actions == null || actions.Length == 0)
                    return;

                for (int i = 0; i < actions.Length; i++)
                {
                    onTimeOut.AddListener(actions[i]);
                }
            }

            public void Dispose()
            {
                onTimeOut.RemoveAllListeners();
            }
        }

        private List<Coroutine> _timerRoutines = new();

        [SerializeField]
        private List<Timer> timers = new();

        private void Awake()
        {
            _timerRoutines = new List<Coroutine>(timers.Count);
        }

        private void OnDestroy()
        {
            StopAll();

            for (int i = 0; i < timers.Count; i++)
            {
                timers[i].Dispose();
            }
        }

        private void StopAll()
        {
            for (int i = 0; i < _timerRoutines.Count; i++)
            {
                if(_timerRoutines[i] != null)
                    StopCoroutine(_timerRoutines[i]);
            }
            
            _timerRoutines.Clear();
        }


        private IEnumerator TimerCoroutine(Timer timer)
        {
            yield return timer.WaitDelay;
            
            timer.onTimeOut?.Invoke();
        }

        /// <summary>
        /// Запускает все таймеры
        /// </summary>
        public void StartAll()
        {
            for (int i = 0; i < timers.Count; i++)
            {
                _timerRoutines.Add(StartCoroutine(TimerCoroutine(timers[i])));
            }
        }
    }
}