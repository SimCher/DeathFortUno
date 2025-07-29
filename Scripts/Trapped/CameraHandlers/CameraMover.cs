using System.Collections;
using DeathFortUnoCard.Scripts.Trapped.Effects;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.Trapped.CameraHandlers
{
    public class CameraMover : MonoBehaviour
    {
        [SerializeField] private Camera devilTableCamera;
        [SerializeField] private int step;
        [SerializeField] private int stepMultiplier;
        [SerializeField] private Vector3[] targetPositions;

        private VHSCameraShake _cameraShake;

        private Vector3 _cameraOriginalPosition;

        private Coroutine _moveCameraCoroutine;

        private void Start()
        {
            _cameraShake = devilTableCamera.GetComponent<VHSCameraShake>();

            _cameraOriginalPosition = devilTableCamera.transform.localPosition;
        }

        private void OnEnable()
        {
            devilTableCamera.transform.localPosition = _cameraOriginalPosition;
        }

        private void OnDestroy()
        {
            if(_moveCameraCoroutine != null)
                StopCoroutine(_moveCameraCoroutine);
        }
        
        private void LaunchCoroutine(Vector3 targetPosition)
        {
            if (_moveCameraCoroutine != null)
            {
                StopCoroutine(_moveCameraCoroutine);
            }

            _moveCameraCoroutine = StartCoroutine(MoveRoutine(targetPosition));
        }

        private bool IsReached(Vector3 current, Vector3 target, float threshold)
            => Vector3.Distance(current, target) > threshold;

        private IEnumerator MoveRoutine(Vector3 targetPos)
        {
            var currentPos = devilTableCamera.transform.localPosition;
            var currentStep = step * stepMultiplier;

            while (IsReached(currentPos, targetPos, 0.05f))
            {
                currentPos = Vector3.Lerp(currentPos, targetPos, currentStep * Time.deltaTime);
                devilTableCamera.transform.localPosition = currentPos;
                yield return null;
            }

            devilTableCamera.transform.localPosition = targetPos;
        }

        public void MoveToOriginal() => LaunchCoroutine(_cameraOriginalPosition);

        public void StartMoving(int targetPosIndex) => LaunchCoroutine(targetPositions[targetPosIndex]);
    }
}