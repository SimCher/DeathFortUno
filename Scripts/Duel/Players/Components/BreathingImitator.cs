using DeathFortUnoCard.Scripts.Common.Interfaces;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.Duel.Players.Components
{
    public class BreathingImitator : MonoBehaviour, IEnable
    {
        [Header("Параметры дыхания")]
        [SerializeField] private float frequency = 0.5f;
        [SerializeField] private float height = 0.3f;
        [SerializeField] private Vector3 breathingAxis = new(0f, 1f, 0f);

        private Transform _transform;
        
        private Vector3 _initPos;
        private float _time;

        private Coroutine _breathingCoroutine;

        [field: SerializeField] public bool IsEnabled { get; private set; } = true;

        private void Awake()
        {
            _transform = transform;
        }

        private void Start()
        {
            _initPos = _transform.localPosition;
            Disable();
        }

        private void StopCoroutine()
        {
            if (_breathingCoroutine != null)
            {
                StopCoroutine(_breathingCoroutine);
                _breathingCoroutine = null;
            }
        }

        private void StartCoroutine()
        {
            StopCoroutine();

            _breathingCoroutine = StartCoroutine(BreathingRoutine());
        }

        private System.Collections.IEnumerator BreathingRoutine()
        {
            while (IsEnabled)
            {
                _time += Time.deltaTime;
                var progress = Mathf.Sin(_time * Mathf.PI * 2 * frequency) * height;

                _transform.localPosition += breathingAxis * progress;
                yield return null;
            }
        }

        public void Enable()
        {
            if (IsEnabled)
                return;
            IsEnabled = true;
            StartCoroutine();
        }

        public void Disable()
        {
            if (!IsEnabled)
                return;
            IsEnabled = false;
            StopCoroutine();
            _transform.localPosition = _initPos;
        }
    }
}