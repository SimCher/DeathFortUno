// using System;
// using System.Collections.Generic;
// using System.Linq;
// using DeathFortUnoCard.Scripts.CardField.Blocks;
// using DeathFortUnoCard.Scripts.CardField.Blocks.Collections;
// using DeathFortUnoCard.Scripts.CardField.Cards;
// using DeathFortUnoCard.Scripts.Common.Players.AI.Pathfinding.Services;
// using UnityEngine;
// using Random = UnityEngine.Random;
//
// namespace DeathFortUnoCard.Scripts.Common.Players.AI.Pathfinding
// {
//     [Serializable]
//     public class Pathfinder
//     {
//        [field: SerializeField] public NPCPlayer Owner { get; private set; }
//        [field: SerializeField] public Path CurrentPath { get; private set; }
//        [field: SerializeField] public Block TargetBlock { get; private set; }
//
//        private readonly BlockLimitedCollection _trapBlocks = new(4);
//        private readonly BlockLimitedCollection _recentBlocks = new(3);
//
//        private readonly AStar _aStar;
//
//        private Coroutine _pathRoutine;
//
//        private readonly Func<bool> _canAct;
//
//        public Pathfinder(NPCPlayer owner, Block targetBlock, Func<bool> canAct)
//        {
//            Owner = owner;
//            _canAct = canAct ?? (() => true);
//            _aStar = new AStar();
//            TargetBlock = targetBlock;
//        }
//
//        private bool CheckChoice(Block block, ICollection<Card> cards)
//            => cards.Count > 0 && block is {IsWalkable: true};
//
//        private static (Card card, Block block) FindSelected(IEnumerable<Card> cards, ICollection<Block> blocks)
//        {
//            var usedColors = new HashSet<GameColor>();
//
//            foreach (var card in cards)
//            {
//                if(usedColors.Contains(card.Color))
//                    continue;
//
//                var block = CardBlockMatcher.GetSuitable(card, blocks);
//                if (block != null)
//                    return (card, block);
//
//                usedColors.Add(card.Color);
//            }
//
//            return (null, null);
//        }
//
//        private (Card, Block) TryFromPlayerOrTarget(List<Card> trapCards, ICollection<Block> possibleBlocks)
//        {
//            var result = FindSelected(trapCards, possibleBlocks);
//            if (result.card == null)
//                result = FindSelected(trapCards, TargetBlock.Neighbors);
//            return result;
//        }
//
//        private (Card, Block) TryFromTargetOrPlayer(List<Card> trapCards, ICollection<Block> possibleBlocks)
//        {
//            var result = FindSelected(trapCards, TargetBlock.Neighbors);
//            if (result.card == null)
//                result = FindSelected(trapCards, possibleBlocks);
//
//            return result;
//        }
//
//        public (Card card, Block block) FindTrapData(IEnumerable<Card> playerCards, ICollection<Block> possibleBlocks)
//        {
//            var trapCards = new List<Card>(playerCards.Where(c => c.IsTrap));
//
//            var preferTarget = Random.value < 0.5f;
//
//            (Card card, Block block) result = preferTarget
//                ? TryFromTargetOrPlayer(trapCards, possibleBlocks)
//                : TryFromPlayerOrTarget(trapCards, possibleBlocks);
//
//            if (result.card == null || result.block == null)
//            {
//                foreach (var card in trapCards)
//                {
//                    var farBlock = _aStar.FindFarthestBlock(Owner.CurrentBlock, card.Color);
//                    if (farBlock != null)
//                        return (card, farBlock);
//                }
//            }
//            
//            if(result.block != null)
//                _trapBlocks.Add(result.block);
//
//            return result;
//        }
//
//        public (Card card, Block block) FindDataForMoving(IEnumerable<Card> playerCards, ICollection<Block> possibleBlocks)
//        {
//            if (!TargetBlock.IsWalkable || CurrentPath == null)
//                return (null, null);
//
//            var moveCards = new List<Card>(playerCards.Where(c => c.IsMove));
//            var block = CurrentPath.GetNext();
//
//            if (!CheckChoice(block, moveCards))
//                return (null, null);
//
//            foreach (var card in moveCards)
//            {
//                if (block!.Color == card.Color && !_recentBlocks.Contains(block))
//                {
//                    _recentBlocks.Add(Owner.CurrentBlock);
//                    return (card, block);
//                }
//            }
//
//            foreach (var card in moveCards)
//            {
//                var altBlock = CardBlockMatcher.GetFirstNeighbor(card, Owner.CurrentBlock);
//                if (altBlock != null)
//                {
//                    _recentBlocks.Add(Owner.CurrentBlock);
//                    return (card, altBlock);
//                }
//            }
//
//            if (_canAct != null && !_canAct())
//                return (null, null);
//
//            foreach (var neighbor in Owner.CurrentBlock.Neighbors)
//            {
//                if(!neighbor.IsWalkable || _recentBlocks.Contains(neighbor))
//                    continue;
//
//                if(possibleBlocks.Any(n => n == neighbor))
//                    continue;
//
//                foreach (var card in moveCards)
//                {
//                    if (neighbor.Color == card.Color)
//                    {
//                        _recentBlocks.Add(Owner.CurrentBlock);
//                        return (card, neighbor);
//                    }
//                }
//            }
//
//            return (null, null);
//        }
//
//        public void UpdatePath(MonoBehaviour context, Action onComplete = null)
//        {
//            if(_pathRoutine != null)
//                context.StopCoroutine(_pathRoutine);
//
//            _pathRoutine = context.StartCoroutine(_aStar.FindPathAsync(
//                Owner.CurrentBlock,
//                TargetBlock,
//                path =>
//                {
//                    CurrentPath = path;
//                    onComplete?.Invoke();
//                },
//                maxSteps: 2000,
//                maxMillisecsPerFrame: 5f
//            ));
//        }
//     }
// }