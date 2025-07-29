using System;
using System.Collections.Generic;
using DeathFortUnoCard.Scripts.CardField.Blocks;
using DeathFortUnoCard.Scripts.Common.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.Common.Players.AI.Pathfinding
{
    public class AStar
    {
        private static int Heuristic(Block a, Block b) =>
            Math.Abs(a.Coords.x - b.Coords.x) + Math.Abs(a.Coords.y - b.Coords.y);

        private static Path ReconstructPath(Dictionary<Block, Block> cameFrom, Block current)
        {
            var totalPath = new List<Block> {current};
            while (cameFrom.TryGetValue(current, out var prev))
            {
                current = prev;
                totalPath.Add(current);
            }
            totalPath.Reverse();
            return new Path(totalPath);
        }

        private Block Find(
            Block start,
            Predicate<Block> targetPredicate,
            out Path resultPath,
            bool preferFarthest = false)
        {
            resultPath = null;

            var openSet = new PriorityQueue<Block>();
            var openSetTracker = new HashSet<Block>();
            var closedSet = new HashSet<Block>();

            var cameFrom = new Dictionary<Block, Block>();
            var gScore = new Dictionary<Block, int>();
            var fScore = new Dictionary<Block, int>();

            gScore[start] = 0;
            fScore[start] = 0;
            
            openSet.Enqueue(start, 0);
            openSetTracker.Add(start);

            Block bestTarget = null;
            var maxDistance = 0;

            while (openSet.Count > 0)
            {
                var current = openSet.Dequeue();
                openSetTracker.Remove(current);

                if (targetPredicate(current))
                {
                    var distance = Heuristic(start, current);

                    if (!preferFarthest)
                    {
                        resultPath = ReconstructPath(cameFrom, current);
                        return current;
                    }
                    else if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        bestTarget = current;
                    }
                }

                closedSet.Add(current);

                foreach (var neighbor in current.Neighbors)
                {
                    if(closedSet.Contains(neighbor) || !neighbor.IsWalkable)
                        continue;

                    var tentativeG = gScore[current] + 1;

                    if (!gScore.TryGetValue(neighbor, out var existingG) || tentativeG < existingG)
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeG;
                        fScore[neighbor] = tentativeG + Heuristic(neighbor, preferFarthest ? start : neighbor);

                        if (openSetTracker.Add(neighbor))
                        {
                            openSet.Enqueue(neighbor, fScore[neighbor]);
                        }
                    }
                }
            }

            if (preferFarthest && bestTarget != null)
                return bestTarget;

            return null;
        }

        public Block FindFarthestBlock(Block start, GameColor color) =>
            Find(start, b => b.Color == color, out _, preferFarthest: true);

        [CanBeNull]
        public Path FindPath(Block start, Block goal)
        {
            Find(start, b => b == goal, out var path);
            return path;
        }

        public System.Collections.IEnumerator FindPathAsync(
            Block start,
            Block goal,
            Action<Path> onComplete,
            Func<bool> shouldCancel = null,
            int maxSteps = 10000,
            float maxMillisecsPerFrame = 5f)
        {
            var openSet = new PriorityQueue<Block>();
            var openSetTracker = new HashSet<Block>();
            var closedSet = new HashSet<Block>();
            var cameFrom = new Dictionary<Block, Block>();
            var gScore = new Dictionary<Block, int>();
            var fScore = new Dictionary<Block, int>();

            gScore[start] = 0;
            fScore[start] = Heuristic(start, goal);
            openSet.Enqueue(start, fScore[start]);
            openSetTracker.Add(start);

            var steps = 0;
            var stopwatch = new System.Diagnostics.Stopwatch();

            while (openSet.Count > 0)
            {
                if (++steps > maxSteps)
                {
                    Debug.LogWarning($"A* {nameof(FindPathAsync)}: Достигнуто макс. кол-во шагов, прерываю.");
                    onComplete?.Invoke(null);
                    yield break;
                }

                if (shouldCancel != null && shouldCancel())
                {
                    Debug.Log($"A* {nameof(FindPathAsync)}: Внешняя отмена.");
                    onComplete?.Invoke(null);
                    yield break;
                }
                
                stopwatch.Restart();

                var current = openSet.Dequeue();
                openSetTracker.Remove(current);

                if (current == goal)
                {
                    var path = ReconstructPath(cameFrom, current);
                    onComplete?.Invoke(path);
                    yield break;
                }

                closedSet.Add(current);

                foreach (var neighbor in current.Neighbors)
                {
                    if(closedSet.Contains(neighbor) || !neighbor.IsWalkable)
                        continue;

                    var tentativeG = gScore[current] + 1;

                    if (!gScore.TryGetValue(neighbor, out var g) || tentativeG < g)
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeG;
                        fScore[neighbor] = tentativeG + Heuristic(neighbor, goal);

                        if (openSetTracker.Add(neighbor))
                        {
                            openSet.Enqueue(neighbor, fScore[neighbor]);
                        }
                    }
                }
                
                stopwatch.Stop();
                if (stopwatch.ElapsedMilliseconds > maxMillisecsPerFrame)
                    yield return null;
            }
            
            onComplete?.Invoke(null);
        }
    }
}