using UnityEngine;

namespace DeathFortUnoCard.Scripts.CardField.Cards.Effects
{
    public class CardHoverEffect : MonoBehaviour
    {
        [SerializeField] private float hoverHeight = 30f;
        [SerializeField] private float selectedHoverHeight = 60f;
        [SerializeField] private RectTransform rectTransform;

        private Vector3 _originalPosition;

        private bool _isSelected;

        private void OnEnable()
        {
            if (!rectTransform)
            {
                Debug.LogWarning($"{nameof(rectTransform)} равен null!");
                enabled = false;
                return;
            }

            _originalPosition = rectTransform.localPosition;
        }

        public void SetSelectedState(bool isSelected)
        {
            rectTransform.localPosition = isSelected
                ? new Vector3(0f, 0f + selectedHoverHeight, 0f)
                : Vector3.zero;
        }

        public void IncreaseVfx()
            => rectTransform.localPosition = new Vector3(_originalPosition.x, _originalPosition.y + hoverHeight,
                _originalPosition.z);

        public void DecreaseVfx() => rectTransform.localPosition = Vector3.zero;
    }
}