using System;
using UnityEngine;
using UnityEngine.Events;

namespace DeathFortUnoCard.Scripts.Common.Players
{
    public class LookMode : MonoBehaviour
    {
        private bool _isEnabled;
        private bool _isAvailable = true;

        private HumanPlayer _owner;

        public UnityEvent onLookModeEnabled;
        public UnityEvent onLookModeDisabled;


        private void OnEnable()
        {
            onLookModeEnabled.AddListener(() => _owner.SetControllerActivity(true));
            onLookModeDisabled.AddListener(() => _owner.SetControllerActivity(false));
        }

        private void OnDestroy()
        {
            onLookModeEnabled.RemoveAllListeners();
            onLookModeDisabled.RemoveAllListeners();
        }

        public void AddEnabledListeners(params UnityAction[] listeners)
        {
            for (int i = 0; i < listeners.Length; i++)
            {
                onLookModeEnabled.AddListener(listeners[i]);
            }
        }
        
        public void AddDisabledListeners(params UnityAction[] listeners)
        {
            for (int i = 0; i < listeners.Length; i++)
            {
                onLookModeDisabled.AddListener(listeners[i]);
            }
        }

        public void Initialize(HumanPlayer owner) => _owner = owner;

        public void Enable()
        {
            _isEnabled = true;
            TurnController.ThisPlayer.SetControllerActivity(true);
            onLookModeEnabled?.Invoke();
        }

        public void Disable()
        {
            _isEnabled = false;
            TurnController.ThisPlayer.SetControllerActivity(false);
            onLookModeDisabled?.Invoke();
        }

        public void ChangeAccesible(bool notState) => _isAvailable = !notState;

        public void SwitchActivity()
        {
            if (!_isAvailable)
                return;

            _isEnabled = !_isEnabled;

            if (_isEnabled) Enable();
            else Disable();
        }
    }
}