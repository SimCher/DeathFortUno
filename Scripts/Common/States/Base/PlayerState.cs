using DeathFortUnoCard.Scripts.Common.Managers;
using DeathFortUnoCard.Scripts.Common.Players;
using DeathFortUnoCard.Scripts.Common.Players.Inputs;
using DeathFortUnoCard.UI.Scripts.Components;
using UnityEngine.Events;

namespace DeathFortUnoCard.Scripts.Common.States.Base
{
    public abstract class PlayerState : CommonState
    {
        public UnityEvent onPlayerStarted;
        public UnityEvent onNpcStarted;

        protected TurnController turnController;
        protected GameFlowManager gameManager;
        protected GameDeck gameDeck;
        protected PlayerInputHandler input;
        
        protected virtual void PlayerLogic() { }
        
        protected virtual void NpcLogic() { }

        protected virtual void Start()
        {
            input = ServiceLocator.ServiceLocator.Get<PlayerInputHandler>();
            turnController = ServiceLocator.ServiceLocator.Get<TurnController>();
            gameManager = ServiceLocator.ServiceLocator.Get<GameFlowManager>();
            gameDeck = ServiceLocator.ServiceLocator.Get<GameDeck>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            onPlayerStarted.RemoveAllListeners();
            onNpcStarted.RemoveAllListeners();
        }

        public override void On()
        {
            base.On();

            var human = turnController.CurrentPlayer as HumanPlayer;
            if (human)
            {
                onPlayerStarted?.Invoke();
                PlayerLogic();
            }
            else
            {
                onNpcStarted?.Invoke();
                NpcLogic();
            }
            
        }

        public override void Off()
        {
            onExit?.Invoke();
        }
    }
}