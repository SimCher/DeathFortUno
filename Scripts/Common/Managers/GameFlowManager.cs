using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using DeathFortUnoCard.Scripts.Common.States;
using DeathFortUnoCard.Scripts.Common.States.Base;
using DeathFortUnoCard.Scripts.Common.States.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace DeathFortUnoCard.Scripts.Common.Managers
{
    [DefaultExecutionOrder(-999)]
    public class GameFlowManager : MonoBehaviour, IService
    {
        private readonly StateMachine _stateMachine = new();
        [SerializeField] private StateDictionary states = new();

        public UnityEvent onTurnChanged;
        
        private void Awake()
        {
            ServiceLocator.ServiceLocator.Register(this);
        }

        private void Start()
        {
            states.AddRange(GetComponentsInChildren<State>(), _stateMachine);

            if (!states.TryGetValue(nameof(StartState), out var starting))
            {
                Debug.LogError($"Не могу найти {nameof(State)} с ключом {nameof(StartState)}");
                enabled = false;
                return;
            }
            
            _stateMachine.Initialize(starting);
        }

        private void Update()
        {
            if(_stateMachine.CurrentState is IInputableState inputable)
                inputable.HandleInput();
        }

        private void OnDestroy()
        {
            onTurnChanged.RemoveAllListeners();
        }

        private T GetState<T>() where T : State => states.GetValue<T>();

        private void ChangeState<T>() where T : State
        {
            if (typeof(T) != typeof(TurnState) && _stateMachine.CurrentState is T)
            {
                return;
            }
            _stateMachine.ChangeState(GetState<T>());
        }

        public void StartFirstDealing() => ChangeState<DealingState>();

        public void StartCurrentPlayerTurn() => ChangeState<TurnState>();
        public void StartCurrentPlayerMove() => ChangeState<MoveState>();
        public void StartDuel() => ChangeState<DuelState>();
        public void StartSuicide() => ChangeState<SuicideState>();

        public void ChangeTurn()
        {
            if (_stateMachine.IsCurrentState<StartState>() || _stateMachine.IsCurrentState<DealingState>())
                return;
            
            onTurnChanged?.Invoke();
            
            StartCurrentPlayerTurn();
        }

        public void SkipTurn()
        {
            if (_stateMachine.IsCurrentState<StartState>() || _stateMachine.IsCurrentState<DealingState>())
                return;
            
            ChangeTurn();
        }

        public void StartTrapState() => ChangeState<TrapState>();
    }
}