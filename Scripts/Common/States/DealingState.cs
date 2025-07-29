using DeathFortUnoCard.Scripts.Common.Players.Inputs;
using DeathFortUnoCard.Scripts.Common.States.Base;

namespace DeathFortUnoCard.Scripts.Common.States
{
    public class DealingState : CommonState
    {
        public override void On()
        {
            ServiceLocator.ServiceLocator.Get<PlayerInputHandler>().SetCursorState(false);
            base.On();
        }
    }
}