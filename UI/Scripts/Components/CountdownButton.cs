using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DeathFortUnoCard.UI.Scripts.Components
{
    /// <summary>
    /// UI-кнопка с запуском обратного отсчёта
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class CountdownButton : MonoBehaviour
    {
        [Header("Ссылки")]
        [SerializeField] private TMP_Text label;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Обратный отсчёт")]
        [SerializeField, Range(1, 60)] private int countdownSeconds = 5;
        [SerializeField] private string baseText = "Accept";

        [Header("Активация")]
        [SerializeField] private bool isVisible;

        private Button _button;
        private Coroutine _countdownCoroutine;
        private WaitForSeconds _countdownDelay;
        private Action _onClickCallback;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnManualClick);

            if (!label)
            {
                Debug.LogError($"{nameof(CountdownButton)}: ({nameof(TMP_Text)} не назначен.)", this);
                enabled = false;
                return;
            }

            if (!canvasGroup)
            {
                Debug.LogError($"{nameof(CountdownButton)}: ({nameof(CanvasGroup)} не назначен.)", this);
                enabled = false;
                return;
            }

            _countdownDelay = new WaitForSeconds(1f);

            if (isVisible)
                ShowAndStartCountdown();
            else
                SetVisible(false);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        private System.Collections.IEnumerator CountdownRoutine()
        {
            var remaining = countdownSeconds;

            while (remaining > 0)
            {
                SetLabel($"{baseText} ({remaining})");
                yield return _countdownDelay;
                remaining--;
            }
            
            SetLabel(baseText);
            _button.onClick.Invoke();
            _countdownCoroutine = null;
        }

        private void SetLabel(string text)
        {
            label.text = text;
        }

        private void SetVisible(bool visible)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.blocksRaycasts = visible;
            canvasGroup.interactable = visible;
            _button.interactable = visible;
        }

        private void StopCountdown()
        {
            if (_countdownCoroutine != null)
            {
                StopCoroutine(_countdownCoroutine);
                _countdownCoroutine = null;
            }
        }

        private void OnManualClick()
        {
            _onClickCallback?.Invoke();
            StopCountdown();
        }

        /// <summary>
        /// Показывает кнопку, запускает обратный отсчёт и настраивает колбэк при нажатии
        /// </summary>
        public void ShowAndStartCountdown(Action onClick = null)
        {
            _onClickCallback = onClick;
            SetVisible(true);
            StopCountdown();
            _countdownCoroutine = StartCoroutine(CountdownRoutine());
        }

        /// <summary>
        /// Полное скрытие кнопки
        /// </summary>
        public void Hide()
        {
            StopCountdown();
            SetVisible(false);
        }
    }
}