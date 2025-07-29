using UnityEngine;
using UnityEngine.UI;

namespace DeathFortUnoCard.UI.Scripts.Components
{
    [System.Serializable]
    public class UIDiceView
    {
        [SerializeField] private Image image;
        [SerializeField] private int value;

        public int Value => value;
        public Image Image => image;

        public void Set(int value, Sprite sprite)
        {
            this.value = value;
            image.enabled = true;
            image.sprite = sprite;
        }

        public void SetColor(Color color) => image.color = color;

        public void Clear()
        {
            value = 0;
            image.enabled = false;
            image.color = Color.black;
        }
    }
}