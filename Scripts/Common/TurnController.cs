using System;
using System.Collections.Generic;
using System.Linq;
using DeathFortUnoCard.Scripts.Common.Managers;
using DeathFortUnoCard.Scripts.Common.Players;
using DeathFortUnoCard.Scripts.Common.Players.Agent;
using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace DeathFortUnoCard.Scripts.Common
{
    public class TurnController : MonoBehaviour, IService
    {
#if UNITY_EDITOR
        public enum DebugPlayer
        {
            Player, NPC
        }

        public DebugPlayer trappedPlayer;
        public DebugPlayer trapOwnerPlayer;
#endif
        [SerializeField] private Player[] players;
        [SerializeField] private int currentPlayerIndex;

        public UnityEvent onTurnOver;
        public UnityEvent<Player, Player> onTrapStateInitiated;
        public UnityEvent<Player, Player> onTrapStateInitiatedByAI;
        
        private GameFlowManager _gameManager;

        public IReadOnlyList<Player> Players => players;
        
        [CanBeNull]
        public Player KillerPlayer { get; private set; }

        public int CurrentPlayerIndex
        {
            get => currentPlayerIndex;
            private set => currentPlayerIndex = Mod(value, players.Length);
        }

        private int NextPlayerIndex => Mod(currentPlayerIndex + 1, players.Length);

        private int PrevPlayerIndex => Mod(currentPlayerIndex - 1, players.Length);

        public Player PrevPlayer => players[PrevPlayerIndex];
        public Player CurrentPlayer => players[CurrentPlayerIndex];
        public Player NextPlayer => players[NextPlayerIndex];

        public bool IsThisPlayerTurn => ThisPlayer == CurrentPlayer;
        
        public static HumanPlayer ThisPlayer { get; set; }

        private void Awake()
        {
            ServiceLocator.ServiceLocator.Register(this);
        }

        private void Start()
        {
            _gameManager = ServiceLocator.ServiceLocator.Get<GameFlowManager>();
            ThisPlayer = FindObjectsByType<HumanPlayer>(FindObjectsSortMode.None)
                .First(p => p.ID.Name == "Human");
        }
#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                var trapped = trappedPlayer == DebugPlayer.Player
                    ? ThisPlayer
                    : NextPlayer;
                
                var trapOwner = trapOwnerPlayer == DebugPlayer.NPC
                    ? NextPlayer
                    : ThisPlayer;
                
                // var duelController = ServiceLocator.ServiceLocator.Get<DuelController>();
                // duelController.SetPlayers(trapped, trapOwner);
                
                onTrapStateInitiated?.Invoke(trapped, trapOwner);
                //
                // if (trapped == trapOwner)
                // {
                //     _gameManager.StartSuicide();
                // }
                // else
                //     _gameManager.StartDuel();
            }
        }
#endif
        

        private void OnDestroy()
        {
            onTurnOver.RemoveAllListeners();
            onTrapStateInitiated.RemoveAllListeners();
        }

        private static int Mod(int x, int m) => (x % m + m) % m;

        private void ChangeTurnPlayer()
        {
            switch (CurrentPlayer.Data.BonusTurns)
            {
                case > 0:
                    CurrentPlayer.Data.BonusTurns--;
                    break;
                case 0:
                    CurrentPlayerIndex++;
                    break;
            }

            if (NextPlayer.Data.SkipNextTurn)
            {
                NextPlayer.Data.SkipNextTurn = false;
                CurrentPlayerIndex += 2;
            }
        }
        
        public void AssignPlayers(Player[] playerArray)
        {
            players = new Player[playerArray.Length];

            for (int i = 0; i < playerArray.Length; i++)
            {
                var human = playerArray[i] as HumanPlayer;
                if (human)
                    ThisPlayer = human;

                players[i] = playerArray[i];
            }
        }

        public void OnTurnOver()
        {
            Debug.Log($"<color=black>{nameof(OnTurnOver)}</color>: Trace:\n{Environment.StackTrace}");
            var currentPlayerBlock = CurrentPlayer.CurrentBlock;

            if (currentPlayerBlock.IsTarget)
            {
                KillerPlayer = CurrentPlayer;
                currentPlayerBlock.ChangeTargetState(false);
            }
            
            if (currentPlayerBlock.IsTrapped)
            {
                var ai = CurrentPlayer as AIPlayer;

                if (ai)
                {
                    onTrapStateInitiatedByAI?.Invoke(ai, currentPlayerBlock.TrapOwner);
                    currentPlayerBlock.SwitchTrapState(null);
                }
                else
                {
                    onTrapStateInitiated?.Invoke(CurrentPlayer, currentPlayerBlock.TrapOwner);
                    currentPlayerBlock.SwitchTrapState(null);
                }
            }
            else
            {
                ChangeTurnPlayer();
                _gameManager.ChangeTurn();
            }
        }

        public void ChangeTurnOrStartSuicide(int bulletsCount)
        {
            if(bulletsCount == 0)
                OnTurnOver();
            else
            {
                _gameManager.StartSuicide();
            }
        }

        public void ChangeTurnOrStartDuel(bool isChangeTurn)
        {
            if(isChangeTurn)
                OnTurnOver();
            else
                _gameManager.StartDuel();
        }
    }
}