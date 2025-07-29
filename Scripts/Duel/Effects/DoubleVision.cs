using System;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.Duel.Effects
{
    public class DoubleVision : MonoBehaviour, IFadable, IDamageEffect
    {
        [SerializeField] private Material doubleVisionMaterial;

        [Header("Параметры")]
        [SerializeField, Range(-1f, 0f)] private float leftOffset;

        [SerializeField, Range(0f, 1f)] private float rightOffset;
        
        [SerializeField] private float fadeDuration = 2f;
        [SerializeField] private float delay;

        [Header("Параметры макс.")]
        [SerializeField] private float maxLeftOffset = -0.25f;
        [SerializeField] private float maxRightOffset = 0.25f;

        private float _fadeTimer;
        private bool _isFading;

        private Coroutine _fadeRoutine;
        private static readonly int LeftOffset = Shader.PropertyToID("_LeftOffset");
        private static readonly int RightOffset = Shader.PropertyToID("_RightOffset");

        public float FadeDuration => fadeDuration;

        public event Action OnFadeCompleted;

        public float LOffset
        {
            get => leftOffset;
            private set
            {
                leftOffset = value;
                doubleVisionMaterial.SetFloat(LeftOffset, value);
            }
        }

        public float ROffset
        {
            get => rightOffset;
            private set
            {
                rightOffset = value;
                doubleVisionMaterial.SetFloat(RightOffset, value);
            }
        }

        private void Start()
        {
            if (!doubleVisionMaterial)
            {
                Debug.LogError($"{nameof(doubleVisionMaterial)} равен null.");
                enabled = false;
                return;
            }

            LOffset = 0f;
            ROffset = 0f;
        }

        private void OnDestroy()
        {
            if (_fadeRoutine != null)
            {
                StopCoroutine(_fadeRoutine);
                _fadeRoutine = null;
            }

            OnFadeCompleted = null;
        }

        private System.Collections.IEnumerator UpdateFadeRoutine()
        {
            _isFading = true;

            while (_isFading)
            {
                LOffset = Mathf.Lerp(LOffset, 0f, Time.deltaTime / fadeDuration);
                ROffset = Mathf.Lerp(ROffset, 0f, Time.deltaTime / fadeDuration);

                if (Mathf.Abs(LOffset) < 0.0001f && Mathf.Abs(ROffset) < 0.0001f)
                {
                    _isFading = false;
                    LOffset = 0f;
                    ROffset = 0f;
                    OnFadeCompleted?.Invoke();
                }

                yield return null;
            }
        }

        public void Fade()
        {
            if (_isFading)
                return;

            LOffset = maxLeftOffset;
            ROffset = maxRightOffset;

            if (_fadeRoutine != null)
            {
                StopCoroutine(_fadeRoutine);
            }

            _fadeRoutine = StartCoroutine(UpdateFadeRoutine());
        }

        public float Delay => delay;

        public void Apply()
        {
            Fade();
        }
    }
}