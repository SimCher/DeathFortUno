// using DeathFortUnoCard.Scripts.Common.Collections;
// using DeathFortUnoCard.Scripts.Common.Players.AI.Behaviours;
//
// namespace DeathFortUnoCard.Scripts.Common.Players.AI.Collections
// {
//     [System.Serializable]
//     public class BehaviourDictionary : GameDictionaryBase<AIBehaviour>
//     {
//         public void AddRange(AIBehaviour[] behs, NPCPlayer owner)
//         {
//             for (int i = 0; i < behs.Length; i++)
//             {
//                 Add(behs[i].GetType().Name, behs[i]);
//                 behs[i].Initialize(owner);
//             }
//         }
//     }
// }