// using DeathFortUnoCard.Scripts.CardField.Checkers;
// using DeathFortUnoCard.Scripts.CardField.Dealers;
// using DeathFortUnoCard.Scripts.Common.Managers;
// using UnityEngine;
// using UnityEngine.Events;
//
// namespace DeathFortUnoCard.Scripts.Common.Players.AI.Behaviours
// {
//     public abstract class AIBehaviour : MonoBehaviour
//     {
//         protected NPCPlayer owner;
//
//         protected MoveChecker moveChecker;
//         protected GameFlowManager gameManager;
//         protected Dealer dealer;
//         protected TurnController turnController;
//
//         public UnityEvent onMoveOver;
//
//         public virtual void Initialize(NPCPlayer newOwner)
//         {
//             if (!newOwner)
//             {
//                 Debug.LogError($"Дан пустой {nameof(newOwner)}");
//                 enabled = false;
//                 return;
//             }
//
//             turnController = ServiceLocator.ServiceLocator.Get<TurnController>();
//             gameManager = ServiceLocator.ServiceLocator.Get<GameFlowManager>();
//             moveChecker = ServiceLocator.ServiceLocator.Get<MoveChecker>();
//             dealer = ServiceLocator.ServiceLocator.Get<Dealer>();
//
//             owner = newOwner;
//         }
//
//         public abstract void Move();
//     }
// }