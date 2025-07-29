// namespace DeathFortUnoCard.Scripts.Common.Players.AI.Behaviours
// {
//     public class TrapBehaviour : AIBehaviour
//     {
//         public override void Move()
//         {
//             var selected =
//                 owner.Pathfinder.FindTrapData(owner.PlayerCards, turnController.NextPlayer.CurrentBlock.Neighbors);
//
//             if (selected.card == null || selected.block == null)
//             {
//                 if (dealer.TryDeal())
//                     owner.Wait();
//                 else
//                 {
//                     onMoveOver?.Invoke();
//                 }
//             }
//             else
//             {
//                 owner.Data.Hand.SetSelectedCard(selected.card);
//                 moveChecker.OnBlockSelected(selected.block);
//             }
//         }
//     }
// }