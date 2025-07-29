using DeathFortUnoCard.Scripts.Common.Players.Inputs;
using DeathFortUnoCard.Scripts.Common.States.Base;
using UnityEngine.Events;

namespace DeathFortUnoCard.Scripts.Common.States
{
    public class TrapState : PlayerState
    {
        private PlayerInputHandler _inputHandler;

        public UnityEvent<DuelData> onInitiated;
        
        protected override void Start()
        {
            base.Start();
            _inputHandler = ServiceLocator.ServiceLocator.Get<PlayerInputHandler>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            onInitiated.RemoveAllListeners();
        }

        public override void On()
        {
            base.On();
            _inputHandler.SetCursorState(turnController.IsThisPlayerTurn);
        }

        public override void Off()
        {
            base.Off();
            _inputHandler.SetCursorState(true);
        }
    }
}