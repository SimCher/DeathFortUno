using DeathFortUnoCard.Scripts.Common.States;
using DeathFortUnoCard.Scripts.Duel.Players.Shooters;
using DeathFortUnoCard.Scripts.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DeathFortUnoCard.Scripts.Common.Players.Agent
{
    [RequireComponent(typeof(Timer))]
    public class AIPlayer : Player
    {
        private AIBehavior _behavior;
        [SerializeField] private MinMax waitTime;

        private Timer _timer;

        private void Awake()
        {
            _timer = GetComponent<Timer>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            FindFirstObjectByType<TurnState>().onNpcStarted.AddListener(Wait);
            _behavior = GetComponent<AIBehavior>();

            if (!_behavior)
            {
                Debug.LogError($"{nameof(AIPlayer)}: не найден {nameof(AIBehavior)} на объекте!");
            }
            
            _timer.onTimeOut.AddListener(() => _behavior.TakeTurn(this));
            
            _behavior.onMoveOver.AddListener(() => onDestinationReached?.Invoke());
            _behavior.onTakedNewCard.AddListener(Wait);

            var aiShooter = Shooter as NPCShooter;

            if (aiShooter)
            {
                aiShooter.EvaluateDifficulty(_behavior.Difficulty);
            }
            else
            {
                Debug.LogError($"Не могу найти {nameof(NPCShooter)} на компоненте {name}");
            }
        }

        public override void StartMoving()
        {
            if (navigation.IsMoving)
                return;

            navigation.Move(Destination.GetPosition());
        }
        
        public void Wait()
        {
            if (ServiceLocator.ServiceLocator.Get<TurnController>().IsThisPlayerTurn)
                return;
        
            var time = Random.Range(waitTime.min, waitTime.max);
            Debug.Log($"Эду {time} сек.");
            
            _timer.StartTimer(time);
        }
    }
}