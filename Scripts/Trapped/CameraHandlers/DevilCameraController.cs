using DeathFortUnoCard.Scripts.Trapped.Effects;
using UnityEngine;
using UnityEngine.Events;

namespace DeathFortUnoCard.Scripts.Trapped.CameraHandlers
{
    public class DevilCameraController : MonoBehaviour
    {
        [SerializeField] private Camera devilTableCamera;
        [SerializeField] private VHSCameraShake cameraShake;
        [SerializeField, Range(0, 90)] private int startCameraField = 80;

        [Header("Обшие настройки")]
        [SerializeField, Range(1, 30)] private int step;

        [SerializeField, Range(1, 10)] private int stepMultiplier;

        public CameraEventSettings[] events;

        [Header("Настройки удара дьявола")]
        [SerializeField] private float knockShakeCameraAmount;
        [SerializeField] private float knockShakeCameraSpeed;
        [SerializeField] private float knockShakeDuration;

        public UnityEvent onKnockOver;

        private Coroutine _changeFoVCoroutine;

        private void Start()
        {
            devilTableCamera.fieldOfView = startCameraField;
        }

        private void OnDestroy()
        {
            if(_changeFoVCoroutine != null)
                StopCoroutine(_changeFoVCoroutine);
            
            onKnockOver.RemoveAllListeners();

            for (int i = 0; i < events.Length; i++)
            {
                events[i].action.RemoveAllListeners();
            }
        }

        private void StartCoroutine(float target = 0f, UnityEvent e = null, float anotherTarget = 0f)
        {
            if (_changeFoVCoroutine != null)
            {
                StopCoroutine(_changeFoVCoroutine);
                _changeFoVCoroutine = null;
            }

            if (Mathf.Approximately(target, 0f) && e == null && Mathf.Approximately(anotherTarget, 0f))
            {
                StopAllCoroutines();
                _changeFoVCoroutine = base.StartCoroutine(ShakeCameraRoutine());
                return;
            }

            StopAllCoroutines();
            _changeFoVCoroutine = base.StartCoroutine(ChangeFoVRoutine(target, e, anotherTarget));
        }

        private System.Collections.IEnumerator ShakeCameraRoutine()
        {
            var originShakeAmount = cameraShake.Amplitude;
            var originShakeSpeed = cameraShake.Speed;
            var duration = knockShakeDuration;
            var elapsed = 0f;

            while (elapsed < knockShakeDuration)
            {
                cameraShake.Amplitude = Mathf.Lerp(knockShakeCameraAmount, originShakeAmount, elapsed / duration);
                cameraShake.Speed = Mathf.Lerp(knockShakeCameraSpeed, originShakeSpeed, elapsed);

                elapsed += Time.deltaTime;

                yield return null;
            }

            cameraShake.Amplitude = originShakeAmount;
            cameraShake.Speed = originShakeSpeed;
            
            onKnockOver?.Invoke();

            SetShowCameraField(1);
        }

        private System.Collections.IEnumerator ChangeFoVRoutine(float target, UnityEvent e, float second = 0f)
        {
            const float epsilon = 1f;
            var currentFoV = devilTableCamera.fieldOfView;
            var doubleStep = step * stepMultiplier;

            while (Mathf.Abs(currentFoV - target) > epsilon)
            {
                currentFoV = Mathf.Lerp(currentFoV, target, doubleStep * Time.deltaTime);
                devilTableCamera.fieldOfView = currentFoV;
                yield return null;
            }

            devilTableCamera.fieldOfView = target;

            if (Mathf.Approximately(second, 0f))
            {
                e?.Invoke();
                yield break;
            }

            while (Mathf.Abs(currentFoV - second) > epsilon)
            {
                currentFoV = Mathf.Lerp(currentFoV, second, step * 3 * Time.deltaTime);
                devilTableCamera.fieldOfView = currentFoV;
                yield return null;
            }

            devilTableCamera.fieldOfView = second;
            e?.Invoke();
        }

        public void SetKnockedSettings() => StartCoroutine();

        public void SetOriginal() => devilTableCamera.fieldOfView = startCameraField;

        [ContextMenu("Установить показ камеры поля")]
        public void SetShowCameraField(int id)
            => StartCoroutine(events[id].cameraFoVStart, events[id].action, events[id].cameraFoVFinish);
    }
}