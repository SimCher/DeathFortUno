using DeathFortUnoCard.Scripts.CardField.Blocks;
using DeathFortUnoCard.Scripts.CardField.Blocks.Storages;
using DeathFortUnoCard.Scripts.CardField.Cards;
using DeathFortUnoCard.Scripts.CardField.Checkers.Data;
using DeathFortUnoCard.Scripts.CardField.Services;
using DeathFortUnoCard.Scripts.Common;
using DeathFortUnoCard.Scripts.Common.Players;
using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using UnityEngine;
using UnityEngine.Events;

namespace DeathFortUnoCard.Scripts.CardField.Checkers
{
    public class MoveChecker : MonoBehaviour, IService
    {
        [SerializeField] private PlayerSelectedData selectedData = new();
        private BlockStorage _blockStorage;
        private TurnController _turnController;

        public UnityEvent<Card> onCardChecked;
        public UnityEvent onCardEmpty;
        public UnityEvent<BlockView> onBlockChecked;
        public UnityEvent<Block, Player> onTrapChecked;
        public UnityEvent<Block> onBlockCheckFailed;
        
        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            _blockStorage = ServiceLocator.Get<BlockStorage>();
            _turnController = ServiceLocator.Get<TurnController>();
        }

        private void OnDestroy()
        {
            onCardChecked.RemoveAllListeners();
            onCardEmpty.RemoveAllListeners();
            onBlockChecked.RemoveAllListeners();
            onTrapChecked.RemoveAllListeners();
            onBlockCheckFailed.RemoveAllListeners();
        }

        private void Print(string msg) => Debug.Log($"<color=green><b>{msg}</b></color>");

        private void HandleMoveCardCheck(Block selectedBlock)
        {
            Print("Првоерка карты и блока на соответствие.");
            if (BlockCardComparer.CheckMove(selectedData.Card, selectedBlock,
                    _turnController.CurrentPlayer.CurrentBlock))
            {
                if (!_blockStorage.TryGetView(selectedBlock, out var view))
                {
                    Debug.LogError(
                        $"Не могу получить {nameof(BlockView)} в {nameof(_blockStorage)} с ключом {selectedBlock}");
                    return;
                }
                Print("Проверка выполнена.");
                onBlockChecked?.Invoke(view);
                selectedData.Clear();
            }
            else
            {
                Print("Проверка не выполнена.");
                onBlockCheckFailed?.Invoke(selectedBlock);
            }
        }

        private void HandleTrapCardCheck(Block selectedBlock)
        {
            if (BlockCardComparer.CheckTrap(selectedData.Card, selectedBlock))
            {
                selectedBlock.SwitchTrapState(_turnController.CurrentPlayer);
                onTrapChecked?.Invoke(selectedBlock, _turnController.CurrentPlayer);
            }
            else
            {
                onBlockCheckFailed?.Invoke(selectedBlock);
            }
        }

        public void OnCardSelected(Card selected)
        {
            if (selected == null)
            {
                selectedData.Card = null;
                onCardEmpty?.Invoke();
                return;
            }

            selectedData.Card = selected;
            
            onCardChecked?.Invoke(selectedData.Card);
        }

        public void OnBlockSelected(Block selected)
        {
            if (selectedData.Card == null)
                return;
            
            if(selectedData.Card.IsMove)
                HandleMoveCardCheck(selected);
            else if (selectedData.Card.IsTrap)
            {
                HandleTrapCardCheck(selected);
            }
        }
    }
}