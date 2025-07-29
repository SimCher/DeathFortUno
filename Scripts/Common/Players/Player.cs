using System;
using DeathFortUnoCard.Scripts.CardField.Blocks;
using DeathFortUnoCard.Scripts.Common.Interfaces;
using DeathFortUnoCard.Scripts.Common.Players.Data;
using DeathFortUnoCard.Scripts.Duel.Players.Shooters;
using UnityEngine;
using UnityEngine.Events;

namespace DeathFortUnoCard.Scripts.Common.Players
{
    [RequireComponent(typeof(Navigation.Navigation), typeof(Shooter), typeof(Health))]
    public abstract class Player : MonoBehaviour, IPlayerPositionSubject
    {
        [SerializeField] private Block currentBlock;
        [SerializeField] private BlockView destination;

        protected Navigation.Navigation navigation;

        protected bool isEnabled;
        
        [field: SerializeField] public PlayerData Data { get; private set; }
        [field: SerializeField] public UniqueData ID { get; private set; }
        
        [field: SerializeField] public Shooter Shooter { get; private set; }
        [field: SerializeField] public GunHolderController GunHolder { get; private set; }
        [field: SerializeField] public Health Health { get; private set; }
        
        public UnityEvent onDestinationReached;

        protected Coroutine moveRoutine;

        private TurnController _turnController;

        public Block CurrentBlock
        {
            get => currentBlock;
            set
            {
                if (value == null)
                {
                    Debug.LogWarning($"Предотвращена попытка присвоения {nameof(Block)} со значением null в " +
                                     $"{nameof(Player)}");
                    return;
                }
                
                if (!value.IsWalkable)
                    return;
                
                currentBlock?.ChangeWalkState(true);

                currentBlock = value;
                value.ChangeWalkState(false);
                
                BlockChanged?.Invoke(ID.Id, currentBlock);
            }
        }

        public BlockView Destination
        {
            get => destination;
            set
            {
                if (!value)
                {
                    Debug.LogError($"Дан null для {nameof(Destination)}");
                    return;
                }

                destination = value;
            }
        }
        public event Action<int, Block> BlockChanged;

        protected virtual void OnEnable()
        {
            _turnController = ServiceLocator.ServiceLocator.Get<TurnController>();
            navigation = GetComponent<Navigation.Navigation>();
            Shooter = GetComponent<Shooter>();
            GunHolder = GetComponentInChildren<GunHolderController>();
        }

        private void Print(string msg)
        {
            Debug.Log($"<color=yellow><b>{msg}</b></color>");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_turnController.CurrentPlayer != this)
                return;
            
            Print("Текущий игрок!");
            
            if (!navigation.IsMoving)
                return;
            
            Print("Двигаюсь!");

            if (!other.CompareTag("Checkpoint"))
                return;
            
            Print("В чекпоинте!");
            
            if (!Destination)
                return;
            
            Print($"{Destination} есть!");

            var view = other.GetComponentInParent<BlockView>();
            if (!view)
                return;
            
            Print($"Нашёл {view} у {other}!");

            CurrentBlock = Destination.Block;
            
            if (view.Block != Destination.Block)
                return;
            
            Print("Текущий блок равен блоку назначения!");
            
            OnMoveOver();
        }

        private void OnMoveOver()
        {
            Debug.Log($"<color=red><b>В OnMoveOver()!</b></color>");
            transform.position = Destination.GetPosition();
            navigation.Stop();
            destination = null;
            
            onDestinationReached?.Invoke();
        }

        private void OnDestroy()
        {
            if (moveRoutine != null)
            {
                StopCoroutine(moveRoutine);
            }
            
            BlockChanged = null;
            
            onDestinationReached.RemoveAllListeners();
        }

        public void SetControllerActivity(bool state) => isEnabled = state;

        public abstract void StartMoving();

        public void SetId()
        {
            if (ID == null)
            {
                Debug.LogError($"{nameof(ID)} не назначен!");
                return;
            }

            if (ID.Id <= 0)
            {
                Debug.LogError($"Объект {name} имеет некорректный {nameof(ID.Id)} в компоненте " +
                               $"{nameof(Player)}");
                return;
            }
            
            Data.Hand.SetId(ID.Id);
            Data.Marker.SetId(ID.Id);
        }

        public void SetStartBlock(Block newStartBlock)
        {
            Data.StartBlock = newStartBlock;
            CurrentBlock = newStartBlock;
            BlockChanged?.Invoke(ID.Id, newStartBlock);
        }

        public void LookAt(Transform target)
            => transform.LookAt(target);
    }
}