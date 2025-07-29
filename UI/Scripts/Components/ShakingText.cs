using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using TMPro;
using UnityEngine;

namespace DeathFortUnoCard.UI.Scripts.Components
{
    [RequireComponent(typeof(TMP_Text), typeof(ShakeText), typeof(Visibility))]
    public class ShakingText : MonoBehaviour
    {
        private TMP_Text _tmpText;
        private ShakeText _shaker;
        private Visibility _visibility;

        private UIResources _resources;

        private void Awake()
        {
            _tmpText = GetComponent<TMP_Text>();
            _shaker = GetComponent<ShakeText>();
            _visibility = GetComponent<Visibility>();
        }

        private void Start()
        {
            _resources = ServiceLocator.Get<UIResources>();
            Hide();
        }

        public void Show(string key)
        {
            if (!_resources.trapTexts.TryGetValue(key, out var text))
            {
                Debug.LogError($"Не могу найти ключ {key} в {nameof(_resources.trapTexts)}");
                return;
            }

            _tmpText.text = text;
            _visibility.Show();
            _shaker.Enable();
        }

        public void Hide()
        {
            _visibility.Hide();
            _shaker.Disable();
        }
    }
}