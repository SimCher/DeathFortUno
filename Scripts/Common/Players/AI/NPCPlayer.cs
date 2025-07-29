// using System.Collections.Generic;
// using DeathFortUnoCard.Scripts.CardField.Blocks.Storages;
// using DeathFortUnoCard.Scripts.CardField.Cards;
// using DeathFortUnoCard.Scripts.CardField.Dealers;
// using DeathFortUnoCard.Scripts.Common.Players.AI.Behaviours;
// using DeathFortUnoCard.Scripts.Common.Players.AI.Collections;
// using DeathFortUnoCard.Scripts.Common.Players.AI.Pathfinding;
// using DeathFortUnoCard.Scripts.Common.States;
// using DeathFortUnoCard.Scripts.Utils;
// using UnityEngine;
//
// namespace DeathFortUnoCard.Scripts.Common.Players.AI
// {
    // [RequireComponent(typeof(Timer))]
    // public class NPCPlayer : Player
    // {
    //     [Header("Настройки")]
    //     [SerializeField, Range(0, 0.5f)] private float trapCardChance;
    //
    //     [SerializeField] private MinMax waitTime;
    //
    //     public IReadOnlyCollection<Card> PlayerCards => Data.Hand.Cards;
    //
    //     private Timer _timer;
    //     private AIBehaviour _currentBehaviour;
    //     private BehaviourDictionary _behaviours;
    //     
    //     [field: SerializeField]
    //     public Pathfinder Pathfinder { get; private set; }
    //
    //     private void Awake()
    //     {
    //         TryGetComponent(out _timer);
    //     }
    //
    //     protected override void OnEnable()
    //     {
    //         base.OnEnable();
    //         
    //         var dealer = ServiceLocator.ServiceLocator.Get<Dealer>();
    //         var blockStorage = ServiceLocator.ServiceLocator.Get<BlockStorage>();
    //         
    //         Pathfinder = new Pathfinder(this, blockStorage.TargetBlock, () => !dealer.CanDeal);
    //
    //         _behaviours = new BehaviourDictionary();
    //         
    //         _behaviours.AddRange(GetComponentsInChildren<AIBehaviour>(), this);
    //         
    //         FindFirstObjectByType<TurnState>().onNpcStarted.AddListener(Wait);
    //     }
    //
    //     private void EvaluateCardMove()
    //     {
    //         Print("Выбираю карту");
    //         var allTraps = true;
    //         var noTraps = true;
    //
    //         foreach (var card in PlayerCards)
    //         {
    //             if (!card.IsTrap)
    //             {
    //                 allTraps = false;
    //                 continue;
    //             }
    //
    //             noTraps = false;
    //         }
    //
    //         if (allTraps)
    //         {
    //             Print("Все мои карты - ловушки");
    //             _currentBehaviour = _behaviours.GetValue<TrapBehaviour>();
    //         }
    //         else if (noTraps)
    //         {
    //             Print("Все мои карты - движения");
    //             _currentBehaviour = _behaviours.GetValue<MoveBehaviour>();
    //         }
    //         else
    //         {
    //             Print("Хожу");
    //             _currentBehaviour = _behaviours.GetValue<MoveBehaviour>();
    //         }
    //         
    //         _currentBehaviour.Move();
    //     }
    //
    //     public void Print(string message) => Debug.Log($"<color=red>NPC:</color> <b>{message}</b>");
    //
    //     public void UpdatePath() => Pathfinder.UpdatePath(this);
    //
    //     public void Wait()
    //     {
    //         if (ServiceLocator.ServiceLocator.Get<TurnController>().IsThisPlayerTurn)
    //             return;
    //
    //         _currentBehaviour = null;
    //
    //         var time = Random.Range(waitTime.min, waitTime.max);
    //         Print($"Эду {time} сек.");
    //         
    //         _timer.StartTimer(time);
    //     }
    //
    //     public void NPCMove() => EvaluateCardMove();
    //
    //     public override void StartMoving()
    //     {
    //         if (navigation.IsMoving)
    //             return;
    //
    //         if (!Destination)
    //             return;
    //         
    //         navigation.Move(Destination.GetPosition());
    //     }
    // }
// }