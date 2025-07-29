using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using UnityEngine;
using UnityEngine.Events;

namespace DeathFortUnoCard.Scripts.Duel.Effects
{
    public class CameraShaker : MonoBehaviour, IService
    {
        [SerializeField] private float transitionDuration = 10f;

        private Transform _transform;
        
        private Vector3 _initialRotation;
        private Vector3 _targetRotation;
        private float _elapsed;
        private bool _isTransiting;

        private readonly Vector3 _startRotation = new(45f, 45f, 0f);

        private Coroutine _transitionCoroutine;

        public UnityAction OnShakeOver;

        private void Awake()
        {
            ServiceLocator.Register(this);
            _transform = transform;
        }

        private void OnDisable()
        {
            var damageHandler = ServiceLocator.Get<DamageEffectHandler>();

            if (damageHandler)
            {
                OnShakeOver -= damageHandler.Stop;
                damageHandler.CamShaker = null;
            }
        }

        private void OnDestroy()
        {
            if(_transitionCoroutine != null)
                StopCoroutine(_transitionCoroutine);

            OnShakeOver = null;
        }

        private System.Collections.IEnumerator TransitionRoutine()
        {
            while (_isTransiting)
            {
                _elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(_elapsed / transitionDuration);

                var currentRotation = Vector3.Lerp(_initialRotation, _targetRotation, t);
                _transform.localRotation = Quaternion.Euler(currentRotation);

                if (t >= 1f)
                {
                    _isTransiting = false;
                    OnShakeOver?.Invoke();
                }

                yield return null;
            }
        }

        [ContextMenu(nameof(StartTransition))]
        public void StartTransition()
        {
            if (_isTransiting)
                return;

            _isTransiting = true;
            _elapsed = 0f;

            _initialRotation = _startRotation;
            _transform.localRotation = Quaternion.Euler(_initialRotation);
            _targetRotation = Vector3.zero;
            
            if(_transitionCoroutine != null)
                StopCoroutine(_transitionCoroutine);

            _transitionCoroutine = StartCoroutine(TransitionRoutine());
        }
    }
}