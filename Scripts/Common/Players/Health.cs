using System;
using DeathFortUnoCard.Scripts.Duel.Controllers;
using DeathFortUnoCard.Scripts.Duel.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace DeathFortUnoCard.Scripts.Common.Players
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField, Range(1, 5)] private int maxHealth = 3;
        [SerializeField] private int currentHealth;

        private bool _isInitialized;

        public UnityEvent onDamage;
        public UnityEvent onDeath;

        private void OnEnable()
        {
            Initialize();
            
            var duelController = ServiceLocator.ServiceLocator.Get<DuelController>();

            if (duelController)
            {
                onDamage.AddListener(duelController.DropGun);
            }
        }

        private void OnDisable()
        {
            var duelController = ServiceLocator.ServiceLocator.Get<DuelController>();
            
            if(duelController)
                onDamage.RemoveListener(duelController.Disable);
        }

        private void OnDestroy()
        {
            onDamage.RemoveAllListeners();
            onDeath.RemoveAllListeners();
        }

        private void Initialize()
        {
            if (_isInitialized)
                return;
            currentHealth = maxHealth;
            _isInitialized = true;
        }

        private void Die() => onDeath?.Invoke();

#if UNITY_EDITOR
        [ContextMenu(nameof(Increase))]
        public void Increase() => Heal(1);

        [ContextMenu(nameof(Decrease))]
        public void Decrease() => TakeDamage(1);
#endif
        
        public void TakeDamage(int damage)
        {
            if (!_isInitialized)
            {
                Debug.LogError($"Здоровье у {name} не инициализировано!");
                return;
            }

            currentHealth -= damage;

            if (currentHealth < 0)
            {
                currentHealth = 0;
                Die();
                return;
            }
            
            onDamage?.Invoke();
        }

        public void Heal(int healAmount)
        {
            if (!_isInitialized)
            {
                Debug.LogError($"Здоровье у {name} не инициализировано!");
                return;
            }

            currentHealth += healAmount;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }
    }
}