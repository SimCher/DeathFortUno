using DeathFortUnoCard.Scripts.Common.Players.Inputs;
using DeathFortUnoCard.Scripts.Common.States.Base;
using DeathFortUnoCard.Scripts.Utils;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.Common.States
{
    [RequireComponent(typeof(Timer))]
    public class StartState : CommonState
    {
        public override void On()
        {
            base.On();
            ServiceLocator.ServiceLocator.Get<PlayerInputHandler>().SetCursorState(false);
        }
    }
}