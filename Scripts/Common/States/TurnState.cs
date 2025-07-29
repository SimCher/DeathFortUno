using DeathFortUnoCard.Scripts.Common.States.Base;

namespace DeathFortUnoCard.Scripts.Common.States
{
    public class TurnState : PlayerState, ITurnState, IMultipleState
    {
        private TurnController _turnController;

        protected override void Start()
        {
            base.Start();
            _turnController = ServiceLocator.ServiceLocator.Get<TurnController>();
        }

        protected override void PlayerLogic()
        {
            TurnController.ThisPlayer.LookMode.Disable();
        }

        public void HandleInput()
        {
            if (!turnController.IsThisPlayerTurn)
            {
                TurnController.ThisPlayer.SetControllerActivity(true);
                return;
            }

            if (input.LookModeBtnDown)
            {
                TurnController.ThisPlayer.LookMode.SwitchActivity();
            }

            if (input.SkipBtnDown && !gameDeck.IsVisible)
            {
                _turnController.OnTurnOver();
            }
        }
    }
}