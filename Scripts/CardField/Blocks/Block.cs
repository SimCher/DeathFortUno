using System;
using System.Collections.Generic;
using DeathFortUnoCard.Scripts.CardField.Blocks.Interfaces;
using DeathFortUnoCard.Scripts.Common;
using DeathFortUnoCard.Scripts.Common.Players;
using JetBrains.Annotations;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.CardField.Blocks
{
    [Serializable]
    public class Block : IBlockDataSubject, IBlock
    {
        [field: SerializeField]
        public Vector2Int Coords { get; set; }
        
        [field: SerializeField]
        public GameColor Color { get; private set; }

        [field: SerializeField] public bool IsWalkable { get; private set; } = true;
        
        [field: SerializeField]
        public bool IsTarget { get; private set; }

        public bool IsTrapped => (bool)TrapOwner;
        
        [CanBeNull] public Player TrapOwner { get; private set; }

        public HashSet<Block> Neighbors { get; private set; } = new();
        
        [field: SerializeField]
        public event Action StateChanged;

        public enum WalkState
        {
            Walkable,
            Target
        };
        
        public void OnDestroy()
        {
            StateChanged = null;
        }

        private void ChangeState(WalkState state, bool newValue)
        {
            switch (state)
            {
                case WalkState.Target:
                    IsTarget = newValue;
                    break;
                case WalkState.Walkable:
                    IsWalkable = newValue;
                    break;
            }
        }

        private void SetTrapOwner(Player newTrapOwner)
        {
            if (IsTrapped)
            {
                TrapOwner = null;
                return;
            }

            TrapOwner = newTrapOwner;
        }

        public bool IsNeighbor(Block blockForCheck)
            => Neighbors.Contains(blockForCheck);

        private void AddNeighbor(Block newNeighborBlock)
        {
            if (IsNeighbor(newNeighborBlock))
                return;

            Neighbors.Add(newNeighborBlock);
        }

        public void SetColor(GameColor color)
        {
            Color = color;
            StateChanged?.Invoke();
        }

        public void SetCoords(int row, int col) => SetCoords(new Vector2Int(row, col));

        public void SetCoords(Vector2Int newCoords) => Coords = newCoords;

        public void ChangeWalkState(bool state)
        {
            ChangeState(WalkState.Walkable, state);
            StateChanged?.Invoke();
        }

        public void ChangeTargetState(bool state)
        {
            ChangeState(WalkState.Target, state);
            StateChanged?.Invoke();
        }

        public void SwitchTrapState(Player trapOwner)
        {
            SetTrapOwner(trapOwner);
            StateChanged?.Invoke();
        }

        public void SetNeighbor(Block neighborBlock)
        {
            if (neighborBlock == null)
                throw new NullReferenceException();
            
            AddNeighbor(neighborBlock);
        }

        public bool CoordsEquals(Block other) => Coords == other.Coords;
    }
}