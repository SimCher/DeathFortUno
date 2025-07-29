using System;
using System.Collections;
using DeathFortUnoCard.Scripts.Duel.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace DeathFortUnoCard.Scripts.Duel.Objects
{
    [RequireComponent(typeof(AudioSource))]
    public class Gun : MonoBehaviour
    {
#if UNITY_EDITOR
        [Header("Отладка")]
        [SerializeField] private float rayLength = 100f;
#endif

        [Header("Общие настройки")]
        [SerializeField, Range(1, 6)] private int maxAmmo = 6;
        [SerializeField] private float fireRate = 0.5f;
        [SerializeField, Range(0f, 1f)] private float misfireChance = 0.2f;
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private float maxDistance = 100f;
        [SerializeField] private float bulletSpeed = 200f;
        [SerializeField] private float resultDelay = 2f;

        [Header("Настройки отдачи")]
        [SerializeField] private float recoilForce = 0.5f;
        [SerializeField] private float recoilRecoverySpeed = 5f;

        [Header("Настройки барабана")]
        [SerializeField] private bool[] chambers;

        [SerializeField] private int chamberIndex;

        [Header("Ссылки")]
        [SerializeField] private Transform muzzleTransform;
        [SerializeField] private ParticleSystem muzzleFlash;

        [Header("Аудио")]
        [SerializeField] private AudioClip fireSfx;
        [SerializeField] private AudioClip reloadSfx;
        [SerializeField] private AudioClip emptySfx;

        private AudioSource _audioSource;
        
        public UnityEvent onNoShoot;

        private Transform _transform;
        private Vector3 _initPos;

        public bool CanShoot { get; set; } = true;

        private float _nextFireTime;
        
        private Coroutine _recoilCoroutine;
        private WaitForSeconds _resultDelay;

        private void Awake()
        {
            _resultDelay = new WaitForSeconds(resultDelay);
        }

        private void Start()
        {
            _transform = transform;
            _initPos = _transform.localPosition;
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            Disarm();
        }

        private void OnDestroy()
        {
            onNoShoot.RemoveAllListeners();
        }

        [ContextMenu(nameof(ToOriginalPosition))]
        private void ToOriginalPosition() => _transform.localPosition = _initPos;

        [ContextMenu(nameof(Disarm))]
        private void Disarm() => Array.Fill(chambers, false);

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TryFire();
            }
        }

        private void PlaySound(AudioClip clip)
        {
            if (_audioSource && clip)
            {
                _audioSource.PlayOneShot(clip);
            }
        }

        private IEnumerator RecoverFromRecoilRoutine()
        {
            while (Vector3.Distance(_transform.localPosition, _initPos) > 0.001f)
            {
                _transform.localPosition = Vector3.Lerp(
                    _transform.localPosition,
                    _initPos,
                    Time.deltaTime * recoilRecoverySpeed);

                yield return null;
            }

            _transform.localPosition = _initPos;
            _recoilCoroutine = null;
        }

        private void ApplyRecoil()
        {
            var recoilOffset = new Vector3(
                Random.Range(-recoilForce, recoilForce) * 0.1f,
                Random.Range(-recoilForce, recoilForce) * 0.1f,
                -recoilForce);

            _transform.localPosition += recoilOffset;
            
            if(_recoilCoroutine != null)
                StopCoroutine(_recoilCoroutine);

            _recoilCoroutine = StartCoroutine(RecoverFromRecoilRoutine());
        }

        private void SpinChamber() => chamberIndex = Random.Range(0, chambers.Length);

        private void SetNextChamber() => chamberIndex = (chamberIndex + 1) % chambers.Length;

        private bool HasBullet()
        {
            if (chambers[chamberIndex])
                return true;

            SetNextChamber();
            Debug.Log("Патрон отсутствует!"); 
            PlaySound(emptySfx);
            return false;
        }

        public void TryFire()
        {
            if (!CanShoot)
                return;

            CanShoot = false;

            if (!HasBullet())
            {
                StartCoroutine(DelayedBadResult());
                return;
            }

            chambers[chamberIndex] = false;
            SetNextChamber();
            
            if(muzzleFlash)
                muzzleFlash.Play();
            PlaySound(fireSfx);
            ApplyRecoil();

            var origin = muzzleTransform.position;

            if (Physics.Raycast(origin, muzzleTransform.forward, out var hit, maxDistance, targetLayer))
            {
                if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.TakeDamage(1);
                    CanShoot = true;
                }
                else
                {
                    Debug.LogWarning($"Обнаружен объект <b>{hit.collider.name}</b> со слоем {targetLayer}, но без компонента {nameof(IDamageable)}!");
                    onNoShoot?.Invoke();
                }
            }
            else
            {
                StartCoroutine(DelayedBadResult());
            }
        }

        private IEnumerator DelayedBadResult()
        {
            yield return _resultDelay;
            onNoShoot?.Invoke();
            CanShoot = true;
        }

        public void FullLoad()
        {
            SpinChamber();
            Array.Fill(chambers, true);
        }

        public void LoadCount(int count)
        {
            Disarm();
            SpinChamber();

            count = Mathf.Clamp(count, 0, chambers.Length);

            if (count == 0)
            {
                Debug.LogWarning("Попытка зарядить 0 патронов.");
                return;
            }

            if (count == chambers.Length)
            {
                FullLoad();
                return;
            }

            var indices = new int[chambers.Length];
            for (int i = 0; i < chambers.Length; i++)
            {
                indices[i] = i;
            }

            for (int i = 0; i < count; i++)
            {
                var randIndex = Random.Range(i, chambers.Length);
                (indices[i], indices[randIndex]) = (indices[randIndex], indices[i]);
                chambers[indices[i]] = true;
            }
        }

        public void Disable()
        {
            _transform.SetParent(null);
            gameObject.SetActive(false);
        }

        private void OnDrawGizmos()
        {
            if (!muzzleTransform)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawRay(muzzleTransform.position, muzzleTransform.forward * rayLength);
        }
        
        
#if UNITY_EDITOR
        [ContextMenu(nameof(LoadCount))]
        public void LoadCount()
        {
            LoadCount(Random.Range(1, chambers.Length));
        }
#endif
    }
}