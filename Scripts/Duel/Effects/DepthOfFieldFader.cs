using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace DeathFortUnoCard.Scripts.Duel.Effects
{
    [RequireComponent(typeof(Volume))]
    public class DepthOfFieldFader : MonoBehaviour, IFadable, IDamageEffect
    {
        private Volume _volume;

        private DepthOfField _depth;

        [Header("Настройки глубины")]
        [SerializeField, Range(10f, 300f)] private float depthLimit;

        [SerializeField] private float fadeDuration = 2f;
        [SerializeField] private float delayBeforeFade = 1f;

        private Coroutine _depthCoroutine;

        private float _startValue;
        private bool _isFading;
        private float _fadeTimer;
        private float _currentValue;
        [SerializeField] private float delay;

        public float FadeDuration => fadeDuration;

        private float CurrentDepth
        {
            get => _depth.focalLength.value;
            set => _depth.focalLength.value = value;
        }

        public event Action OnFadeCompleted;

        private void Awake()
        {
            if (!TryGetComponent(out _volume))
            {
                Debug.LogError($"Не могу найти {nameof(Volume)} на компоненте {name}");
                enabled = false;
            }
        }

        private void Start()
        {
            _volume.profile.TryGet(out _depth);

            _startValue = (float) _depth.focalLength;
        }

        private void OnDestroy()
        {
            OnFadeCompleted = null;
            
            if(_depthCoroutine != null)
                StopCoroutine(_depthCoroutine);
        }

        private System.Collections.IEnumerator DecreaseDepthOfField()
        {
            _isFading = true;
            _fadeTimer = 0f;

            while (_isFading)
            {
                var t = (_fadeTimer - delayBeforeFade) / fadeDuration;
                _depth.focalLength.value = Mathf.Lerp(depthLimit, _startValue, t * t * t);

                if (CurrentDepth <= _startValue)
                {
                    _isFading = false;
                    CurrentDepth = _startValue;
                    OnFadeCompleted?.Invoke();
                }

                _fadeTimer += Time.deltaTime;
                yield return null;
            }
        }

        [ContextMenu(nameof(Fade))]
        public void Fade()
        {
            if (_isFading)
                return;

            CurrentDepth = depthLimit;
            _fadeTimer = 0f;
            _isFading = true;

            if (_depthCoroutine != null)
            {
                StopCoroutine(_depthCoroutine);
                _depthCoroutine = null;
            }

            _depthCoroutine = StartCoroutine(DecreaseDepthOfField());
        }

        public float Delay => delay;

        public void Apply()
        {
            Fade();
        }
    }
}