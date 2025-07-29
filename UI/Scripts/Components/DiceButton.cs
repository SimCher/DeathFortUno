using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DeathFortUnoCard.UI.Scripts.Components
{
    [AddComponentMenu("UI/Dice Button")]
    public class DiceButton : Button
    {
        [System.Serializable]
        public class IntEvent : UnityEvent<int> { }
        
        [SerializeField] private int value;
        
        public int Value => value;

        public IntEvent onHover = new();
        public UnityEvent onDiceSelected = new();

        public void Initialize(int value, UnityAction<int> onHover, UnityAction onDiceSelected)
        {
            this.value = value;
            this.onHover.AddListener(onHover);
            this.onDiceSelected.AddListener(onDiceSelected);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            onHover?.Invoke(value);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            onDiceSelected?.Invoke();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            onHover?.Invoke(-1);
        }
    }
}