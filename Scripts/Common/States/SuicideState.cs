using DeathFortUnoCard.Scripts.Common.States.Base;
using DeathFortUnoCard.Scripts.Trapped;

namespace DeathFortUnoCard.Scripts.Common.States
{
    public class SuicideState : PlayerState
    {
        protected override void PlayerLogic()
        {
            base.PlayerLogic();
            input.SetCursorState(false);

            if (turnController.IsThisPlayerTurn)
            {
                TurnController.ThisPlayer.SetControllerActivity(false);
                TurnController.ThisPlayer.Stress.Enable(ShootMode.Suicide, true);
            }
        }

        public override void Off()
        {
            base.Off();
            TurnController.ThisPlayer.Stress.Disable();
        }
    }
}