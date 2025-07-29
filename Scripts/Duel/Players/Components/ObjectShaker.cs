using DeathFortUnoCard.Scripts.Common.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DeathFortUnoCard.Scripts.Duel.Players.Components
{
    public class ObjectShaker : MonoBehaviour, IEnable
    {
        [Header("Параметры тряски")]
        [SerializeField] private float shakeAmplitude = 1f;
        [SerializeField] private float shakeFrequency = 0.5f;
        [SerializeField] private float shakeRandomIntensity = 0.03f;
        [SerializeField] private Vector3 shakeAxis = new(1f, 1f, 0f);

        private Vector3 _initPos;
        private Vector3 _randomOffset;

        [field: SerializeField] public bool IsEnabled { get; private set; } = true;

        private Coroutine _shakeCoroutine;
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        private void Start()
        {
            _initPos = _transform.localPosition;
            Disable();
        }

        private System.Collections.IEnumerator ShakeRoutine()
        {
            while (IsEnabled)
            {
                var shakeX = Mathf.Sin(Time.time * shakeFrequency) * shakeAmplitude * shakeAxis.x;
                var shakeY = Mathf.Cos(Time.time * shakeFrequency) * shakeAmplitude * shakeAxis.y;
                var shakeZ = Mathf.Sin(Time.time * shakeFrequency * 0.7f) * shakeAmplitude * shakeAxis.z;

                _randomOffset.x = Random.Range(-shakeRandomIntensity, shakeRandomIntensity);
                _randomOffset.y = Random.Range(-shakeRandomIntensity, shakeRandomIntensity);
                _randomOffset.z = Random.Range(-shakeRandomIntensity, shakeRandomIntensity);

                _transform.localPosition = _initPos + new Vector3(shakeX, shakeY, shakeZ) + _randomOffset;
                yield return null;
            }
        }

        private void StopCoroutine()
        {
            if(_shakeCoroutine != null)
                StopCoroutine(_shakeCoroutine);
        }

        private void StartCoroutine()
        {
            StopCoroutine();

            _shakeCoroutine = StartCoroutine(ShakeRoutine());
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