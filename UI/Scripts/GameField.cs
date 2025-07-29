using System.Collections.Generic;
using DeathFortUnoCard.Scripts.CardField.Blocks;
using DeathFortUnoCard.Scripts.CardField.Blocks.Storages;
using DeathFortUnoCard.Scripts.CardField.Cards;
using DeathFortUnoCard.Scripts.Common;
using DeathFortUnoCard.Scripts.Common.Extensions;
using DeathFortUnoCard.Scripts.Common.Interfaces;
using DeathFortUnoCard.Scripts.Common.Players;
using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using DeathFortUnoCard.UI.Scripts.Collections;
using DeathFortUnoCard.UI.Scripts.Components;
using DeathFortUnoCard.UI.Scripts.Poolers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DeathFortUnoCard.UI.Scripts
{
    [DefaultExecutionOrder(-501)]
    [RequireComponent(typeof(RectTransform), typeof(GridLayoutGroup))]
    public class GameField : MonoBehaviour, IEnable, IPlayerPositionObserver, IService
    {
        #region Serialized Fields

        [Header("Ссылки")]
        [SerializeField] private UIPlayerMarker[] playerMarkers;
        [SerializeField] private UIBlockViewPooler pooler;
        [SerializeField] private UIBlockDictionary uiBlocks;

        [Header("Настройки")]
        [SerializeField, Range(0, 10)] private float paddingX;
        [SerializeField, Range(0, 10)] private float paddingY;
        #endregion

        #region Events

        public UnityEvent<Block> onBlockSelected;

        public UnityEvent<bool> onAnimationStateChanged;
        
        #endregion

        #region Private Fields

        private TurnController _turnController;
        private GridLayoutGroup _gridLayout;
        private bool _isAnimated;


        private HashSet<UIBlockView> _enabledCells = new();
        private HashSet<UIBlockView> _scaledCells = new();
        
        #endregion

        #region Props

        public bool IsAnimated => _isAnimated;

        #endregion

        #region Unity Events

        private void Awake()
        {
            ServiceLocator.Register(this);
            _gridLayout = GetComponent<GridLayoutGroup>();
        }

        #endregion

        private UIPlayerMarker GetCurrentMarker() => _turnController.CurrentPlayer.Data.Marker;

        public void Initialize(int height, int width, int playersCount)
        {
            _gridLayout.constraintCount = width;
            _turnController = ServiceLocator.Get<TurnController>();
            var blockStorage = ServiceLocator.Get<BlockStorage>();
            playerMarkers = new UIPlayerMarker[playersCount];
            pooler.Pool(height * width);
            blockStorage.onBlockAdded.AddListener(AddUIBlock);
        }
        private void ReorderBlocks()
        {
            if (uiBlocks == null || uiBlocks.Count == 0)
                return;

            var sorted = new List<UIBlockView>(uiBlocks.Values);
            
            sorted.Sort((a, b) =>
            {
                var compareY = a.Coords.y.CompareTo(b.Coords.y);
                if (compareY != 0) return compareY;

                return a.Coords.x.CompareTo(b.Coords.x);
            });

            for (int i = 0; i < sorted.Count; i++)
            {
                sorted[i].Transform.SetSiblingIndex(i);
            }
        }

        private void AddUIBlock(Block block)
        {
            var newUIBlock = pooler.GetPooled();
            newUIBlock.Initialize(this, block);
            newUIBlock.transform.SetParent(transform);
            newUIBlock.gameObject.InitValuesForPooled(Vector3.zero, Quaternion.identity);
            uiBlocks.Add(block, newUIBlock);
            ReorderBlocks();
        }

        private void DisableAnimatedState()
        {
            _isAnimated = false;
            onAnimationStateChanged?.Invoke(false);
        }

        private void EnableAnimatedState()
        {
            _isAnimated = true;
            onAnimationStateChanged?.Invoke(true);
        }

        private void EnableAllBlocks()
        {
            foreach (var uiBlock in uiBlocks.Values)
            {
                uiBlock.Enable();
                _enabledCells.Add(uiBlock);
            }
        }

        private void DisableAllBlocks()
        {
            foreach (var cell in _enabledCells)
            {
                cell.Disable();
            }
            
            _enabledCells.Clear();
        }

        private void RestoreAllBlocks()
        {
            foreach (var cell in _scaledCells)
            {
                cell.RestoreScale();
            }
            _scaledCells.Clear();
        }

        private void HighlightAllMatchingBlocks(Card selected)
        {
            foreach (var uiBlock in uiBlocks.Values)
            {
                if (!uiBlock.IsMatch(selected))
                {
                    continue;
                }
                
                uiBlock.Scale();
                _scaledCells.Add(uiBlock);
            }
        }

        private void HighlightAllMatchingBlocks(IEnumerable<Block> blocks, Card selected)
        {
            foreach (var block in blocks)
            {
                if (!uiBlocks.TryGetValue(block, out var uiBlock))
                {
                    continue;
                }

                if (!uiBlock.IsMatch(selected))
                {
                    continue;
                }
               
                uiBlock.Scale();
                _scaledCells.Add(uiBlock);
            }
        }

        public void ScaleAvailableBlocks(Card selected)
        {
            if (selected == null)
            {
                RestoreAllBlocks();
                return;
            }

            if (selected.IsTrap)
            {
                HighlightAllMatchingBlocks(selected);
            }
            else if (selected.IsMove)
            {
                var marker = GetCurrentMarker();
                RestoreAllBlocks();
                var currentBlockNeighbors = marker.Owner.Block.Neighbors;
                HighlightAllMatchingBlocks(currentBlockNeighbors, selected);
            }
        }

        public void OnCardChecked(Card selected)
        {
            if(_turnController.IsThisPlayerTurn)
                Enable();
        }

        public void OnBlockSelected(UIBlockView view)
        {
            onBlockSelected?.Invoke(view.Block);
        }
        
        public void Enable()
        {
            RestoreAllBlocks();
            EnableAllBlocks();
        }

        public void Disable()
        {
            RestoreAllBlocks();
            DisableAllBlocks();
        }

        public void OnBlockChanged(int id, Block block)
        {
            for (int i = 0; i < playerMarkers.Length; i++)
            {
                if(playerMarkers[i] && playerMarkers[i].Id != id)
                    continue;

                if (uiBlocks.TryGetValue(block, out var uiBlock))
                {
                    playerMarkers[i].SetOwner(uiBlock);
                    return;
                }
            }
        }

        public void SetMarker(UIPlayerMarker marker)
        {
            for (int i = 0; i < playerMarkers.Length; i++)
            {
                if (!playerMarkers[i])
                {
                    playerMarkers[i] = marker;
                    return;
                }
            }
        }

        public void ShowError(Block errorBlock)
        {
            if(uiBlocks.TryGetValue(errorBlock, out var view))
                view.HandleError();
        }
    }
}