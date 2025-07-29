using System.Linq;
using DeathFortUnoCard.Scripts.CardField.Blocks;
using DeathFortUnoCard.Scripts.CardField.Blocks.Data;
using DeathFortUnoCard.Scripts.CardField.Blocks.Storages;
using DeathFortUnoCard.Scripts.CardField.Cards;
using DeathFortUnoCard.Scripts.CardField.Cards.Generation;
using DeathFortUnoCard.Scripts.CardField.Cards.Storages;
using DeathFortUnoCard.Scripts.CardField.Checkers;
using DeathFortUnoCard.Scripts.CardField.Dealers;
using DeathFortUnoCard.Scripts.CardField.Generation;
using DeathFortUnoCard.Scripts.Common.Generation.Services;
using DeathFortUnoCard.Scripts.Common.Players;
using DeathFortUnoCard.Scripts.Common.Players.Agent;
using DeathFortUnoCard.Scripts.Common.Players.AI;
using DeathFortUnoCard.Scripts.Common.Players.Inputs;
using DeathFortUnoCard.Scripts.Duel.Effects;
using DeathFortUnoCard.Scripts.Utils;
using DeathFortUnoCard.UI.Scripts;
using DeathFortUnoCard.UI.Scripts.Components;
using DeathFortUnoCard.UI.Scripts.Managers;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.Common.Generation
{
    [DefaultExecutionOrder(-500)]
    public class CommonGenerator : OneShotBehaviour
    {
        [SerializeField] private ColorGameSettings colorSettings;
        [SerializeField] private FieldSettings fieldSettings;
        [SerializeField] private CardTypeSettings cardTypeSettings;
        // [SerializeField] private CardSettings cardSettings;
        protected override void Run()
        {
            var size = fieldSettings.Size;
            
            var blockStorage = ServiceLocator.ServiceLocator.Get<BlockStorage>();
            var cardStorage = ServiceLocator.ServiceLocator.Get<CardStorage>();
            var gameField = ServiceLocator.ServiceLocator.Get<GameField>();
            var turnController = ServiceLocator.ServiceLocator.Get<TurnController>();

            var playerGenerator = ServiceLocator.ServiceLocator.Get<PlayerGenerator>();
            var blockController = ServiceLocator.ServiceLocator.Get<BlockController>();
            
            var gameDeck = ServiceLocator.ServiceLocator.Get<GameDeck>();
            
            var fieldGenerator = ServiceLocator.ServiceLocator.Get<FieldGenerator>();
            var cardGenerator = ServiceLocator.ServiceLocator.Get<CardGenerator>();
            
            var blockCount = size.x * size.y;
            
            var colorNormalized = colorSettings.GetNormalizedPercentages(blockCount);
            var cardTypeNormalized = cardTypeSettings.GetNormalizedPercentages();

            var colorRandomizer = new DictionaryRandomizer<GameColor>();
            var typeRandomizer = new DictionaryRandomizer<CardType>();

            var uiManager = ServiceLocator.ServiceLocator.Get<UIManager>();
            var inputHandler = ServiceLocator.ServiceLocator.Get<PlayerInputHandler>();
            
            var colors = colorRandomizer.Randomize(colorNormalized, blockCount);
            var types = typeRandomizer.Randomize(cardTypeNormalized, cardTypeSettings.TotalCount);

            var moveChecker = ServiceLocator.ServiceLocator.Get<MoveChecker>();
            var dealer = ServiceLocator.ServiceLocator.Get<Dealer>();
            
            gameField.Initialize(size.y, size.x, fieldSettings.PlayerCount);
            blockStorage.Initialize(size.y, size.x);
            
            var startBlocks = fieldGenerator.Generate(colors);
            colors = colorRandomizer.Randomize(colorNormalized, cardTypeSettings.TotalCount);
            cardGenerator.Generate(colors, types, cardTypeSettings.TotalCount, cardStorage, gameDeck);

            var players = playerGenerator.Generate();
            
            turnController.AssignPlayers(players);

            var playerHands = new PlayerHand[players.Length];

            for (int i = 0; i < players.Length; i++)
            {
                var current = players[i];
                current.SetId();
                gameField.SetMarker(current.Data.Marker);
                var human = current as HumanPlayer;
                if (human)
                {
                    var lookMode = human.LookMode;
                    
                    gameField.onAnimationStateChanged.AddListener(lookMode.ChangeAccesible);

                    lookMode.AddEnabledListeners
                    (
                        () => inputHandler.SetCursorState(false),
                        uiManager.HideSidebar,
                        uiManager.HideCurrentPlayerHand,
                        () => uiManager.SwitchLookModeText(false)
                    );

                    lookMode.AddDisabledListeners
                    (
                        () => inputHandler.SetCursorState(true),
                        uiManager.ShowSidebar,
                        uiManager.ShowCurrentPlayerHand,
                        () => uiManager.SwitchLookModeText(true)
                    );
                    
                    var dmgFxHandler = ServiceLocator.ServiceLocator.Get<DamageEffectHandler>();
                    
                    dmgFxHandler.onEffectsStart.AddListener(human.LookMode.Disable);
                    dmgFxHandler.onEffectsOver.AddListener(human.LookMode.Enable);

                    if (human == TurnController.ThisPlayer)
                    {
                        var camShaker = ServiceLocator.ServiceLocator.Get<CameraShaker>();
                        dmgFxHandler.CamShaker = camShaker;
                        camShaker.OnShakeOver += dmgFxHandler.Stop;
                        human.Health.onDamage.AddListener(dmgFxHandler.StartEffect);
                    }
                }
                else
                {
                    moveChecker.onTrapChecked.AddListener(current.GetComponent<AIBehavior>().OnTrapPlaced);
                }
                // else
                // {
                //     dealer.startDealOver.AddListener((current as AIPlayer).UpdatePath);
                // }
                
                

                current.onDestinationReached.AddListener(turnController.OnTurnOver);
                current.BlockChanged += gameField.OnBlockChanged;
                blockController.PlacePlayerOnStartBlock(current, startBlocks[i]);
                
                playerHands[i] = players[i].Data.Hand;
                playerHands[i].SetCardLimit(cardTypeSettings.CardLimit);
                uiManager.AddUIComponents(gameField, playerHands);
            }
            
            dealer.Initialize(cardTypeSettings.StartDealCount, fieldSettings.PlayerCount, playerHands);
        }
    }
}