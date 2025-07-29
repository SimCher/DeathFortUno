using System;
using DeathFortUnoCard.Scripts.CardField.Blocks;
using DeathFortUnoCard.Scripts.CardField.Blocks.Interfaces;
using DeathFortUnoCard.Scripts.CardField.Cards;
using DeathFortUnoCard.Scripts.Common;
using DeathFortUnoCard.Scripts.Common.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DeathFortUnoCard.UI.Scripts.Components
{
    [RequireComponent(typeof(RawImage), typeof(RectTransform))]
    public class UIBlockView : MonoBehaviour, IBlockDataObserver, IEnable, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private RawImage _image;
        private Image _blockImage;

        [SerializeField] private Block block;
#if UNITY_EDITOR
        [SerializeField] private BlockStateText stateText;
#endif

        private bool _isEnabled;

        private GameField _owner;

        public Vector2Int Coords => block?.Coords ?? throw new NullReferenceException("block был null.");

        public Vector2 Size { get; private set; }
        public RectTransform Transform { get; private set; }

        public Block Block => block;
        private void Awake()
        {
            TryGetComponent(out _image);
            _blockImage = GetComponentInChildren<Image>();
            Transform = GetComponent<RectTransform>();
            Size = Transform.sizeDelta;
        }

        public void Initialize(GameField owner, Block block)
        {
            _owner = owner;
            this.block = block;

            block.StateChanged += OnStateChanged;
            OnStateChanged();
        }

        public void OnStateChanged()
        {
            SetColor();
            
            if(block.IsWalkable)
                Enable();
            else
                Disable();

#if UNITY_EDITOR
            stateText.ChangeState(block.IsTarget, block.IsTrapped);
#endif
        }

        public void Enable()
        {
            if (_isEnabled)
                return;
            
            _isEnabled = true;

            var oldColor = _blockImage.color;

            _blockImage.color = new Color(oldColor.r, oldColor.g, oldColor.b, 1f);
        }

        public void Disable()
        {
            if (!_isEnabled)
                return;

            var oldColor = _blockImage.color;

            _blockImage.color = new Color(oldColor.r, oldColor.g, oldColor.b, 0.3f);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_isEnabled)
                return;

            _image.color = Color.white;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _image.color = Color.black;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isEnabled)
                return;

            _owner.OnBlockSelected( this);
            // _owner.OnUiBlockSelected(this);
        }

        public void ScaleIfMatch(Card card)
        {
            if(block.Color == card.Color && block.IsWalkable)
                Scale();
            else
                RestoreScale();
        }

        public bool IsMatch(Card card) => block.Color == card.Color && block.IsWalkable;

        public void Scale() => transform.localScale = new Vector2(1.2f, 1.2f);

        public void RestoreScale() => transform.localScale = Vector3.one;

        public void SetColor()
        {
            switch (block.Color)
            {
                case GameColor.Blue:
                    _blockImage.color = Color.blue;
                    break;
                case GameColor.Red:
                    _blockImage.color = Color.red;
                    break;
                case GameColor.Green:
                    _blockImage.color = Color.green;
                    break;
                case GameColor.Yellow:
                    _blockImage.color = Color.yellow;
                    break;
            }
        }

        public void HandleError() => _image.color = Color.red;
    }
}