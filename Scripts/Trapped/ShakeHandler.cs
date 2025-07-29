using System;
using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using DeathFortUnoCard.Scripts.Trapped.DevilScripts;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace DeathFortUnoCard.Scripts.Trapped
{
    [RequireComponent(typeof(Devil))]
    public class ShakeHandler : MonoBehaviour, IService
    {
        [SerializeField, Min(1)] private int maxShakeCount;

        [Header("События")]
        public UnityEvent onShakingReady;

        public UnityEvent onShakingStart;
        public UnityEvent onMaxShakeCountReached;
        public UnityEvent<int> onShakeCountChanged;

        private int _currentShakeCount;
        private bool _isShaking;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void OnDisable()
        {
            _currentShakeCount = 0;
        }

        private void OnDestroy()
        {
            onShakingStart.RemoveAllListeners();
            onShakingReady.RemoveAllListeners();
            onMaxShakeCountReached.RemoveAllListeners();
        }

        private void Disable()
        {
            _isShaking = false;
            onShakingReady?.Invoke();
        }

        private void Enable()
        {
            maxShakeCount = Random.Range(1, maxShakeCount);
            onShakingReady?.Invoke();
            onShakeCountChanged?.Invoke(maxShakeCount - _currentShakeCount);
        }

        public void Shake()
        {
            if (_currentShakeCount >= maxShakeCount)
            {
                onMaxShakeCountReached?.Invoke();
                return;
            }

            _isShaking = true;
            onShakingStart?.Invoke();
            _currentShakeCount++;
            onShakeCountChanged?.Invoke(maxShakeCount - _currentShakeCount);
        }
    }
}