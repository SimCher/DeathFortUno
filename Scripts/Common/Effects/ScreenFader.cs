using System;
using DeathFortUnoCard.Scripts.Duel.Effects;
using UnityEngine;
using UnityEngine.UI;

namespace DeathFortUnoCard.Scripts.Common.Effects
{
    public class ScreenFader : MonoBehaviour, IFadable, IDamageEffect
    {
        [SerializeField] private Image img;
        [SerializeField] private float fadeDuration = 2f;
        [SerializeField] private float delayBeforeFade = 1f;
        [SerializeField] private Color fadeColor = Color.black;
        [SerializeField] private float delay;
        private float _fadeAlpha = 1f;
        private bool _isFading;
        private float _fadeTimer;

        private Coroutine _fadeCoroutine;

        public float FadeDuration => fadeDuration;

        public event Action OnFadeComplete;

        private void Start()
        {
            if (!img)
            {
                Debug.LogError($"{nameof(img)} не назначен в {nameof(ScreenFader)}");
                enabled = false;
                return;
            }

            img.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, _fadeAlpha);
            img.enabled = false;
        }

        private void OnDestroy()
        {
            OnFadeComplete = null;

            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }
        }

        private System.Collections.IEnumerator UpdateFadeRoutine(float duration, float delay)
        {
            _isFading = true;
            _fadeTimer = 0f;

            while (_isFading)
            {
                var t = (_fadeTimer - delay) / duration;
                _fadeAlpha = Mathf.Lerp(1f, 0f, t * t * t);
                img.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, _fadeAlpha);

                if (_fadeAlpha <= 0f)
                {
                    img.enabled = false;
                    _isFading = false;
                    OnFadeComplete?.Invoke();
                }

                _fadeTimer += Time.deltaTime;

                yield return null;
            }
        }

        public void SetBlack()
        {
            if (_isFading)
                return;

            img.color = Color.black;
            img.enabled = true;

            _isFading = true;

            Unfade();
        }

        public void Unfade()
        {
            if (!_isFading)
                return;
            
            if(_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);

            _fadeCoroutine = StartCoroutine(UpdateFadeRoutine(fadeDuration, delayBeforeFade));
        }

        public void Fade()
        {
            if (_isFading)
                return;

            img.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 1f);
            img.enabled = true;
            _fadeAlpha = 1f;
            _fadeTimer = 0f;
            _isFading = true;
            
            Unfade();
        }

        public float Delay => delay;

        public void Apply()
        {
            Fade();
        }
    }
}