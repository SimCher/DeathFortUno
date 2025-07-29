using TMPro;
using UnityEngine;

namespace DeathFortUnoCard.UI.Scripts.Components
{
    public class UISuccessText : MonoBehaviour
    {
        [SerializeField] private TMP_Text percentageText;
        [SerializeField] private float baseSpeed = 60f;
        [SerializeField] private float slowdownRange = 10f;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color highChanceColor = Color.yellow;

        private float _currentValue;
        private float _targetValue;
        private Coroutine _animateCoroutine;

        private Vector3 _originalPos;
        private readonly WaitForEndOfFrame _wait = new();

        private void Awake()
        {
            _originalPos = percentageText.rectTransform.localPosition;
        }

        private void OnDisable()
        {
            if (_animateCoroutine != null)
            {
                StopCoroutine(_animateCoroutine);
                _animateCoroutine = null;
            }

            percentageText.text = "0%";
            percentageText.color = Color.white;
            percentageText.rectTransform.localPosition = _originalPos;
            _currentValue = 0f;
        }

        public void SetTarget(float newTarget)
        {
            _targetValue = Mathf.Clamp(newTarget, 0f, 100f);
            
            if(Mathf.Approximately(_currentValue, _targetValue))
                return;

            if (_animateCoroutine != null)
            {
                StopCoroutine(_animateCoroutine);
                _animateCoroutine = null;
            }

            _animateCoroutine = StartCoroutine(AnimateRoutine());
        }

        private System.Collections.IEnumerator AnimateRoutine()
        {
            var flashPhase = 0f;
            var shakePhase = 0f;

            while (!Mathf.Approximately(_currentValue, _targetValue))
            {
                var delta = _targetValue - _currentValue;
                var direction = Mathf.Sign(delta);
                var distance = Mathf.Abs(delta);
                var speed = baseSpeed / (1f + distance / slowdownRange);
                var step = speed * Time.deltaTime;

                if (distance < step)
                    _currentValue = _targetValue;
                else
                    _currentValue += step * direction;

                var displayValue = Mathf.RoundToInt(_currentValue);
                percentageText.text = $"{displayValue}%";

                var normalized = _currentValue / 100f;
                
                //МИГАНИЕ
                if (normalized >= 0.05f)
                {
                    flashPhase += Time.deltaTime * 8f;
                    var intensity = Mathf.Abs(Mathf.Sin(flashPhase));
                    percentageText.color = Color.Lerp(normalColor, highChanceColor, intensity);
                }
                else
                {
                    percentageText.color = normalColor;
                    flashPhase = 0f;
                }
                
                //ДРОЖАНИЕ
                if (normalized <= 0.15f)
                {
                    shakePhase += Time.deltaTime * 30f;
                    var shakeAmount = Mathf.Lerp(0.2f, 1.5f, 1f - normalized * 7f);
                    var shakeX = Mathf.Sin(shakePhase) * shakeAmount;
                    var shakeY = Mathf.Cos(shakePhase * 0.5f) * shakeAmount;
                    percentageText.rectTransform.localPosition = _originalPos + new Vector3(shakeX, shakeY, 0f);
                }
                else
                {
                    percentageText.rectTransform.localPosition = _originalPos;
                    shakePhase = 0f;
                }

                yield return _wait;
            }

            percentageText.text = $"{Mathf.RoundToInt(_targetValue)}%";
            percentageText.color = normalColor;
            percentageText.rectTransform.localPosition = _originalPos;

            _animateCoroutine = null;
        }
    }
}