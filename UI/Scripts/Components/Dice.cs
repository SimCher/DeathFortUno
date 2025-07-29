using System;
using System.Collections;
using DeathFortUnoCard.Scripts.CardField.Dealers;
using DeathFortUnoCard.Scripts.Common.Effects;
using UnityEngine;
using UnityEngine.Events;

namespace DeathFortUnoCard.UI.Scripts.Components
{
    [RequireComponent(typeof(Renderer), typeof(Mover), typeof(GlowAnimator))]
    public class Dice : MonoBehaviour
    {
        [SerializeField, Range(1, 6)] private int value;
        [SerializeField] private float targetY = 1.5f;
        [SerializeField] private float duration = 0.5f;

        [SerializeField] private float flyTargetY = 5f;
        [SerializeField] private float flyDuration = 0.1f;

        private bool _isEnabled;

        private float _originalY;

        private Mover _mover;

        private Vector3 _originalPosition;

        private Coroutine _moveToYCoroutine;

        private Transform _transform;
        private GlowAnimator _glow;

        public UnityAction<Dice> OnSelect;

        public int Value => value;

        private void Awake()
        {
            _mover = GetComponent<Mover>();
            _glow = GetComponent<GlowAnimator>();
        }

        private void OnDestroy()
        {
            if(_moveToYCoroutine != null)
                StopCoroutine(_moveToYCoroutine);
        }

        private void StartCoroutine(float y, bool isFly = false)
        {
            gameObject.SetActive(true);
            
            if(_moveToYCoroutine != null)
                StopCoroutine(_moveToYCoroutine);

            _moveToYCoroutine = StartCoroutine(MoveToY(y, isFly));
        }

        private IEnumerator MoveToY(float y, bool isFly = false)
        {
            var currentDuration = isFly ? flyDuration : duration;
            var elapsed = 0f;
            var startPos = _transform.position;
            var targetPos = new Vector3(startPos.x, y, startPos.z);

            while (elapsed < duration)
            {
                _transform.position = Vector3.Lerp(startPos, targetPos, elapsed / currentDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            _transform.position = targetPos;
        }

        private bool Equals(Dice other)
            => other && value == other.value && GetInstanceID() == other.GetInstanceID();

        public void Initialize()
        {
            if (!_transform)
                _transform = transform;
            _originalPosition = _transform.position;
            _originalY = _originalPosition.y;
        }

        public void ChangeActivity(bool state)
        {
            if (!state)
                Deselect();

            _isEnabled = state;
        }

        public void Move() => _mover.Move();

        public void Enable() => gameObject.SetActive(true);

        public void Select()
        {
            Enable();
            _glow.ActivateGlow();
            StartCoroutine(targetY);
            OnSelect?.Invoke(this);
        }

        public void Deselect()
        {
            Enable();
            _glow.DeactivateGlow();
            StartCoroutine(_originalY);
        }

        public void SetStart()
        {
            ChangeActivity(false);
            transform.position = _originalPosition;
            gameObject.SetActive(false);
        }

        public override bool Equals(object other) => Equals(other as Dice);

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(base.GetHashCode());
            hashCode.Add(targetY);
            hashCode.Add(duration);
            hashCode.Add(flyTargetY);
            hashCode.Add(flyDuration);
            hashCode.Add(_isEnabled);
            hashCode.Add(_originalY);
            hashCode.Add(_mover);
            hashCode.Add(_originalPosition);
            hashCode.Add(_moveToYCoroutine);
            hashCode.Add(Value);
            return hashCode.ToHashCode();
        }
    }
}