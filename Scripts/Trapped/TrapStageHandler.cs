using DeathFortUnoCard.Scripts.Common;
using DeathFortUnoCard.Scripts.Common.Players;
using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using DeathFortUnoCard.Scripts.Common.States;
using DeathFortUnoCard.Scripts.Common.States.Base;
using DeathFortUnoCard.Scripts.Common.States.Collections;
using DeathFortUnoCard.Scripts.Duel.Controllers;
using DeathFortUnoCard.Scripts.Trapped.DevilScripts;
using DeathFortUnoCard.Scripts.Trapped.States;
using DeathFortUnoCard.Scripts.Trapped.States.Base;
using DeathFortUnoCard.Scripts.Utils;
using DeathFortUnoCard.UI.Scripts.Components;
using UnityEngine;
using UnityEngine.Events;

namespace DeathFortUnoCard.Scripts.Trapped
{
    [DefaultExecutionOrder(-999)]
    [RequireComponent(typeof(MultiTimerManager))]
    public class TrapStageHandler : MonoBehaviour, IService
    {
#if UNITY_EDITOR
        public enum DebugState
        {
            Unknown,
            Suicide,
            Duel
        }

        public enum ResultDebugState
        {
            Unknown,
            Lucky,
            Unlucky
        }

        [Header("Отладка")]
        public DebugState testState;

        public ResultDebugState resultState;
#endif
        [Header("Состояния")]
        [SerializeField] private StateMachine stateMachine;

        [SerializeField] private StateDictionary states;

        [SerializeField] private Player whoTrapped;
        [SerializeField] private Player byWhomTrapped;

        public UnityEvent<int[]> onResultEvaluated;
        public UnityEvent onFinish;

        [SerializeField] private UITrapResultWindow resultWindow;
        public bool IsSuicideTrap => whoTrapped == byWhomTrapped;
        
        private void Awake()
        {
            ServiceLocator.Register(this);

            stateMachine = new StateMachine();
            
            states.AddRange(GetComponentsInChildren<BaseTrapState>(), stateMachine);
            
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            whoTrapped = null;
            byWhomTrapped = null;
        }

        private void OnDestroy()
        {
            onFinish.RemoveAllListeners();
            onResultEvaluated.RemoveAllListeners();
        }

        private T GetState<T>() where T : BaseTrapState
            => states.GetValue<T>();

        private void ChangeState<T>() where T : BaseTrapState
            => stateMachine.ChangeState(GetState<T>());

        public void ShowShakeResult() => ChangeState<ShowShakeResultState>();

        public void Enable(Player trapped, Player trapOwner)
        {
#if UNITY_EDITOR
            var turnController = ServiceLocator.Get<TurnController>();
            var duelController = ServiceLocator.Get<DuelController>();

            switch (testState)
            {
                case DebugState.Duel:
                    whoTrapped = turnController.CurrentPlayer;
                    byWhomTrapped = turnController.NextPlayer;
                    break;
                case DebugState.Suicide:
                    whoTrapped = byWhomTrapped = turnController.CurrentPlayer;
                    break;
                case DebugState.Unknown:
                    default:
                        whoTrapped = trapped;
                        byWhomTrapped = trapOwner;
                        break;
            }
            
            duelController.SetPlayers(trapped, trapOwner);
#endif

            if (!states.TryGetValue(nameof(TrapStartState), out var starting))
            {
                Debug.LogError($"Не могу найти {nameof(TrapStartState)} в {nameof(states)}");
                enabled = false;
                return;
            }
            
            resultWindow.SetPlayersData(trapped.ID, trapOwner.ID);
            stateMachine.Initialize(starting);
        }
        
        public void LayDices() => ChangeState<LayDiceState>();
        public void SelectDices() => ChangeState<SelectDiceState>();
        public void OnDiceSelected() => ChangeState<DiceSelectedState>();
        public void PrepareShake() => ChangeState<PrepareToShakeState>();
        public void StartShaking() => ChangeState<ShakeState>();

        public void SetDigit(int value) => resultWindow.OnSelectedValueChange(value);

        public void DisableTrapStage()
        {
            onFinish?.Invoke();
            resultWindow.OnTrapStageDisabled();
        }
    }
}