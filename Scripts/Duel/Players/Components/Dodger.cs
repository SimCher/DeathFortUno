using System;
using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.Duel.Players.Components
{
    public class Dodger : MonoBehaviour, IService
    {
        private enum DodgeState
        {
            Idle,
            Dodging,
            Returning
        }

        private enum DodgeDirection
        {
            Center,
            Left,
            Right
        }

        [Header("Общие настройки")]
        [SerializeField, Tooltip("Расстояние уклонения")]
        private float dodgeDistance = 2f;

        [SerializeField, Tooltip("Скорость уклонения")]
        private float dodgeSpeed = 10f;

        [Header("Настройки NPC")]
        [SerializeField] private Transform playerTransform;
        
        [SerializeField] private DodgeState currentState = DodgeState.Idle;
        [SerializeField] private DodgeDirection currentDir = DodgeDirection.Center;

        private Vector3 _initPos;
        private Quaternion _initRot;

        private Vector3 _targetPos;
        private Quaternion _targetRot;

        private Coroutine _dodgeCoroutine;
        
        private Vector3 LeftDir => -playerTransform.right;
        private Vector3 RightDir => playerTransform.right;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void OnDisable()
        {
            if (_dodgeCoroutine != null)
            {
                StopCoroutine(_dodgeCoroutine);
                _dodgeCoroutine = null;
            }
        }

        private System.Collections.IEnumerator DodgeRoutine()
        {
            while (true)
            {
                if (currentState == DodgeState.Idle)
                    yield break;

                playerTransform.position =
                    Vector3.MoveTowards(playerTransform.position, _targetPos, dodgeSpeed * Time.deltaTime);

                playerTransform.rotation = Quaternion.RotateTowards(playerTransform.rotation, _targetRot,
                    dodgeSpeed * 100f * Time.deltaTime);

                if (HasReachedPosition(playerTransform.position, _targetPos) &&
                    HasReachedRotation(playerTransform.rotation, _targetRot))
                {
                    if (currentState == DodgeState.Returning)
                    {
                        currentState = DodgeState.Idle;
                        playerTransform.position = _initPos;
                        playerTransform.rotation = _initRot;
                    }
                    
                    yield break;
                }

                yield return null;
            }
        }

        private bool HasReachedPosition(Vector3 current, Vector3 target)
            => (current - target).sqrMagnitude <= 0.0001f;

        private bool HasReachedRotation(Quaternion current, Quaternion target)
            => Quaternion.Angle(current, target) <= 0.1f;

        private void StartDodge(Vector3 direction)
        {
            currentState = DodgeState.Dodging;
            _targetPos = _initPos + direction * dodgeDistance;
            _targetRot = Quaternion.LookRotation(-direction);
        }

        private void Clear()
        {
            playerTransform = null;
            _initPos = Vector3.zero;
            _initRot = Quaternion.identity;
            _targetPos = Vector3.zero;
            _targetRot = Quaternion.identity;

            currentDir = DodgeDirection.Center;
            currentState = DodgeState.Idle;

            if (_dodgeCoroutine != null)
            {
                StopCoroutine(_dodgeCoroutine);
                _dodgeCoroutine = null;
            }
        }

        public void StartDodge()
        {
            if(_dodgeCoroutine != null)
                StopCoroutine(_dodgeCoroutine);
            
            switch (currentDir)
            {
                case DodgeDirection.Left:
                    StartDodge(LeftDir);
                    break;
                case DodgeDirection.Right:
                    StartDodge(RightDir);
                    break;
                case DodgeDirection.Center:
                    StartReturn();
                    return;
            }

            _dodgeCoroutine = StartCoroutine(DodgeRoutine());
        }

        public void StartReturn()
        {
            if(_dodgeCoroutine != null)
                StopCoroutine(_dodgeCoroutine);

            currentState = DodgeState.Returning;
            _targetPos = _initPos;
            _targetRot = _initRot;

            _dodgeCoroutine = StartCoroutine(DodgeRoutine());
        }

        public void ToLeft() => currentDir = DodgeDirection.Left;

        public void ToRight() => currentDir = DodgeDirection.Right;

        public void Return() => currentDir = DodgeDirection.Center;

        public void SetPlayer(Transform newPlayerTransform)
        {
            Clear();

            playerTransform = newPlayerTransform;

            _initPos = newPlayerTransform.position;
            _initRot = newPlayerTransform.rotation;

            _targetPos = _initPos;
            _targetRot = _initRot;
        }
    }
}