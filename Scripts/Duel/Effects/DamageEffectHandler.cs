using System.Collections.Generic;
using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using UnityEngine;
using UnityEngine.Events;

namespace DeathFortUnoCard.Scripts.Duel.Effects
{
    public class DamageEffectHandler : MonoBehaviour, IService
    {
        [Header("Ссылки")]
        [SerializeField] private CameraShaker cameraShaker;

        [Tooltip("Компоненты, реализующие IDamageEffect")] [SerializeField]
        private List<MonoBehaviour> effects = new();

        [Header("События")]
        public UnityEvent onEffectsStart;
        public UnityEvent onEffectsOver;

        private readonly List<IDamageEffect> _effects = new();
        private Coroutine _damageCoroutine;

        public CameraShaker CamShaker
        {
            get => cameraShaker;
            set
            {
                if (value)
                    cameraShaker = value;
            }
        }

        private void Awake()
        {
            ServiceLocator.Register(this);
            for (int i = 0; i < effects.Count; i++)
            {
                var effect = effects[i] as IDamageEffect;
                if(effect != null)
                    _effects.Add(effect);
            }
        }

        private void OnDestroy()
        {
            if(_damageCoroutine != null)
                StopCoroutine(_damageCoroutine);
            
            onEffectsStart.RemoveAllListeners();
            onEffectsOver.RemoveAllListeners();
        }

        private System.Collections.IEnumerator PlayEffectsRoutine()
        {
            onEffectsStart?.Invoke();
            var timer = 0f;
            var applied = new bool[_effects.Count];

            while (true)
            {
                var allApplied = true;

                timer += Time.deltaTime;

                for (int i = 0; i < _effects.Count; i++)
                {
                    if (!applied[i] && timer >= _effects[i].Delay)
                    {
                        _effects[i].Apply();
                        applied[i] = true;
                    }

                    if (!applied[i])
                        allApplied = false;
                }

                if (allApplied)
                    break;

                yield return null;
            }
        }

        [ContextMenu(nameof(StartEffect))]
        public void StartEffect()
        {
            if(_damageCoroutine != null)
                StopCoroutine(_damageCoroutine);
            
            cameraShaker.StartTransition();
            _damageCoroutine = StartCoroutine(PlayEffectsRoutine());
        }

        public void Stop()
        {
            if (_damageCoroutine != null)
            {
                StopCoroutine(_damageCoroutine);
                _damageCoroutine = null;
            }
            
            onEffectsOver?.Invoke();
        }
    }
}