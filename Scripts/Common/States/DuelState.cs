using DeathFortUnoCard.Scripts.Common.States.Base;
using DeathFortUnoCard.Scripts.Trapped;

namespace DeathFortUnoCard.Scripts.Common.States
{
    public class DuelState : PlayerState
    {
        protected override void PlayerLogic()
        {
            base.PlayerLogic();
            input.SetCursorState(false);

            if (turnController.IsThisPlayerTurn)
            {
                TurnController.ThisPlayer.SetControllerActivity(true);
                TurnController.ThisPlayer.Stress.Enable(ShootMode.Duel, true);
            }
        }

        public override void Off()
        {
            base.Off();
            TurnController.ThisPlayer.Stress.Disable();
        }
    }
}