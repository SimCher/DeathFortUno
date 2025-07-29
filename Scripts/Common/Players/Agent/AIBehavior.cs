using System;
using System.Collections.Generic;
using System.Linq;
using DeathFortUnoCard.Scripts.CardField.Blocks;
using DeathFortUnoCard.Scripts.CardField.Blocks.Storages;
using DeathFortUnoCard.Scripts.CardField.Cards;
using DeathFortUnoCard.Scripts.CardField.Checkers;
using DeathFortUnoCard.Scripts.Common.Extensions;
using DeathFortUnoCard.Scripts.Common.Players.AI.Pathfinding;
using DeathFortUnoCard.UI.Scripts.Components;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace DeathFortUnoCard.Scripts.Common.Players.Agent
{
    [RequireComponent(typeof(AIPlayer))]
    public class AIBehavior : MonoBehaviour
    {
        [SerializeField] private AISettings settings;

        private const int EasyTrapMemory = 2;
        private const int MediumTrapMemory = 5;
        private const int HardTrapMemory = 10;
        
        private AIPlayer _ai;
        private BlockStorage _blockStorage;
        private HashSet<Block> _memoryTraps = new();
        private Dictionary<Block, float> _suspectedEnemyTraps = new();
        private TurnController _turnController;
        private MoveChecker _moveChecker;

        private bool _hasKnife;
        private bool _hasDrawnThisTurn;

        public UnityEvent onMoveOver;
        public UnityEvent onTakedNewCard;

        public AIDifficulty Difficulty => settings.difficulty;

        private int TrapMemoryLimit => settings.difficulty switch
        {
            AIDifficulty.Easy => EasyTrapMemory,
            AIDifficulty.Medium => MediumTrapMemory,
            _ => HardTrapMemory
        };
        
        private void Awake()
        {
            _blockStorage = ServiceLocator.ServiceLocator.Get<BlockStorage>();
            _turnController = ServiceLocator.ServiceLocator.Get<TurnController>();
            _moveChecker = ServiceLocator.ServiceLocator.Get<MoveChecker>();
            _ai = GetComponent<AIPlayer>();
        }

        private void OnDestroy()
        {
            onMoveOver.RemoveAllListeners();
            onTakedNewCard.RemoveAllListeners();
        }

        private void UpdateTrapPredictions()
        {
            var keys = _suspectedEnemyTraps.Keys.ToArray();
            var trapRate = settings.difficulty switch
            {
                AIDifficulty.Easy => 0.65f,
                AIDifficulty.Medium => 0.85f,
                AIDifficulty.Hard => 0.95f,
                _ => 0.85f
            };

            for (int i = 0; i < keys.Length; i++)
            {
                var block = keys[i];
                _suspectedEnemyTraps[block] *= trapRate;

                if (_suspectedEnemyTraps[block] < 0.1f)
                    _suspectedEnemyTraps.Remove(block);
            }
        }

        private bool HasKnife()
        {
            //TODO: Подключить флаг владения
            return false;
        }

        private float EvaluateThreat(Player player)
        {
            if (player == _ai)
                return 0f;

            var threat = 0f;
            
            GetView(player.CurrentBlock, out var playerBlock);
            GetView(_ai.CurrentBlock, out var selfBlock);

            var distance = Vector3.Distance(playerBlock.GetPosition(), selfBlock.GetPosition());
            threat += 30f / (distance + 1f); //ближе = опаснее
            
            //Нож
            if (_turnController.KillerPlayer == player)
                threat += 50f;

            if (player.Data.Hand.IsOverflowing)
                threat += 5f;
            else
                threat += 3f;

            return threat;

        }

        private Player GetMostThreateningEnemy()
        {
            Player mostThreatening = null;
            var highestThreat = float.MinValue;

            for (int i = 0; i < _turnController.Players.Count; i++)
            {
                var p = _turnController.Players[i];
                if(p == _ai)
                    continue;

                var threat = EvaluateThreat(p);
                if (threat > highestThreat)
                {
                    highestThreat = threat;
                    mostThreatening = p;
                }
            }

            return mostThreatening;
        }

        private void GetView(Block block, out BlockView view) => _blockStorage.TryGetView(block, out view);

        private void HandleOverflowFallback()
        {
            var hand = _ai.Data.Hand;
            var cards = hand.Cards;
            
            //Ищем карту-ловушку
            Card trapCard = null;
            foreach (var card in cards) 
            {
                if (card.IsTrap)
                {
                    trapCard = card;
                    break;
                }
            }

            if (trapCard == null)
            {
                Debug.Log("AI: Нет ловушек для избавления - передаю ход");
                onMoveOver?.Invoke();
                return;
            }

            Block bestBlock = null;
            var bestScore = float.NegativeInfinity;

            var blocksDict = _blockStorage.Blocks;

            foreach (var (block, _) in blocksDict)
            {
                if(block is {IsWalkable: false})
                    continue;
                
                if(block == _blockStorage.TargetBlock || block == _ai.CurrentBlock)
                    continue;
                
                if(_memoryTraps.Contains(block))
                    continue;
                if(block.Color != trapCard.Color)
                    continue;

                var score = ScoreOverflowTrapBlock(block);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestBlock = block;
                }
            }

            if (bestBlock == null)
            {
                Debug.Log("AI: Нет подходящих блоков для сброса ловушки - передаём ход");
                onMoveOver?.Invoke();
                return;
            }
            
            hand.SetSelectedCard(trapCard);
            _moveChecker.OnBlockSelected(bestBlock);
        }
        
        private HashSet<Block> GetPathToEnemy(Block from, Block to)
        {
            var pathfinder = new AStar();
            var path = pathfinder.FindPath(from, to);

            return path != null ? new HashSet<Block>(path) : new HashSet<Block>();
        }

        private Player GetEnemy()
        {
            var players = _turnController.Players;
            if (players.Count == 2)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i] != _ai)
                        return players[i];
                }
            }
            else if (players.Count > 2)
            {
                return GetMostThreateningEnemy();
            }
            
            Debug.LogError($"Недопустимое количество игроков обнаружено в {nameof(AIBehavior)}");
            return null;
        }

        private float ScoreOverflowTrapBlock(Block block)
        {
            var score = 0f;

            var enemy = GetEnemy();
            
            GetView(block, out var blockView);
            GetView(enemy.CurrentBlock, out var enemyView);
            GetView(_ai.CurrentBlock, out var myView);

            var blockPos = blockView.GetPosition();
            var distToEnemy = Vector3.Distance(blockPos, enemyView.GetPosition());
            var distToSelf = Vector3.Distance(blockPos, myView.GetPosition());

            switch (settings.playStyle)
            {
                case AIPlayStyle.Aggressive:
                    score = 100f - distToEnemy; //ближе к врагу - лучше
                    break;
                case AIPlayStyle.Defensive:
                    score = distToEnemy; //подальше от врага
                    break;
                case AIPlayStyle.Trickster:
                    score = Mathf.Abs(distToEnemy - distToSelf); //вбок от обоих
                    break;
                case AIPlayStyle.Balanced:
                    score = 50f - Mathf.Abs(distToEnemy - 3f); //около средней дистанции
                    break;
                case AIPlayStyle.Randomized:
                    score = Random.Range(0f, 100f);
                    break;
            }

            if (_suspectedEnemyTraps.ContainsKey(block))
                score -= 20f;

            return score;
        }
        
        private float ScoreBlock(Block block, Card card)
        {
            var score = 0f;

            var targetBlock = _blockStorage.TargetBlock;
            var enemy = GetEnemy();
            
            GetView(block, out var blockView);
            GetView(targetBlock, out var targetView);
            GetView(enemy.CurrentBlock, out var enemyView);
            
            var distToTarget = Vector3.Distance(blockView.GetPosition(), targetView.GetPosition());
            var distToEnemy = Vector3.Distance(blockView.GetPosition(), enemyView.GetPosition());
            
            // ===== БАЗА: близость к целям =====
            score += 10f / (distToTarget + 1);

            if (HasKnife())
            {
                //агрессия при наличии ножа
                score += distToEnemy < 1.5f ? 50 : 0;
            }
            
            // ===== СТИЛЕВОЕ ПОВЕДЕНИЕ =====

            switch (settings.playStyle)
            {
                case AIPlayStyle.Aggressive:
                    score += (6f - distToEnemy) * 2f; //чем ближе к врагу - тем лучше
                    break;
                case AIPlayStyle.Defensive:
                    score -= (5f - distToEnemy) * 2f; //чем ближе к врагу - тем хуже
                    if (distToTarget < 2f)
                        score += 5f; //но к центру идти можно
                    break;
                case AIPlayStyle.Trickster:
                    var isSideStep = !_ai.CurrentBlock.Neighbors.Contains(block)
                                     && block.Neighbors.Any(n => _ai.CurrentBlock.Neighbors.Contains(n));

                    if (isSideStep)
                        score += 7f; //любит "шаг вбок"
                    if (distToEnemy < 2f)
                        score += 3f; //пугаем игрока
                    break;
                
                case AIPlayStyle.Balanced:
                    score += 2f / (distToEnemy + 1); //немного стремится к противнику
                    score += 2f / (distToTarget + 1); //немного к центру
                    break;
                
                case AIPlayStyle.Randomized:
                    score += Random.Range(-4f, 4f);
                    break;
            }
            
            // ===== РИСКИ =====
            if (_suspectedEnemyTraps.TryGetValue(block, out var risk))
            {
                var trapPenalty = risk * settings.difficulty switch
                {
                    AIDifficulty.Easy => 5f,
                    AIDifficulty.Medium => 15f,
                    AIDifficulty.Hard => 30f,
                    _ => 15f
                };
                score -= trapPenalty;
            }
            if (_memoryTraps.Contains(block))
                score -= 10f;

            return score;
        }

        private float ScoreTrapBlock(Block block, Card card)
        {
            var score = 0f;
            var enemy = GetEnemy();
            var pathToEnemy = GetPathToEnemy(_ai.CurrentBlock, enemy.CurrentBlock);
            
            GetView(block, out var blockView);
            GetView(enemy.CurrentBlock, out var enemyView);

            var distToEnemy = Vector3.Distance(blockView.GetPosition(), enemyView.GetPosition());
            
            // ===== ОБЩИЕ ЧЕРТЫ =====
            score += 3f / (distToEnemy + 1);
            
            if (_suspectedEnemyTraps.TryGetValue(block, out var risk))
                score -= risk * 10f;
            
            if (_memoryTraps.Contains(block))
                score -= 100f;
            
            // ===== ОЦЕНКА ЗАМЕНЫ ЛОВУШКИ =====
            if (block.IsTrapped)
            {
                var replacePenalty = 0f;
                
                //Стиль влияет на готовность заменить
                switch (settings.playStyle)
                {
                    case AIPlayStyle.Aggressive:
                        replacePenalty += 15f; //обычно не заменяет
                        break;
                    case AIPlayStyle.Trickster:
                        replacePenalty += 5f; //готов заменять
                        break;
                    case AIPlayStyle.Balanced:
                        replacePenalty += 25f; //редко заменяет
                        break;
                    case AIPlayStyle.Randomized:
                        replacePenalty += Random.Range(10f, 30f); //хаотично
                        break;
                }
                
                //Уровень сложности уменьшает или увеличивает наказание
                switch (settings.difficulty)
                {
                    case AIDifficulty.Easy:
                        replacePenalty *= 2f; //почти никогда не заменяет
                        break;
                    case AIDifficulty.Medium:
                        replacePenalty *= 1f;
                        break;
                    case AIDifficulty.Hard:
                        replacePenalty *= 0.3f; //легко заменяет, если выгодно
                        break;
                }

                score -= replacePenalty;
            }
            
            // ==== СТИЛЕВОЕ ПОВЕДЕНИЕ ====
            switch (settings.playStyle)
            {
                case AIPlayStyle.Aggressive:
                    if (pathToEnemy.Contains(block) && distToEnemy > 3f)
                        score += 10f; //защита на дальних подступах
                    break;
                case AIPlayStyle.Trickster:
                    if (block.Neighbors.Any(nb => pathToEnemy.Contains(nb)))
                        score += 12f; //ставим рядом с путём
                    if (distToEnemy < 2f)
                        score += 7f; //пугаем игрока, если он рядом
                    break;
                case AIPlayStyle.Balanced:
                    if (!pathToEnemy.Contains(block) && block.Neighbors.Any(nb => pathToEnemy.Contains(nb)))
                        score += 5f; //аккуратная игра рядом с путём
                    break;
                
                case AIPlayStyle.Randomized:
                    score += Random.Range(-5f, 5f); //немного случайности
                    break;
            }

            return score;
        }

        private List<AIMoveOption> EvaluateMoveOptions(Block currentBlock, PlayerHand hand)
        {
            var result = new List<AIMoveOption>();

            foreach (var card in hand.Cards)
            {
                if(!card.IsMove)
                    continue;

                foreach (var neighbor in currentBlock.Neighbors)
                {
                    if(!neighbor.IsWalkable)
                        continue;
                    
                    if(neighbor.Color != card.Color)
                        continue;

                    var score = ScoreBlock(neighbor, card);
                    result.Add(new AIMoveOption
                    {
                        card = card,
                        target =  neighbor,
                        score = score
                    });
                }
            }

            return result;
        }

        private List<AIMoveOption> EvaluateTrapOptions(PlayerHand hand)
        {
            var trapCards = hand.Cards.Where(c => c.IsTrap);
            var result = new List<AIMoveOption>();

            foreach (var card in trapCards)
            {
                foreach (var (block, _) in _blockStorage.Blocks)
                {
                    if(!block.IsWalkable)
                        continue;
                    
                    if(block == _blockStorage.TargetBlock)
                        continue;
                    
                    if(_turnController.Players.Any(p => p.CurrentBlock == block))
                        continue;
                    
                    if(block.Color != card.Color)
                        continue;

                    var score = ScoreTrapBlock(block, card);
                    result.Add(new AIMoveOption
                    {
                        card = card,
                        target = block,
                        score = score
                    });
                }
            }

            return result;
        }

        private List<AIMoveOption> EvaluateKnifeMove(Block currentBlock, PlayerHand hand, Block enemyBlock)
        {
            var result = new List<AIMoveOption>();

            foreach (var card in hand.Cards)
            {
                if(!card.IsMove)
                    continue;

                foreach (var neighbor in currentBlock.Neighbors)
                {
                    if(!neighbor.IsWalkable)
                        continue;
                    if(neighbor.Color != card.Color)
                        continue;
                    
                    GetView(neighbor, out var neighborView);
                    GetView(enemyBlock, out var enemyView);
                    var dist = Vector3.Distance(neighborView.GetPosition(), enemyView.GetPosition());
                    var score = 100f / (dist + 1);
                    
                    result.Add(new AIMoveOption
                    {
                        card = card,
                        target = neighbor,
                        score = score
                    });
                }
            }
            return result;
        }

        private void TryKnifeAttack(AIPlayer player)
        {
            var enemy = GetEnemy();
            var enemyBlock = enemy.CurrentBlock;
            
            //Если противник рядом - атакуем
            if (player.CurrentBlock.Neighbors.Contains(enemyBlock))
            {
                Debug.Log("AI: Атакую ножом!");
                //TODO: player.Data.ActionSystem.AttackWithKnife(enemy);
                onMoveOver?.Invoke();
                return;
            }
            
            //Если не рядом - двигаемся к врагу
            var moveOptions = EvaluateKnifeMove(player.CurrentBlock, player.Data.Hand, enemyBlock);
            if (moveOptions.Count > 0)
            {
                SortByScoreDescending(moveOptions);
                var best = moveOptions[0];
                player.Data.Hand.SetSelectedCard(best.card);
                _moveChecker.OnBlockSelected(best.target);
                return;
            }
            
            //Не можем походить - пробуем взять карту
            if (!_hasDrawnThisTurn)
            {
                _hasDrawnThisTurn = true;
                if (player.Data.Hand.TryDrawCard())
                {
                    onTakedNewCard?.Invoke();
                    return;
                }
            }
            
            //Нет карты для движения - утилизируем карту через ловушку
            var trapOptions = EvaluateTrapOptions(player.Data.Hand);
            if (trapOptions.Count > 0)
            {
                SortByScoreDescending(trapOptions);
                var bestTrap = trapOptions[0];
                player.Data.Hand.SetSelectedCard(bestTrap.card);
                _moveChecker.OnBlockSelected(bestTrap.target);
                return;
            }
            
            //Ничего не можем - конец хода
            Debug.Log("AI: Не могу атаковать и не могу приблизиться");
            _hasDrawnThisTurn = false;
            onMoveOver?.Invoke();
        }

        private void SortByScoreDescending(List<AIMoveOption> list) => list.Sort((a, b) => b.score.CompareTo(a.score));

        private void SortByScoreWithRandomness(List<AIMoveOption> list)
        {
            const float noiseFactor = 3f; //насколько сильно можно "сдвинуть" score

            var count = list.Count;
            for (int i = 0; i < count; i++)
            {
                var noise = Random.Range(-noiseFactor, noiseFactor);
                list[i].score += noise;
            }
            
            SortByScoreDescending(list);
        }

        private void ApplyDifficultyNoise(ref List<AIMoveOption> options)
        {
            if (options is {Count: <= 1})
                return;

            var difficulty = settings.difficulty;

            if (difficulty == AIDifficulty.Easy)
            {
                if (Random.value < 0.4f) //40% шанс на случайный ход
                {
                    options.Shuffle();
                    return;
                }
            }
            else if (difficulty == AIDifficulty.Medium)
            {
                if (Random.value < 0.15f) // 15% шанс на "ошибку"
                {
                    SortByScoreWithRandomness(options);
                    return;
                }
            }

            SortByScoreDescending(options);
        }

        public void OnKnifePickedUp() => _hasKnife = true;

        public void OnKnifeLost() => _hasKnife = false;

        public void TakeTurn(AIPlayer player)
        {
            UpdateTrapPredictions();

            if (_hasKnife)
            {
                TryKnifeAttack(player);
                return;
            }

            //Попытка походить
            var moveOptions = EvaluateMoveOptions(player.CurrentBlock, player.Data.Hand);

            ApplyDifficultyNoise(ref moveOptions);
            
            if (moveOptions.Count > 0)
            {
                _hasDrawnThisTurn = false;
                var best = moveOptions[0];
                player.Data.Hand.SetSelectedCard(best.card);
                _moveChecker.OnBlockSelected(best.target);
                return;
            }
            
            //Если нельзя ходить - попробовать поставить ловушку
            var trapOptions = EvaluateTrapOptions(player.Data.Hand);
            if (trapOptions.Count > 0)
            {
                _hasDrawnThisTurn = false;
                SortByScoreDescending(trapOptions);
                var best = trapOptions[0];
                player.Data.Hand.SetSelectedCard(best.card);
                _moveChecker.OnBlockSelected(best.target);
                return;
            }

            if (_ai.Data.Hand.IsOverflowing)
            {
                Debug.Log("AI: рука скоро переполнится - избавляемся от ловушки");
                HandleOverflowFallback();
                return;
            }
            
            //Если ещё не тянули карту - тянем и пробуем заново
            if (!_hasDrawnThisTurn)
            {
                Debug.Log("AI: Нет вариантов - пробую взять карту");
                _hasDrawnThisTurn = true;

                if (player.Data.Hand.TryDrawCard())
                {
                    onTakedNewCard?.Invoke();
                    return;
                }
                else
                {
                    Debug.LogWarning("AI: колода пуста, не удалось взять карту.");
                }
            }
            
            //Конец хода
            Debug.Log("AI: Завершаю ход, нечем ходить и нечего ставить");
            _hasDrawnThisTurn = false;
            onMoveOver?.Invoke();
        }

        public void OnTrapPlaced(Block block, Player trapOwner)
        {
            if (trapOwner == _ai)
            {
                if (block.IsTrapped)
                    _memoryTraps.Remove(block);
                else
                {
                    _memoryTraps.Add(block);
                    while (_memoryTraps.Count > TrapMemoryLimit)
                    {
                        var enumerator = _memoryTraps.GetEnumerator();
                        enumerator.MoveNext();
                        _memoryTraps.Remove(enumerator.Current);
                    }
                }
                
            }
            else
            {
                if (block.IsTrapped)
                    _suspectedEnemyTraps[block] = 1f;
                else _suspectedEnemyTraps[block] = 0f;
            }
        }

        [Serializable]
        private class AIMoveOption
        {
            public Card card;
            public Block target;
            public float score;
        }
    }
}