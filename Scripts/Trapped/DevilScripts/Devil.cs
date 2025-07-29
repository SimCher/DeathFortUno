using DeathFortUnoCard.Scripts.Trapped.Services;
using UnityEngine;
using UnityEngine.Events;

namespace DeathFortUnoCard.Scripts.Trapped.DevilScripts
{
    [RequireComponent(typeof(Animator))]
    public class Devil : MonoBehaviour
    {
        private DevilAnimatorService _animator;

        public UnityEvent onKnock;
        public UnityEvent onDiceIsInGlass;
        public UnityEvent onFinish;

        private bool _isInitialized;

        private void OnEnable()
        {
            if (_isInitialized)
                return;

            _animator ??= new DevilAnimatorService(GetComponent<Animator>());

            _isInitialized = true;
        }

        private void OnDestroy()
        {
            onKnock.RemoveAllListeners();
            onDiceIsInGlass.RemoveAllListeners();
            onFinish.RemoveAllListeners();
        }

        public void SetDices() => _animator.SetDices();

        public void LuckyDigitSelected() => _animator.Knock();

        [ContextMenu(nameof(HideDicesInHand))]
        public void HideDicesInHand() => onDiceIsInGlass?.Invoke();

        public void PrepareToShake() => _animator.PrepareToShake();

        public void TakeGlass() => _animator.TakeGlass();

        public void Shake() => _animator.Shake();

        public void Finish()
        {
            Debug.Log("<color=purple>Вызван onFinished</color>");
            _animator.Exit();
        }

        public void Finished() => onFinish?.Invoke();

        public void Knocked() => onKnock?.Invoke();
    }
}