using System;
using DeathFortUnoCard.Scripts.Common.Players;
using DeathFortUnoCard.Scripts.Common.Players.Agent;
using DeathFortUnoCard.Scripts.Duel.Objects;
using DeathFortUnoCard.Scripts.Utils;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace DeathFortUnoCard.Scripts.Duel.Players.Shooters
{
    [RequireComponent(typeof(Timer))]
    public class NPCShooter : Shooter
    {
        [Header("Настройки")]
        [SerializeField] private float aimSpeed = 1f;

        [SerializeField] private float baseAimError = 0.3f;

        [SerializeField] private float shootAccuracyThreshold = 5f;

        [SerializeField] private MinMax waitRange;

        [Header("Сложность (0 - мажет, 10 - почти не мажет)")]
        [Range(0, 10)] [SerializeField] private int difficulty = 5;

        [Min(1f)] [SerializeField] private int maxDifficulty = 10;
        
        private Timer _timer;
        private Transform _targetPlayer;
        private Transform _gunTransform;
        private Coroutine _aimCoroutine;
        private Vector3 _aimErrorVector;
        private Quaternion _targetRotation;
        
        public UnityEvent onShoot;

        private void Awake()
        {
            if (!TryGetComponent(out _timer))
            {
                Debug.LogError($"Не смог найти компонент {nameof(Timer)} на объекте {name}");
                enabled = false;
            }
        }

        private void OnDestroy()
        {
            onShoot.RemoveAllListeners();
        }

        private Vector3 GenerateAimError()
        {
            //Чем выше сложность, тем меньше допустимая ошибка
            var errorFactor = Mathf.Lerp(1f, 0f, difficulty / maxDifficulty);
            var aimError = baseAimError * errorFactor;
            
            var errorX = Random.Range(-aimError, aimError);
            var errorY = Random.Range(-aimError, aimError);
            return new Vector3(errorX, errorY, 0f);
        }

        private bool ShouldMissShot()
        {
            //Чем выше сложность, тем меньше шанс промаха
            var missChance = Mathf.Lerp(0.95f, 0.05f, difficulty / maxDifficulty);
            return Random.value < missChance;
        }

        private System.Collections.IEnumerator AimShootRoutine()
        {
            _aimErrorVector = GenerateAimError();

            while (_targetPlayer && gun)
            {
                var directionToPlayer = (_targetPlayer.position - _gunTransform.position).normalized;
                var inaccurateDirection =
                    (directionToPlayer + _gunTransform.TransformDirection(_aimErrorVector)).normalized;
                _targetRotation = Quaternion.LookRotation(inaccurateDirection);

                _gunTransform.rotation = Quaternion.Slerp(
                    _gunTransform.rotation,
                    _targetRotation,
                    aimSpeed * Time.deltaTime);

                var angleDiff = Quaternion.Angle(_gunTransform.rotation, _targetRotation);

                if (angleDiff <= shootAccuracyThreshold)
                {
                    yield break;
                }

                yield return null;
            }
        }

        public void EvaluateDifficulty(AIDifficulty diff)
        {
            difficulty = diff switch
            {
                AIDifficulty.Easy => Random.Range(0, 2),
                AIDifficulty.Medium => Random.Range(3, 5),
                AIDifficulty.Hard => Random.Range(6, 8),
                AIDifficulty.Impossible => Random.Range(9, maxDifficulty + 1),
                _ => difficulty
            };
        }

        public override void SetGun(Gun assignGun)
        {
            base.SetGun(assignGun);
            _gunTransform = assignGun.transform;
        }

        public void SetTargetPlayer(Player newTargetPlayer)
        {
            _targetPlayer = newTargetPlayer ? newTargetPlayer.transform : null;
            
            if(_aimCoroutine != null)
                StopCoroutine(_aimCoroutine);

            if (_targetPlayer && gun)
            {
                _aimCoroutine = StartCoroutine(AimShootRoutine());
                _timer.StartTimer(waitRange.min, waitRange.max);
            }
        }

        public void Shoot()
        {
            if (!gun)
                return;

            if (ShouldMissShot())
            {
                _aimErrorVector = GenerateAimError();
            }
            else
            {
                _aimErrorVector = Vector3.zero;
            }

            if (_targetPlayer)
            {
                var dirToPlayer = (_targetPlayer.position - _gunTransform.position).normalized;
                var inaccurateDir = (dirToPlayer + _gunTransform.TransformDirection(_aimErrorVector)).normalized;
                _gunTransform.rotation = Quaternion.LookRotation(inaccurateDir);
            }
            
            onShoot?.Invoke();
        }
    }
}