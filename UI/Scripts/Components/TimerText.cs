using System;
using TMPro;
using UnityEngine;

namespace DeathFortUnoCard.UI.Scripts.Components
{
    [RequireComponent(typeof(TMP_Text), typeof(Animator))]
    public class TimerText : MonoBehaviour
    {
        private TMP_Text _timeText;
        private TimerTextAnimator _animator;
        private TimeSpan _t;

        private void Awake()
        {
            if (!TryGetComponent(out _timeText))
            {
                Debug.LogError($"Текстовый компонент не найден на GameObject {name}");
                enabled = false;
                return;
            }

            _animator = new TimerTextAnimator(GetComponent<Animator>());
        }

        private void SetText(TimeSpan time) =>
            _timeText.text = $"{time.Minutes:D2}:{time.Seconds:D2}";

        private void EqualTexts(string text)
        {
            if(!_timeText.text.Equals(text))
                _animator.RegularBlit();
        }

        public void UpdateText(int sec, int maxSec) => UpdateText((float)sec, maxSec);

        public void UpdateText(float sec, float maxSec)
        {
            if (maxSec <= 0)
            {
                Debug.LogWarning($"{nameof(maxSec)} должен быть больше нуля!");
                return;
            }

            var percentage = sec / maxSec * 100f;

            switch (percentage)
            {
                case <= 50f and > 25f:
                    _animator.WarningBlit();
                    break;
                case <= 25f:
                    _animator.DangerBlit();
                    break;
                default:
                    _animator.RegularBlit();
                    break;
            }

            _t = TimeSpan.FromSeconds(sec);
            SetText(_t);
        }
    }

    internal class TimerTextAnimator
    {
        private readonly Animator _animator;
        private static readonly int RegularBlitTrigger = Animator.StringToHash("RegularBlit");
        private static readonly int WarningBlitTrigger = Animator.StringToHash("WarningBlit");
        private static readonly int DangerBlitTrigger = Animator.StringToHash("DangerBlit");

        public TimerTextAnimator(Animator animator)
        {
            _animator = animator;
        }

        public void RegularBlit()
        {
            _animator.ResetTrigger(DangerBlitTrigger);
            _animator.ResetTrigger(WarningBlitTrigger);
            _animator.SetTrigger(RegularBlitTrigger);
        }

        public void WarningBlit()
        {
            _animator.ResetTrigger(DangerBlitTrigger);
            _animator.ResetTrigger(RegularBlitTrigger);
            _animator.SetTrigger(WarningBlitTrigger);
        }

        public void DangerBlit()
        {
            _animator.ResetTrigger(RegularBlitTrigger);
            _animator.ResetTrigger(WarningBlitTrigger);
            _animator.SetTrigger(DangerBlitTrigger);
        }
    }
}