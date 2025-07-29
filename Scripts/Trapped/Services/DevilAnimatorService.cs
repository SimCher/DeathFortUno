using UnityEngine;

namespace DeathFortUnoCard.Scripts.Trapped.Services
{
    public class DevilAnimatorService
    {
        private readonly Animator _animator;

        public DevilAnimatorService(Animator devilAnimator)
        {
            _animator = devilAnimator;
        }

        public void Idle() => _animator.Play("Idle");
        public void Exit() => _animator.Play("Exit");
        public void Knock() => _animator.Play("Knock");
        public void PrepareToShake() => _animator.Play(nameof(PrepareToShake));
        public void SetDices() => _animator.Play(nameof(SetDices));
        public void Shake() => _animator.Play(nameof(Shake), 0, 0f);
        public void ShowResults() => _animator.Play(nameof(ShowResults));
        public void TakeGlass() => _animator.Play(nameof(TakeGlass));
    }
}