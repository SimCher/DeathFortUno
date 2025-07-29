using DeathFortUnoCard.Scripts.Common.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DeathFortUnoCard.UI.Scripts.Components
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public class ShakeButtonPanel : MonoBehaviour, IEnable
    {
        [SerializeField] private Button shakeBtn;
        [SerializeField] private TMP_Text shakeCounText;
        [SerializeField] private Button acceptBtn;

        private CanvasGroup _group;
        private void Awake()
        {
            _group = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            Disable();
        }

        public void ChangeCountText(int currentCount)
        {
            shakeCounText.text = $"({currentCount})";
        }

        public void Enable()
        {
            shakeBtn.enabled = true;
            _group.alpha = 1f;
            _group.blocksRaycasts = true;
            _group.interactable = true;
        }

        public void Disable()
        {
            _group.alpha = 0f;
            _group.blocksRaycasts = false;
            _group.interactable = false;
        }
    }
}