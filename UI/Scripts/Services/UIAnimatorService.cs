using UnityEngine;

namespace DeathFortUnoCard.UI.Scripts.Services
{
    public class UIAnimatorService
    {
        private readonly Animator _animator;

        public UIAnimatorService(Animator animator)
        {
            _animator = animator;
        }

        private void PlayAnimation(string name) => _animator.Play(name, 0, 0f);

        public void Idle() => PlayAnimation("Idle");
        public void Hide() => PlayAnimation("Hide");
        
        public void Appeared() => PlayAnimation("Appearing");

        public void Disappeared() => PlayAnimation("Disappearing");

        public void Increase() => PlayAnimation("Increase");

        public void Decrease() => PlayAnimation("Decrease");
    }
}