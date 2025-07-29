// namespace DeathFortUnoCard.Scripts.Common.Players.AI.Behaviours
// {
//     public class MoveBehaviour : AIBehaviour
//     {
//         public override void Initialize(NPCPlayer newOwner)
//         {
//             base.Initialize(newOwner);
//             onMoveOver.AddListener(turnController.OnTurnOver);
//             onMoveOver.AddListener(gameManager.SkipTurn);
//         }
//
//         public override void Move()
//         {
//             owner.Print("Ищу данные для движения");
//             var selected =
//                 owner.Pathfinder.FindDataForMoving(owner.PlayerCards,
//                     turnController.NextPlayer.CurrentBlock.Neighbors);
//
//             if (selected.card == null || selected.block == null)
//             {
//                 owner.Print("Данных нет.");
//                 if (dealer.TryDeal())
//                 {
//                     owner.Print("Беру карту и жду");
//                     owner.Wait();
//                 }
//                 else
//                 {
//                     owner.Print("Завершаю ход");
//                     onMoveOver?.Invoke();
//                 }
//             }
//             else
//             {
//                 owner.Print("Данные есть. Выбираю блок");
//                 owner.Data.Hand.SetSelectedCard(selected.card);
//                 moveChecker.OnBlockSelected(selected.block);
//             }
//         }
//     }
// }