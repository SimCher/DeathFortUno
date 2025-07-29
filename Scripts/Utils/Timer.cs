using System;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace DeathFortUnoCard.Scripts.Utils
{
    public class Timer : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Настройки")]
        [SerializeField] private float timeRemaining = 10f;

        #endregion

        #region Private Fields

        private bool _isRunning;
        private float _startTime;
        private float _duration;

        #endregion

        #region Events

        public UnityEvent onTimeOut;
        public UnityEvent<float, float> onTick;

        #endregion

        #region Properties

        public bool IsTicking => _isRunning;

        #endregion

        #region Unity Events

        private void OnDestroy()
        {
            onTimeOut.RemoveAllListeners();
            onTick.RemoveAllListeners();
        }

        #endregion

        #region Private Methods

        private void Tick()
        {
            if (!_isRunning)
                return;

            timeRemaining = Mathf.Max(0f, timeRemaining - (Time.time - _startTime));
            _startTime = Time.time;
            onTick?.Invoke(timeRemaining, _duration);

            if (timeRemaining <= 0f)
            {
                Debug.Log($"<color=black>{nameof(Tick)}</color>: Trace:\n{Environment.StackTrace}");
                _isRunning = false;
                CancelInvoke(nameof(Tick));
                onTimeOut?.Invoke();
                _duration = 0f;
            }
        }

        private void LaunchTimer(float time)
        {
            timeRemaining = time;
            _duration = time;
            _isRunning = true;
            _startTime = Time.time;
        }

        private void LaunchTimer(float time, UnityAction onTimeOutAction, bool clearListeners)
        {
            if (clearListeners)
            {
                onTimeOut.RemoveAllListeners();
            }
            
            onTimeOut.AddListener(onTimeOutAction);
            
            LaunchTimer(time);
            StartTicking();
        }

        private float GetTimeFromRange(float minTime, float maxTime)
        {
            if (minTime < maxTime)
                return Random.Range(minTime, maxTime);

            return minTime > maxTime ? Random.Range(maxTime, minTime) : minTime;
        }

        private void StartTicking() => InvokeRepeating(nameof(Tick), 0f, 1f);

        #endregion

        #region Public Methods

        [ContextMenu("Start Timer")]
        public void StartTimer()
        {
            LaunchTimer(timeRemaining);
            
            StartTicking();
        }

        public void StartTimer(float time, UnityAction onTimeOutAction, bool clearListeners = true)
        {
            LaunchTimer(time, onTimeOutAction, clearListeners);
        }

        public void StartTimer(float time)
        {
            LaunchTimer(time);
            
            StartTicking();
        }
        
        public void StartTimer(float minTime, float maxTime, UnityAction onTimeOutAction, bool clearListeners = true)
        {
            LaunchTimer(GetTimeFromRange(minTime, maxTime), onTimeOutAction, clearListeners);
        }

        public void StartTimer(MinMax minMaxTime, UnityAction onTimeOutAction, bool clearListeners = true)
        {
            StartTimer(minMaxTime.min, minMaxTime.max, onTimeOutAction, clearListeners);
        }

        public void StartTimer(float minTime, float maxTime)
        {
            StartTimer(GetTimeFromRange(minTime, maxTime));
        }

        #endregion
    }
}