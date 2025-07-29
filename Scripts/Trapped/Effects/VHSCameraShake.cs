using UnityEngine;
using Random = UnityEngine.Random;

namespace DeathFortUnoCard.Scripts.Trapped.Effects
{
    public class VHSCameraShake : MonoBehaviour
    {
        [SerializeField, Range(0.01f, 0.1f)] private float updateInterval = 0.033f;

        private Transform _cameraTransform;
        private Vector3 _initialRotation;
        private float _timeOffset;

        private Coroutine _shakeCoroutine;
        private WaitForSeconds _shakeDelay;

        [field: SerializeField, Range(0f, 5f)]
        public float Amplitude { get; set; }
        
        [field: SerializeField, Range(0f, 3f)]
        public float Speed { get; set; }

        private void Awake()
        {
            _cameraTransform = transform;
            _initialRotation = _cameraTransform.localEulerAngles;
            _timeOffset = Random.value * 10f;

            _shakeDelay = new WaitForSeconds(updateInterval);
        }

        private void OnEnable()
        {
            if(_shakeCoroutine != null)
                StopCoroutine(_shakeCoroutine);

            _shakeCoroutine = StartCoroutine(ShakeRoutine());
        }

        private void OnDisable()
        {
            if(_shakeCoroutine != null)
                StopCoroutine(_shakeCoroutine);
        }

        private float GetShakeValue(float x, float y)
            => (Mathf.PerlinNoise(x, y) * 2f - 1f) * Amplitude;

        private System.Collections.IEnumerator ShakeRoutine()
        {
            while (true)
            {
                var time = Time.time * Speed + _timeOffset;

                var xShake = GetShakeValue(time, 0f);
                var yShake = GetShakeValue(0f, time);
                var zShake = GetShakeValue(time, time);

                _cameraTransform.localEulerAngles = _initialRotation + new Vector3(xShake, yShake, zShake);

                yield return _shakeDelay;
            }
        }
    }
}