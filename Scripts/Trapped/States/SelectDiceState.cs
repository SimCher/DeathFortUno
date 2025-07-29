using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using DeathFortUnoCard.Scripts.Common.States.Base;
using DeathFortUnoCard.Scripts.Trapped.States.Base;
using DeathFortUnoCard.UI.Scripts.Components;
using UnityEngine;
using UnityEngine.Events;

namespace DeathFortUnoCard.Scripts.Trapped.States
{
    public class SelectDiceState : BaseTrapState
    {
        private DiceKeeper _diceKeeper;

        public UnityEvent onSuicideEnter;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            onSuicideEnter.RemoveAllListeners();
        }

        protected override void TrapStateInitialize()
        {
            base.TrapStateInitialize();

            _diceKeeper = ServiceLocator.Get<DiceKeeper>();
        }

        public override void On()
        {
            if(owner.IsSuicideTrap)
                onSuicideEnter?.Invoke();
            else
                base.On();
        }
    }
}