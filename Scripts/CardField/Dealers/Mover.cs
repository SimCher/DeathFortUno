using UnityEngine;
using UnityEngine.Events;

namespace DeathFortUnoCard.Scripts.CardField.Dealers
{
    public class Mover : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Настройки")]
        [SerializeField] private float duration = 0.5f;

        [Header("Ссылки")]
        [SerializeField] private Transform startPoint;
        [SerializeField] private Transform endPoint;

        [Header("События")]
        public UnityEvent<Transform, GameObject> onMoveFinished;

        #endregion

        #region Private Fields

        private GameObject _currentObject;
        private Coroutine _moveCoroutine;

        private bool _isMoving;

        #endregion

        #region Unity Events

        private void OnDestroy()
        {
            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
                _moveCoroutine = null;
            }
            
            onMoveFinished.RemoveAllListeners();
        }

        #endregion

        #region Private Methods

        private System.Collections.IEnumerator MoveToPosition()
        {
            var elapsed = 0f;
            var startPos = startPoint.position;
            var targetPos = endPoint.position;

            while (elapsed < duration)
            {
                _currentObject.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            FinishMoving();
        }

        private void FinishMoving()
        {
            _currentObject.transform.position = endPoint.position;
            onMoveFinished?.Invoke(endPoint, _currentObject);
            
            _isMoving = false;
        }

        #endregion

        #region Public Methods

        public void Move()
        {
            if (!endPoint)
            {
                Debug.LogError($"Попытка присвоить null для {nameof(_currentObject)}");
                return;
            }
            
            if (_isMoving && _currentObject)
            {
                StopCoroutine(_moveCoroutine);
                FinishMoving();
            }

            if (!_currentObject)
                _currentObject = gameObject;
            
            _currentObject.SetActive(true);
            startPoint = _currentObject.transform;

            _moveCoroutine = StartCoroutine(MoveToPosition());
        }

        public void MoveToTarget(GameObject objToMove)
        {
            if (!objToMove)
            {
                Debug.LogError($"Попытка присвоить null для {nameof(_currentObject)}");
                return;
            }

            _currentObject = objToMove;
            
            _currentObject.SetActive(true);
            _currentObject.transform.position = startPoint.position;

            _moveCoroutine = StartCoroutine(MoveToPosition());
        }

        public void MoveToTarget(Transform destination, GameObject objToMove)
        {
            endPoint = destination;
            
            MoveToTarget(objToMove);
        }

        #endregion
    }
}