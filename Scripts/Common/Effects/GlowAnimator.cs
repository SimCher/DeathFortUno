using UnityEngine;

namespace DeathFortUnoCard.Scripts.Common.Effects
{
    public class GlowAnimator : MonoBehaviour
    {
        [Header("Настройки свечения")]
        [SerializeField] private float glowSpeed = 2f;

        [SerializeField] private float glowStrength = 3f;

        [SerializeField] private Color glowColor = Color.green;

        private Coroutine _animationCoroutine;
        private Renderer _cachedRenderer;
        private MaterialPropertyBlock _propertyBlock;

        private float _phase;
        private static readonly int GlowPhase = Shader.PropertyToID("_GlowPhase");
        private static readonly int GlowStrength = Shader.PropertyToID("_GlowStrength");
        private static readonly int GlowColor = Shader.PropertyToID("_GlowColor");

        private void Awake()
        {
            _cachedRenderer = GetComponent<Renderer>();
            _propertyBlock = new MaterialPropertyBlock();
            ResetGlow();
        }

        private void OnDisable()
        {
            DeactivateGlow();
        }

        private void ResetGlow()
        {
            _cachedRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat(GlowPhase, 0f);
            _propertyBlock.SetFloat(GlowStrength, 0f);
            _propertyBlock.SetColor(GlowColor, Color.black);
            _cachedRenderer.SetPropertyBlock(_propertyBlock);
        }

        private System.Collections.IEnumerator AnimateGlowRoutine()
        {
            while (true)
            {
                _phase += Time.deltaTime * glowSpeed;
                if (_phase > Mathf.PI * 2f)
                    _phase -= Mathf.PI * 2f;
                
                _cachedRenderer.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetFloat(GlowPhase, _phase);
                _propertyBlock.SetFloat(GlowStrength, glowStrength);
                _propertyBlock.SetColor(GlowColor, glowColor);
                _cachedRenderer.SetPropertyBlock(_propertyBlock);

                yield return null;
            }
        }

        public void ActivateGlow()
        {
            if (_animationCoroutine != null)
                return;

            _phase = 0f;
            _animationCoroutine = StartCoroutine(AnimateGlowRoutine());
        }

        public void DeactivateGlow()
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
                _animationCoroutine = null;
            }
            
            ResetGlow();
        }
    }
}