using DeathFortUnoCard.UI.Scripts.Common;
using DeathFortUnoCard.UI.Scripts.Services;
using UnityEngine;

namespace DeathFortUnoCard.UI.Scripts.Components
{
    [RequireComponent(typeof(Animator))]
    public class Visibility : MonoBehaviour, IVisible
    {
        private UIAnimatorService _animator;

        [field: SerializeField]
        public bool IsVisible { get; private set; } = true;

        private void Awake()
        {
            _animator = new UIAnimatorService(GetComponent<Animator>());
        }

        private void OnEnable()
        {
            if(IsVisible)
                ForceShow();
            else
                ForceHide();
        }

        private void OnDisable()
        {
            _animator.Hide();
        }

        [ContextMenu("Show")]
        public void Show()
        {
            if (IsVisible)
                return;

            _animator.Appeared();
            IsVisible = true;
        }

        public void ForceShow()
        {
            IsVisible = true;
            _animator.Idle();
        }

        public void ForceHide()
        {
            IsVisible = false;
            _animator.Hide();
        }

        [ContextMenu("Hide  ")]
        public void Hide()
        {
            if (!IsVisible)
                return;
            
            _animator.Disappeared();
            IsVisible = false;
        }
    }
}